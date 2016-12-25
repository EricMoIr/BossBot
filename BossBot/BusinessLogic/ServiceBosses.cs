using BossBot;
using DataManager;
using DataManager.XML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic
{
    public class ServiceBosses
    {
        private IUnitOfWork iow;
        private IRepository<Boss> bosses;
        public ServiceBosses()
        {
            IUnitOfWork iow = new XMLUnitOfWork();
            //BossLoader.GetBosses();
            bosses = iow.BossRepository;
        }
        public Boss GetBoss(string name)
        {
            Boss boss = new Boss() { Name = name };
            return bosses.Find(x => x.NameContains(boss));
            //int index = BossLoader.GetBosses().FindIndex(x => x.NameContains(boss));
            //if (index < 0)
            //throw new ArgumentException("The boss doesn't exist");
            //return BossLoader.GetBosses()[index];
        }

        private Map GetNextMap(Boss boss, Map map)
        {
            int index = boss.FindIndexOfSpawn(map);
            if (index < 0)
                throw new ArgumentException($"That isn't one of {boss.Name}'s spawn maps");
            if (index == boss.SpawnMaps.Count() - 1)
                return boss.SpawnMaps[0];
            return boss.SpawnMaps[index + 1];
        }

        public void UpdateSpawn(string bossName, DateTime time, int channel, string mapName)
        {
            Boss boss = GetBoss(bossName);
            Map map = GetMapOfBoss(boss, mapName);
            boss.AddSpawnTime(time, map);
            bosses.Update(boss);
        }

        public void UpdateSpawn(string bossName, DateTime time, int channel)
        {
            Boss boss = GetBoss(bossName);
            Map map = GetMapOfBoss(boss);
            boss.AddSpawnTime(time, map);
            bosses.Update(boss);
        }
        private Map GetMapOfBoss(Boss boss)
        {
            if (boss.SpawnMaps.Count > 1)
            {
                throw new ArgumentException($"{boss.Name} spawns in more than one map. Specify the map");
            }
            return boss.SpawnMaps[0];
        }

        public List<Boss> GetBosses()
        {
            return (List<Boss>)bosses.Get();
        }

        private Map GetMapOfBoss(Boss boss, string mapName)
        {
            Map map = new Map() { Name = mapName };
            int index = boss.FindIndexOfSpawn(map);
            if (index < 0)
                throw new ArgumentException($"That isn't one of {boss.Name}'s spawn maps. Pantsu");
            return boss.SpawnMaps[index];
        }
        public Spawn GetSpawn(string bossName, int channel = 1)
        {
            Boss boss = GetBoss(bossName);
            Spawn spawn = boss.GetSpawnTime(channel);
            if (spawn != null)
                return spawn;
            throw new ArgumentException($"Sorry but I don't know the timer of {boss.Name}. Pantsu");
        }
        public DateTime GetSpawnTime(string bossName, int channel)
        {
            return GetSpawn(bossName, channel).Time;
        }

        public Dictionary<Tuple<Boss, int>, Spawn> GetAllSpawns()
        {
            Dictionary<Tuple<Boss, int>, Spawn> spawns = new Dictionary<Tuple<Boss, int>, Spawn>();
            List<Boss> bosses = (List<Boss>)this.bosses.Get();
            foreach (Boss boss in bosses)
            {
                for (int i = 1; i < boss.Spawns.Count(); i++)
                {
                    Spawn spawn = boss.Spawns[i];
                    if (spawn != null)
                    {
                        spawns.Add(new Tuple<Boss, int>(boss, i), spawn);
                    }
                }
            }
            if (spawns.Count() > 0)
                return spawns;
            throw new ArgumentException("There are no spawns registered. Pantsu");
        }

        public void ClearSpawn(string bossName, int channel)
        {
            Boss boss = GetBoss(bossName);
            boss.ClearSpawn(channel);
            bosses.Update(boss);
        }
        private IEnumerable<Tuple<U, T>> GetKeysEqual<U, T, K>(Dictionary<Tuple<U, T>, K> d, Tuple<U, T> tuple)
        {
            return d.Keys.Where(key => key.Item1.Equals(tuple.Item1) && key.Item2.Equals(tuple.Item2));
        }
        private void SetKeysEqual<U, T, K>(Dictionary<Tuple<U, T>, K> d, Tuple<U, T> tuple, K value)
        {
            var myKeys = d.Keys.Where(key => key.Item1.Equals(tuple.Item1) && key.Item2.Equals(tuple.Item2));
            if (myKeys.Count() > 0)
            {
                d[myKeys.First()] = value;
            }
            else
            {
                d[tuple] = value;
            }
        }

        public void UpdateSpawn(string bossName, DateTime time)
        {
            Boss boss = GetBoss(bossName);
            if (boss == null)
            {
                throw new ArgumentException($"{boss.Name} doesn't exist");
            }
            if (boss.SpawnMaps.Count == 1)
            {
                boss.AddSpawnTime(time, boss.SpawnMaps[0]);
                bosses.Update(boss);
            }
            else
            {
                throw new ArgumentException($"{boss.Name} has more than one spawn map. Specify where you killed it.");
            }
        }
    }
}
