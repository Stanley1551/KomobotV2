using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using KomobotV2.DataAccess;
using KomobotV2.Logger;

namespace KomobotV2
{
    class Program
    {
        static VoiceNextClient voiceClient;
        static CommandsNextModule commands;
        static private object thisLock = new object();
        static bool orderingInProgress = false;
        public static Dictionary<DiscordMember, DateTime> gameStartedDictionary;

        static TimeSpan ts = new TimeSpan();

        public static JsonConfig config;
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            config = new JsonParser().Config;
            gameStartedDictionary = new Dictionary<DiscordMember, DateTime>();
            KomoLogger logger = new KomoLogger();
            DiscordClient client = new DiscordClient(new DiscordConfiguration()
            {
                TokenType = TokenType.Bot,
                Token = config.DiscordAPIKey,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
            });

                client.PresenceUpdated += async (e) =>
                {
                    if(Komobase.IsSubscribed(e.Member.Username))
                    {
                        //if just started playing
                        if(e.Game != null && e.Game.Name != null && !gameStartedDictionary.ContainsKey(e.Member))
                        {
                            gameStartedDictionary.Add(e.Member, DateTime.Now);
                        }
                        //if ended 
                        else if(e.Game == null || e.Game.Name == null || e.Game.Name == string.Empty)
                        {
                            if(gameStartedDictionary.ContainsKey(e.Member))
                            {
                                DiscordDmChannel dm = await client.CreateDmAsync(e.Member);

                                await client.SendMessageAsync(dm, "No! Ennyit függtél most: " +
                                    GetTimeLapsedString(DateTime.Now, gameStartedDictionary[e.Member]),
                                    false, null);

                                gameStartedDictionary.Remove(e.Member);
                            }
                        }
                    }
                };

            client.GuildMemberRemoved += async (e) =>
            {
                var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1, null);
                AuditLogActionType actionType = log.FirstOrDefault().ActionType;

