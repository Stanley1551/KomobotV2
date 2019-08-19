using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KomobotV2.Logger;
using KomobotV2;
using Unity;
using Currency;
using System.Net;
using QrCodeCreation;
using Football;
using Strava;
using KomoBase;
using ClashRoyale;
using Wow;
using KomobotCore.DataAccess;
using Twitch;
using Unity.Injection;

namespace KomobotCore
{
    class Program
    {
        static CommandsNextModule commands;
        
        public static JsonConfig config;
        public static UnityContainer Container = new UnityContainer();

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            //magic to prevent OperationNotSupportedException
            var proxy = WebRequest.DefaultWebProxy;
            WebRequest.DefaultWebProxy = null;
            KomoLogger logger = new KomoLogger();

            Container.RegisterType<ICurrencyService, CurrencyService>();
            Container.RegisterType<IQrCodeCreatorService, QrCodeCreatorService>();
            Container.RegisterType<IFootballDataService, FootballDataService>();
            Container.RegisterType<IStravaService, StravaService>();
            Container.RegisterType<IClashRoyaleService, ClashRoyaleService>();

            try
            {
                config = new JsonParser().Config;
            }
            catch (Exception e) { logger.Fatal("Loading configuration", e); Console.ReadKey(); return; }

            Container.RegisterType<ITwitchService, TwitchService>(new InjectionConstructor(config.twitchClientID, config.twitchAccessToken, config.twitchChannelsToMonitor));
            Container.RegisterType<IWoWService, WoWService>(new InjectionConstructor(config.blizzardCharInfoEndpoint, config.blizzardOauthAccessTokenEndpoint, config.blizzardOauthCheckTokenEndpoint,
                config.client_id, config.client_secret));

            DiscordClient client = new DiscordClient(new DiscordConfiguration()
            {
                TokenType = TokenType.Bot,
                Token = config.DiscordAPIKey,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
            });


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

        private static async Task Initialize(DiscordClient client, JsonConfig config)
        {
            //SQLite database
            using (KomoBaseAccess kba = new KomoBaseAccess())
            {
                kba.Initialize();
                kba.SyncUsers(client.Guilds.FirstOrDefault().Value.Members.Select(x => x.Username).ToList());
            }

            var users = client.Guilds.FirstOrDefault().Value.Members.ToList<DiscordMember>();

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

            WireUpEvents(client);


        }

        private static void UpdateNote(DiscordClient client)
        {
            //missing?
        }

        private static void WireUpEvents(DiscordClient client)
        {
            client.PresenceUpdated += new AsyncEventHandler<PresenceUpdateEventArgs>(async (e) => await DiscordEventHandler.PresenceUpdated(e, client));

            client.GuildMemberRemoved += new AsyncEventHandler<GuildMemberRemoveEventArgs>(async (e) => await DiscordEventHandler.GuildMemberRemoved(e, client));

            client.ChannelCreated += new AsyncEventHandler<ChannelCreateEventArgs>(async (e) => await DiscordEventHandler.ChannelCreated(e, client));

            client.GuildEmojisUpdated += new AsyncEventHandler<GuildEmojisUpdateEventArgs>(async (e) => await DiscordEventHandler.EmojiUpdated(e, client));

            client.ChannelDeleted += new AsyncEventHandler<ChannelDeleteEventArgs>(async (e) => await DiscordEventHandler.ChannelDeleted(e, client, config));

            client.SocketClosed += async (e) => await DiscordEventHandler.SocketClosed(e);

            

        }

    }
}
