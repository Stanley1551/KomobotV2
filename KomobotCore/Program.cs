using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KomobotV2.DataAccess;
using KomobotV2.Logger;
using DSharpPlus.Net.WebSocket;
using KomobotV2;
using TwitchLib.Api.Services;
using TwitchLib.Api;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace KomobotCore
{
    class Program
    {
        static CommandsNextModule commands;
        static private object thisLock = new object();
        public static TwitchAPI API = new TwitchAPI();
        private static LiveStreamMonitorService Monitor = new LiveStreamMonitorService(API);
        public static JsonConfig config;
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            KomoLogger logger = new KomoLogger();

            try
            {
                config = new JsonParser().Config;
            }
            catch (Exception e) { logger.Fatal("Loading configuration", e); Console.ReadKey(); return; }

            DiscordClient client = new DiscordClient(new DiscordConfiguration()
            {
                TokenType = TokenType.Bot,
                Token = config.DiscordAPIKey,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
            });

            WireUpEvents(client);

            await ConfigLiveMonitorAsync(client);


            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!",
                EnableDms = true,
                CaseSensitive = false,
                EnableDefaultHelp = true,
            });
            commands.RegisterCommands<Commands>();

            logger.Debug("Connecting...");
            await client.ConnectAsync();
            logger.Debug("Connected, waiting for client initialization.");
            await Task.Delay(5000);
            logger.Debug("Starting initialization.");
            await Initialize(client, config);
            logger.Debug("Initialized");
            await Task.Delay(-1);
        }

        private async static Task ConfigLiveMonitorAsync(DiscordClient client)
        {

            API.Settings.ClientId = config.twitchClientID;
            API.Settings.AccessToken = config.twitchAccessToken;

            //EITHER
            //var users = await API.V5.Users.GetUserByNameAsync("stanleyhun15");
            //var userId = users.Matches[0].Id;

            //List<string> lst = new List<string> { userId };

            //OR
            List<string> TwitchChannelIDs = new List<string>();

            var channelsToMonitor = config.twitchChannelsToMonitor.Split(';').ToList();
            if(channelsToMonitor != null && channelsToMonitor.Count >= 1)
            {
                var tasks = new List<Task<string>>();
                //This is some next level async shit
                channelsToMonitor.ForEach((x) => tasks.Add(GetTwitchIDAsync(API, x)));
                var results = await Task.WhenAll(tasks);
                
                foreach(string userId in results)
                {
                    if (userId != null && !string.IsNullOrEmpty(userId))
                    {
                        TwitchChannelIDs.Add(userId);
                    }
                }
            }

            //úgyse lesz benne semmi ha valami nem jó
            if(TwitchChannelIDs.Count >= 1)
            {
                Monitor.SetChannelsById(TwitchChannelIDs);
            }

            Monitor.OnStreamOnline += async (sender, e) => await DiscordEventHandler.Monitor_OnStreamOnline(sender, e, client, API);
            //Monitor.OnStreamOffline += Monitor_OnStreamOffline;
            //Monitor.OnStreamUpdate += Monitor_OnStreamUpdate;
            

            Monitor.Start(); //Keep at the end!

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

            foreach (DiscordChannel channel in currentVoiceChannels)
            {
                currentVoiceChannelNames.Add(channel.Name);
            }

            foreach (string channelName in mandatoryChannels)
            {
                if (!currentVoiceChannelNames.Contains(channelName))
                {
                    missingChannels.Add(channelName);
                }
            }
            if (missingChannels != null && missingChannels.Count > 0)
            {
                foreach (string channelName in missingChannels)
                {
                    await client.Guilds.FirstOrDefault().Value.CreateChannelAsync(channelName, ChannelType.Voice,
                        client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name.ToLower() == "voice channels").FirstOrDefault(), null, null,
                        null, "Mandatory " +
                        "channel recreation on initialization.");
                }
            }
        }

        private static async Task<string> GetTwitchIDAsync(TwitchAPI api, string username)
        {
            var user = await api.V5.Users.GetUserByNameAsync(username);

            return user.Matches[0].Id;
        }

        private static void WireUpEvents(DiscordClient client)
        {
            client.PresenceUpdated += new AsyncEventHandler<PresenceUpdateEventArgs>(async (e) => await DiscordEventHandler.PresenceUpdated(e, client));

            client.GuildMemberRemoved += new AsyncEventHandler<GuildMemberRemoveEventArgs>(async (e) => await DiscordEventHandler.GuildMemberRemoved(e, client));

            client.ChannelCreated += new AsyncEventHandler<ChannelCreateEventArgs>(async (e) => await DiscordEventHandler.ChannelCreated(e, client));

            client.GuildEmojisUpdated += new AsyncEventHandler<GuildEmojisUpdateEventArgs>(async (e) => await DiscordEventHandler.EmojiUpdated(e, client));

            client.ChannelDeleted += new AsyncEventHandler<ChannelDeleteEventArgs>(async (e) => await DiscordEventHandler.ChannelDeleted(e, client, config));

            client.SocketClosed += async (e) => await DiscordEventHandler.SocketClosed(e);

            Monitor.OnChannelsSet += (sender, e) => DiscordEventHandler.Monitor_OnChannelsSet(sender, e);

        }

    }
}
