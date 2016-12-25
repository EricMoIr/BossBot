using System;
using Discord;
using Discord.Commands;
using Discord.Modules;
using DataManager.XML;
using BossBot;
using System.Collections.Generic;

namespace EntryPoint
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Map> maps = new List<Map>();
            Map m = new Map() { Name = "guards graveyard"};
            maps.Add(m);
            List<Boss> bosses = new List<Boss>();
            Boss b = new Boss() { Name = "abomination", RespawnTime = 2, SpawnMaps = maps, Type = "gimmick" };
            Spawn s = new Spawn(m, DateTime.Now);
            b.Spawns[1] = s;
            bosses.Add(b);
            XMLFileManager.WriteFile(".\\Files\\bosses.txt", bosses);
            //Map m = XMLFileManager.ReadFile<Map>(".\\Files\\maps.txt");
            //List<Boss> abosses = XMLFileManager.ReadFile<List<Boss>>(".\\Files\\bosses.txt");
            Start();
            Console.Read();
        }
        public static void Start()
        {

            DiscordClient client;
            string botToken = "MjU5MzM0NDA2Njk3NjQ4MTI5.Czd_YQ.RXOO2rFzuIv5OLv5Bdmdt0de9FQ";
            client = new DiscordClient(c =>
                {
                    c.AppName = "BossBot";
                    c.LogLevel = LogSeverity.Error;
                    c.LogHandler = Log;
                });
            client.ExecuteAndWait(async () =>
            {
                await client.Connect(botToken, TokenType.Bot);
                Console.Clear();
                Console.Title = string.Format($"Currently connected as a {"BOT"}.");
                client.UsingCommands(c =>
                        {
                            c.PrefixChar = '.';
                            c.AllowMentionPrefix = false;
                            c.IsSelfBot = false;
                            c.HelpMode = HelpMode.Public;
                        });
                client.UsingModules();
                client.AddModule<MyModule>("MyModule", ModuleFilter.None);
            });
        }

        private static void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Exception}] [{e.Source}] {e.Message}");
        }
    }
}
