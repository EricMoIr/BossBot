using BossBot;
using System;

namespace DataManager.XML.Models
{
    [Serializable]
    public class BossXML : Boss, IModel<Boss>
    {
        public BossXML()
        {

        }

        public Boss Get()
        {
            return new Boss()
            {
                Name = Name,
                RespawnTime = RespawnTime,
                SpawnMaps = SpawnMaps,
                Type = Type
            };
        }
    }
}
