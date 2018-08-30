using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.VoiceNext;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KomobotV2.DataAccess;
using KomobotV2.Logger;

namespace KomobotV2
{
    class Program
    {
        static VoiceNextClient voiceClient;
        static CommandsNextModule commands;
        static private object thisLock = new object();
        public static Dictionary<DiscordMember, DateTime> gameStartedDictionary;
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
            catch (Exception e) { logger.Fatal("Loading configuration", e); }

            gameStartedDictionary = new Dictionary<DiscordMember, DateTime>();
            
            DiscordClient client = new DiscordClient(new DiscordConfiguration()
            {
                TokenType = TokenType.Bot,
                Token = config.DiscordAPIKey,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
            });

            WireUpEvents(client);

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
        }

        private static void WireUpEvents(DiscordClient client)
        {
            client.PresenceUpdated += new AsyncEventHandler<PresenceUpdateEventArgs>(async (e) => await DiscordEventHandler.PresenceUpdated(e, client, gameStartedDictionary));

            client.GuildMemberRemoved += new AsyncEventHandler<GuildMemberRemoveEventArgs>(async (e) => await DiscordEventHandler.GuildMemberRemoved(e, client));

            client.ChannelCreated += new AsyncEventHandler<ChannelCreateEventArgs>(async (e) => await DiscordEventHandler.ChannelCreated(e, client));

            client.GuildEmojisUpdated += new AsyncEventHandler<GuildEmojisUpdateEventArgs>(async (e) => await DiscordEventHandler.EmojiUpdated(e, client));

            client.ChannelDeleted += new AsyncEventHandler<ChannelDeleteEventArgs>(async (e) => await DiscordEventHandler.ChannelDeleted(e, client, config));
        }

    }
}