                switch(actionType)
                {
                    case AuditLogActionType.Ban:
                        {
                            await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(),
                            e.Member.Username+ " kapott egy banhammert!", false);
                            break;
                        }
                    case AuditLogActionType.Kick:
                        {
                            await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(),
                            e.Member.Username + " ki lett rúgva innen!", false);
                            break;
                        }
                }

                DiscordMessage msg = await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(),
                    e.Member.Username + ", bukás!", false);
                
            };

            client.ChannelCreated += async (e) =>
            {
                var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1, null, AuditLogActionType.ChannelCreate);

                if (!log.FirstOrDefault().UserResponsible.IsBot)
                {
                    await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(), "Nocsak! Új " +
                        "kommunikációs csatorna " + e.Channel.Name + " néven!");
                }

                //if (!IsVoiceChannelsOrdered(client, config))
                //{
                //    await OrderVoiceChannels(client, config);
                //}
            };

            //client.Heartbeated += async (e) =>
            //{
            //    if (!orderingInProgress && !IsVoiceChannelsOrdered(client, config))
            //    {
            //        await OrderVoiceChannels(client, config);
            //    }
            //};
            
            client.GuildEmojisUpdated += async (e) =>
            {
                DiscordEmoji added = e.EmojisAfter.Except(e.EmojisBefore).FirstOrDefault();

                await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(), "Hinnye! Új emoji lett létrehozva " + added.CreationTimestamp +
                    "-kor " + added.Name + " néven!");
            };

            client.ChannelDeleted += async (e) =>
            {
                var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1,null,AuditLogActionType.ChannelDelete);

                var mandatoryChannels = config.mandatoryVoiceChannels;

                if(log == null || log.Count < 1)
                {
                    throw new ArgumentException("No entry found");
                }

                var user = log.FirstOrDefault().UserResponsible;

                Console.WriteLine(user.Username + " deleted the " + e.Channel.Name + " channel!");

                if(mandatoryChannels.Contains(e.Channel.Name))
                {
                    await client.Guilds.FirstOrDefault().Value.CreateChannelAsync(e.Channel.Name, e.Channel.Type, e.Channel.Parent,
                        e.Channel.Bitrate, e.Channel.UserLimit, e.Channel.PermissionOverwrites, "Mandatory channel recreation.");

                    await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(),
                        "A(z) " + e.Channel.Name + " kötött csatorna, nem törölhető! " + user.Username + ", ellenőrző!");
                }
            };

            
            voiceClient = client.UseVoiceNext();

            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!",
                EnableDms = true,
                CaseSensitive = false,
                EnableDefaultHelp = true,
            });
            commands.RegisterCommands<Commands>();

            await client.ConnectAsync();
            await Task.Delay(5000);
            await Initialize(client, config);
            await Task.Delay(-1);
        }

        private static string GetTimeLapsedString(DateTime to, DateTime from)
        {
            StringBuilder sb = new StringBuilder();
            var timelapsed = to - from;
            sb = sb.Append(timelapsed.Hours + "óra " + timelapsed.Minutes + "perc " + timelapsed.Seconds + "másodperc ");

            return sb.ToString();
        }

        private static async Task Initialize(DiscordClient client, JsonConfig config)
        {
            //SQLite database
            Komobase komobase = new Komobase();

            var users = client.Guilds.FirstOrDefault().Value.Members.ToList<DiscordMember>();

            komobase.SyncUsers(users);

            //check if any mandatory channel is missing.
            var currentVoiceChannels = client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Type == ChannelType.Voice);
            List<string> currentVoiceChannelNames = new List<string>();

            List<string> mandatoryChannels = config.mandatoryVoiceChannels.Split(';').ToList<string>();

            List<string> missingChannels = new List<string>();

            foreach(DiscordChannel channel in currentVoiceChannels)
            {
                currentVoiceChannelNames.Add(channel.Name);
            }

            foreach(string channelName in mandatoryChannels)
            {
                if(!currentVoiceChannelNames.Contains(channelName))
                {
                    missingChannels.Add(channelName);
                }
            }
            if (missingChannels != null && missingChannels.Count > 0)
            {
                foreach (string channelName in missingChannels)
                {
                    await client.Guilds.FirstOrDefault().Value.CreateChannelAsync(channelName, ChannelType.Voice,
                        client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name.ToLower() == "voice channels").FirstOrDefault(),null,null,
                        null, "Mandatory " +
                        "channel recreation on initialization.");
                }

                
            }
            //if(!IsVoiceChannelsOrdered(client,config))
            //{
            //    await OrderVoiceChannels(client, config);
            //}
        }

        private static async Task OrderVoiceChannels(DiscordClient client, JsonConfig config)
        {
            orderingInProgress = true;
            List<DiscordChannel> list = client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Type == ChannelType.Voice).ToList();
            List<string> mandatoryChannels = config.mandatoryVoiceChannels.Split(';').ToList<string>();

            for (int i = 0; i < mandatoryChannels.Count; i++)
            {
                await list.Where(x => x.Name == mandatoryChannels[i]).FirstOrDefault().ModifyPositionAsync(i);
            }
            orderingInProgress = false;

            //await client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name == "Chit-Chat").FirstOrDefault().ModifyPositionAsync(0);
            //await client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name == "PUBG").FirstOrDefault().ModifyPositionAsync(1);
            //await client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name == "Overwatch").FirstOrDefault().ModifyPositionAsync(2);
            //await client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name == "GTA V").FirstOrDefault().ModifyPositionAsync(3);
            //await client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name == "CS:GO").FirstOrDefault().ModifyPositionAsync(4);
        }

        private static bool IsVoiceChannelsOrdered(DiscordClient client, JsonConfig config)
        {
            bool retVal = true;
            List<DiscordChannel> list = client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Type == ChannelType.Voice).ToList();
            list = list.OrderBy(x => x.Position).ToList();
            List<string> mandatoryChannels = config.mandatoryVoiceChannels.Split(';').ToList<string>();

            for (int i = 0; i < mandatoryChannels.Count; i++)
            {
                if (mandatoryChannels[i] != list[i].Name)
                    return false;
            }

            return retVal;
        }

    }
}
