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
