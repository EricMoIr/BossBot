using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossBot
{
    public class Spawn : Tuple<Map, DateTime>
    {
        public Spawn() : base(new Map(), DateTime.Now)
        {
        }
        public Spawn(Map map, DateTime time) : base(map, time)
        {
        }
    }
}
