using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossBot
{
    [Serializable]
    public class Spawn
    {
        public Map Map { get; set; }
        public DateTime Time { get; set; }
        public Spawn()
        {
        }
        public Spawn(Map map, DateTime time)
        {
            Map = map;
            Time = time;
        }
    }
}
