using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossBot
{
    public class Boss
    {
        //la primera posicion me la estoy pasando por los huevos
        public Spawn[] Spawns { get; }

        public string Name { get; set; }
        public List<Map> SpawnMaps { get; set; }
        public string Type { get; set; }
        public int RespawnTime { get; set; }

        public Boss()
        {
            Spawns = new Spawn[10];
        }

        public int FindIndexOfSpawn(Map map)
        {
            return SpawnMaps.FindIndex(m => m.NameContains(map));
        }
        public override bool Equals(object obj)
        {
            Boss b = obj as Boss;
            if (b != null)
            {
                return Name.ToLower().Equals(b.Name.ToLower());
            }
            return false;
        }
        /*
         * parameters = spawnMap
         */
        public virtual void AddSpawnTime(DateTime timeOfKill, params object[] parameters)
        {
            Map mapOfKill = (Map)parameters[0];
            Map spawnMap = GetNextSpawnMap(mapOfKill);
            DateTime timeOfSpawn = GetTimeOfNextSpawn(timeOfKill);
            Spawns[1] = new Spawn(spawnMap, timeOfSpawn);
        }

        //va a tirar excepcion de limites
        public virtual Spawn GetSpawnTime(int ch = 1)
        {
            return Spawns[ch];
        }

        public Map GetNextSpawnMap(Map map)
        {
            int index = FindIndexOfSpawn(map);
            if (index < 0)
                throw new ArgumentException($"That isn't one of {Name}'s spawn maps");
            if (index == SpawnMaps.Count() - 1)
                return SpawnMaps[0];
            return SpawnMaps[index + 1];
        }

        public bool NameContains(Boss boss)
        {
            return Name.ToLower().Contains(boss.Name.ToLower());
        }
        public override string ToString()
        {
            return Name;
        }
        protected DateTime GetTimeOfNextSpawn(DateTime timeOfKill)
        {
            return timeOfKill + new TimeSpan(RespawnTime, 0, 0);
        }

        public void ClearSpawn(int channel)
        {
            if (Spawns[channel] != null)
            {
                Spawns[channel] = null;
            }
            else
                throw new ArgumentException("Sorry but I can't clear a timer I don't know. Pantsu");
        }
    }
}
