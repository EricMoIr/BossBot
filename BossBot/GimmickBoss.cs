using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossBot
{
    public class GimmickBoss : Boss
    {
        private Map SpawnMap()
        {
            return SpawnMaps.First();
        }
        public override void AddSpawnTime(DateTime timeOfKill, params object[] parameters)
        {
            int ch = (int)parameters[0];
            DateTime timeOfSpawn = GetTimeOfNextSpawn(timeOfKill);
            Spawns[ch] = new Spawn(SpawnMap(), timeOfSpawn);
        }
    }
}
