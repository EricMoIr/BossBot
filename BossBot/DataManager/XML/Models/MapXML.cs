using BossBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.XML.Models
{
    [Serializable]
    public class MapXML : Map, IModel<Map>
    {
        public MapXML()
        {

        }

        public Map Get()
        {
            return new Map()
            {
                Name = Name
            };
        }
    }
}
