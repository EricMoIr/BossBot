using BossBot;
using BusinessLogic;
using Discord.Commands;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
        private static readonly string KILLED_FORMAT = ".killed bossName hh.mm channel map";
        private static readonly string KILLED_ONE_CH_FORMAT = ".killed bossName hh.mm map";
        private static readonly string KILLED_ONE_MAP_FORMAT = ".killed bossName hh.mm channel";
        private static readonly string KILLED_ONE_CH_ONE_MAP_FORMAT = ".killed bossName hh.mm";
        private static readonly string SPAWNS_FORMAT = ".spawns";
        private static readonly string BOSSES_FORMAT = ".bosses";
        private static readonly string PROTIPS_FORMAT = ".protips";
        private static readonly int DIFF_IN_HOURS_WITH_SERVER = 2;
        public void Install(ModuleManager manager)
        {
            manager.CreateCommands("", cgb =>
            {
                CreateCommandsCommand(cgb);
                CreateProtipsCommand(cgb);
                CreateKilledCommand(cgb);
                CreateSpawnCommand(cgb);
                CreateClearCommand(cgb);
                CreateSpawnsCommand(cgb);
                CreateBossesCommand(cgb);
            });
        }

        private void CreateBossesCommand(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand("bosses")
                .Do(async (e) =>
                {
                    List<Boss> bosses = serviceBosses.GetBosses();
                    if(bosses.Count == 0)
                    {
                        await PrintMessage(e, "I don't know any boss. Derp");
                    }
                    else
                    {
                        await PrintMessage(e, "These are the bosses I know:");
                    }
                    string message = "";
                    foreach (Boss boss in bosses)
                    {
                        message += boss.ToString() + "\n";
                    }
                    await PrintMessage(e, message);
                });
        }

        private void CreateKilledCommand(CommandGroupBuilder cgb)
        {
            CreateKilledTwoParamCommand(cgb);
            CreateKilledThreeParamCommand(cgb);
            CreateKilledDefaultCommand(cgb);
        }

        private void CreateCommandsCommand(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand("commands")
                .Do(async (e) =>
                {
                    string commands = BOSSES_FORMAT + "\n"
                       + CLEAR_FORMAT + "\n"
                       + KILLED_FORMAT + "\n"
                       + PROTIPS_FORMAT + "\n"
                       + SPAWN_FORMAT + "\n"
                       + SPAWNS_FORMAT;
                    await PrintMessage(e, commands);
                });
        }
        private void CreateProtipsCommand(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand("protips")
                .Do(async (e) =>
                {
                    string oneCh = "If the boss only spawns in one channel, you can use\n" + KILLED_ONE_CH_FORMAT;
                    await PrintMessage(e, oneCh);
                    string oneMap = "If the boss only spawns in one map, you can use\n" + KILLED_ONE_MAP_FORMAT;
                    await PrintMessage(e, oneMap);
                    string oneChOneMap = "If the boss only spawns in one channel and one map, you can use\n" + KILLED_ONE_CH_ONE_MAP_FORMAT;
                    await PrintMessage(e, oneChOneMap);
                });
        }
        private void CreateKilledTwoParamCommand(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand("killed")
                .Parameter("name")
                .Parameter("time")
                .Do(async (e) =>
                {
                    string name = "";
                    try
                    {
                        name = e.Args[0];
                        DateTime time = StringToTime(e.Args[1]);
                        serviceBosses.UpdateSpawn(name, time);
                        AddBossReminder(e, name);
                        await PrintMessage(e, "Got it");
                    }
                    catch (InvalidOperationException ex)
                    {
                        serviceBosses.ClearSpawn(name, 1);
                        await PrintMessage(e, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        await PrintMessage(e, ex.Message);
                    }
                });
        }
        private void CreateKilledThreeParamCommand(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand("killed")
                .Parameter("name")
                .Parameter("time")
                .Parameter("MapOrChannel")
                .Do(async (e) =>
                {
                    string name = "";
                    int defaultChannel = 1;
                    int channel = 1;
                    try
                    {
                        name = e.Args[0];
                        DateTime time = StringToTime(e.Args[1]);
                        if (int.TryParse(e.Args[2], out channel))
                        {
                            serviceBosses.UpdateSpawn(name, time, channel);
                            AddBossReminder(e, name, channel);
                        }
                        else
                        {
                            string map = e.Args[2];
                            serviceBosses.UpdateSpawn(name, time, defaultChannel, map);
                            AddBossReminder(e, name, defaultChannel);
                        }
                        await PrintMessage(e, "Got it");
                    }
                    catch (InvalidOperationException ex)
                    {
                        if(channel == 0)
                        {
                            serviceBosses.ClearSpawn(name, defaultChannel);
                        }
                        else
                        {
                            serviceBosses.ClearSpawn(name, channel);
                        }
                        await PrintMessage(e, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        await PrintMessage(e, ex.Message);
                    }
                });
        }

        private void CreateKilledDefaultCommand(CommandGroupBuilder cgb)
        {
            cgb.CreateCommand("killed")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    string[] args = e.Args[0].Split(' ');
                    try
                    {
                        string bossName = args[0];
                        DateTime time = StringToTime(args[1]);
                        int channel = int.Parse(args[2]);
                        serviceBosses.UpdateSpawn(bossName, time, channel, args[3]);
                        AddBossReminder(e, bossName, channel);
                        await PrintMessage(e, "Got it.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        await PrintMessage(e, "You are missing parameters. The format is \".killed bossName channel hh.mm map\". Pantsu");
                    }
                    catch (ArgumentException ex)
                    {
                        await PrintMessage(e, ex.Message);
                    }
                    catch (Exception)
                    {
                        await PrintFormat(e, KILLED_FORMAT);
                    }
                });
        }
        private void AddBossReminder(CommandEventArgs e, string name, int channel = 1)
        {
            DateTime spawnTime = serviceBosses.GetSpawnTime(name, channel);
            DateTime alarmTime = spawnTime + new TimeSpan(DIFF_IN_HOURS_WITH_SERVER, 0, 0) - new TimeSpan(0, REMINDER_MINUTES, 0);
            if (alarmTime.CompareTo(DateTime.Now) > 0)
            {
                AlarmClock clock = new AlarmClock(alarmTime);
                var spawn = serviceBosses.GetSpawn(name, channel);
                clock.Alarm += async (sender, ev)
                    => await PrintSpawnWithMention(e, 
                    Tuple.Create(serviceBosses.GetBoss(name), channel), 
                    spawn,
                    "@here");
                alarms.Add(clock);
            }
            else
            {
                throw new InvalidOperationException("The time entered is invalid. The boss would have already spawned");
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
                catch (Exception ex)
                {
                    await PrintMessage(e, ex.Message);
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
                        await PrintMessage(e, $"The spawn of {serviceBosses.GetBoss(args[0])} on channel {channel} was cleared");
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
                        await PrintMessage(e, $"{ex.Message}");
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
                    string[] args = e.Args[0].Split(' ');
                    try
                    {
                        int channel = args.Length > 1 ? int.Parse(args[1]) : 1;
                        Spawn spawn = serviceBosses.GetSpawn(args[0], channel);
                        await PrintSpawnWithMention(e, Tuple.Create(serviceBosses.GetBoss(args[0]), channel), spawn, "@here");
                    }
                    catch (ArgumentException ex)
                    {
                        await PrintMessage(e, $"{ex.Message}");
                    }
                    catch (Exception)
                    {
                        await PrintFormat(e, SPAWN_FORMAT);
                    }
                });
        }

        private static async Task PrintFormat(CommandEventArgs e, string format)
        {
            await PrintMessage(e, $"Git gud. The format is \"{format}\"");
        }

        private static async Task PrintSpawn(CommandEventArgs e, Tuple<Boss, int> boss, Spawn spawn)
        {
            await PrintMessage(e, $"{boss.Item1.ToString()} will spawn in channel {boss.Item2} at {spawn.Map.ToString()} at {spawn.Time.TimeOfDay.ToString()}");
        }
        private static async Task PrintSpawnWithMention(CommandEventArgs e, Tuple<Boss, int> boss, Spawn spawn, string mention)
        {
            await PrintMessage(e, $"{boss.Item1.ToString()} will spawn in channel {boss.Item2} at {spawn.Map.ToString()} at {spawn.Time.TimeOfDay.ToString()}", mention);
        }

        private static async Task PrintSpawns(Dictionary<Tuple<Boss, int>, Spawn> spawns, CommandEventArgs e)
        {
            await PrintMessage(e, "", "@here");
            foreach (KeyValuePair<Tuple<Boss, int>, Spawn> entry in spawns)
            {
                await PrintSpawn(e, entry.Key, entry.Value);
            }
        }
        private static async Task PrintMessage(CommandEventArgs e, string message, string mention)
        {
            await e.Channel.SendMessage(mention);
            await PrintMessage(e, message);
        }
        private static async Task PrintMessage(CommandEventArgs e, string message)
        {
            await e.Channel.SendMessage($"```{message}```");
        }

        private static DateTime StringToTime(string time)
        {
            return DateTime.ParseExact(time, "HH.mm", CultureInfo.InvariantCulture);
        }
    }
}
