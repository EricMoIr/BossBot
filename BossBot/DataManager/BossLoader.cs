using BossBot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DataManager
{
    public static class BossLoader
    {
        private static readonly string BOSSES_PATH = ConfigurationManager.AppSettings["bossesPath"];
        private static readonly string BOSSES_DIRECTORY_PATH = ConfigurationManager.AppSettings["bossesDirectoryPath"];
        public static List<Boss> GetBosses()
        {
            List<Boss> output = new List<Boss>();
            if (!Directory.Exists(BOSSES_DIRECTORY_PATH))
            {
                var dInfo = Directory.CreateDirectory(BOSSES_DIRECTORY_PATH);
            }
            if (!File.Exists(BOSSES_PATH))
            {
                var stream = File.Create(BOSSES_PATH);
                stream.Close();
            }
            var lines = File.ReadLines(BOSSES_PATH);
            foreach (string line in lines)
            {
                try
                {
                    string[] args = line.Split('-');
                    string bossName = args[0];
                    string type = args[1];
                    int respawnTime = int.Parse(args[2]);
                    List<Map> respawnMaps = new List<Map>();
                    for (int i = 3; i < args.Count(); i++)
                    {
                        string mapName = args[i];
                        Map m = new Map() { Name = mapName };
                        respawnMaps.Add(m);
                    }
                    Boss boss = null;
                    if (type == "gimmick")
                    {
                        boss = new Boss()//GimmickBoss()
                        {
                            Name = bossName,
                            RespawnTime = respawnTime,
                            SpawnMaps = respawnMaps
                        };
                    }
                    else
                    {
                        boss = new Boss()
                        {
                            Name = bossName,
                            RespawnTime = respawnTime,
                            SpawnMaps = respawnMaps
                        };
                    }
                    output.Add(boss);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException("You are missing arguments in your boss file");
                }
                catch (FormatException)
                {
                    throw new ArgumentException("The respawn time is not a number");
                }
            }
            return output;
        }
    }
}
