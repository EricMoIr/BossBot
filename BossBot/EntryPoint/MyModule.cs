using BossBot;
using BusinessLogic;
using Discord.Commands;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace EntryPoint
{
    public class MyModule : IModule
    {
        private ServiceBosses serviceBosses = new ServiceBosses();
        private List<AlarmClock> alarms = new List<AlarmClock>();
        private static int REMINDER_MINUTES = int.Parse(ConfigurationManager.AppSettings["reminderTime"]);
        private static readonly string SPAWN_FORMAT = ".spawn bossName channel";
        private static readonly string CLEAR_FORMAT = ".clear bossName channel";
        private static readonly string KILLED_FORMAT = ".killed bossName channel hh.mm map";
        public void Install(ModuleManager manager)
        {
            manager.CreateCommands("", c =>
            {
                CreateKilledCommand(c);
                CreateSpawnCommand(c);
                CreateClearCommand(c);
                CreateSpawnsCommand(c);
            });
        }

        private void CreateKilledCommand(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand("killed")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string[] args = e.Args[0].Split(' ');
                    try
                    {
                        serviceBosses.UpdateSpawn(args[0], int.Parse(args[1]), args[2], args[3]);
                        AddBossReminder(e);
                        await e.Channel.SendMessage("`Got it.`");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        await e.Channel.SendMessage("`You are missing parameters. The format is \".killed bossName channel hh.mm map\". Pantsu`");
                    }
                    catch (ArgumentException ex)
                    {
                        await e.Channel.SendMessage($"`{ex.Message}`");
                    }
                    catch (Exception)
                    {
                        await PrintFormat(e, KILLED_FORMAT);
                    }
                });
        }

        private async void AddBossReminder(CommandEventArgs e)
        {
            string[] args = e.Args[0].Split(' ');
            DateTime spawnTime = serviceBosses.GetSpawnTime(args[0], int.Parse(args[1]));
            DateTime alarmTime = spawnTime - new TimeSpan(0, REMINDER_MINUTES, 0);
            if(alarmTime.CompareTo(DateTime.Now) > 0)
            {
                AlarmClock clock = new AlarmClock(alarmTime);
                var spawn = serviceBosses.GetSpawn(args[0], int.Parse(args[1]));
                clock.Alarm += async (sender, ev)
                    => await PrintSpawn(e, Tuple.Create(serviceBosses.GetBoss(args[0]),
                    int.Parse(args[1])), spawn);
                alarms.Add(clock);
            }
            else
            {
                await e.Channel.SendMessage("`The time entered is invalid. The boss would have already spawned`");
            }
        }

        private void CreateSpawnsCommand(CommandGroupBuilder c)
        {
            c.CreateCommand("spawns")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) =>
            {
                try
                {
                    await PrintSpawns(serviceBosses.GetAllSpawns(), e);
                }
                catch(Exception ex)
                {
                    await e.Channel.SendMessage(ex.Message);
                }
            });
        }

        private void CreateClearCommand(CommandGroupBuilder c)
        {
            c.CreateCommand("clear")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string[] args = e.Args[0].Split(' ');
                    int channel = args.Length > 1 ? int.Parse(args[1]) : 1;
                    try
                    {
                        serviceBosses.ClearSpawn(args[0], channel);
                        await e.Channel.SendMessage($"`The spawn of {serviceBosses.GetBoss(args[0])} on channel {channel} was cleared`");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        await PrintFormat(e, CLEAR_FORMAT);
                    }
                    catch (FormatException)
                    {
                        await PrintFormat(e, CLEAR_FORMAT);
                    }
                    catch (ArgumentException ex)
                    {
                        await e.Channel.SendMessage($"`{ex.Message}`");
                    }
                    catch (Exception)
                    {
                        await PrintFormat(e, CLEAR_FORMAT);
                    }
                });
        }

        private void CreateSpawnCommand(CommandGroupBuilder c)
        {
            c.CreateCommand("spawn")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    Tuple<Map, DateTime> spawn = null;
                    string[] args = e.Args[0].Split(' ');
                    try
                    {
                        int channel = args.Length > 1 ? int.Parse(args[1]) : 1;
                        spawn = serviceBosses.GetSpawn(args[0], channel);
                        await PrintSpawn(e, Tuple.Create(serviceBosses.GetBoss(args[0]), channel), spawn);
                    }
                    catch (ArgumentException ex)
                    {
                        await e.Channel.SendMessage($"`{ex.Message}`");
                    }
                    catch (Exception)
                    {
                        await PrintFormat(e, SPAWN_FORMAT);
                    }
                });
        }

        private static async Task PrintFormat(CommandEventArgs e, string format)
        {
            await e.Channel.SendMessage($"`Git gud. The format is \"{format}\"`");
        }

        private static async Task PrintSpawn(CommandEventArgs e, Tuple<Boss, int> boss, Tuple<Map, DateTime> spawn)
        {
            await e.Channel.SendMessage($"`{boss.Item1.ToString()} will spawn in channel {boss.Item2} at {spawn.Item1.ToString()} at {spawn.Item2.TimeOfDay.ToString()}`");
        }

        private static async Task PrintSpawns(Dictionary<Tuple<Boss, int>, Spawn> spawns, CommandEventArgs e)
        {
            foreach (KeyValuePair<Tuple<Boss, int>, Spawn> entry in spawns)
            {
                await PrintSpawn(e, entry.Key, entry.Value);
            }
        }
    }
}
