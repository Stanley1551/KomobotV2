using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using DSharpPlus;
using KomobotV2.DataAccess;

namespace KomobotV2
{
    public static class DiscordEventHandler
    {
        #region Events
        public static async Task ChannelCreated(ChannelCreateEventArgs e, DiscordClient client)
        {
            var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1, null, AuditLogActionType.ChannelCreate);

            if (!log.FirstOrDefault().UserResponsible.IsBot)
            {
                await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(), "Nocsak! Új " +
                    "kommunikációs csatorna " + e.Channel.Name + " néven!");
            }
        }

        public static async Task PresenceUpdated(PresenceUpdateEventArgs e, DiscordClient client, Dictionary<DiscordMember, DateTime> gameStartedDictionary)
        {
            if (Komobase.IsSubscribed(e.Member.Username))
            {
                //if just started playing
                if (e.Game != null && e.Game.Name != null && !gameStartedDictionary.ContainsKey(e.Member) && (e.PresenceBefore.Game == null || e.PresenceBefore.Game.Name == string.Empty))
                {
                    gameStartedDictionary.Add(e.Member, DateTime.Now);
                }
                //if ended 
                else if (e.Game == null || e.Game.Name == null || e.Game.Name == string.Empty)
                {
                    if (gameStartedDictionary.ContainsKey(e.Member))
                    {
                        DiscordDmChannel dm = await client.CreateDmAsync(e.Member);

                        await client.SendMessageAsync(dm, "No! Ennyit függtél most: " +
                            GetTimeLapsedString(DateTime.Now, gameStartedDictionary[e.Member]),
                            false, null);

                        gameStartedDictionary.Remove(e.Member);
                    }
                }
            }
        }

        public static async Task GuildMemberRemoved(GuildMemberRemoveEventArgs e, DiscordClient client)
        {
            var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1, null);
            AuditLogActionType actionType = log.FirstOrDefault().ActionType;

            switch (actionType)
            {
                case AuditLogActionType.Ban:
                    {
                        await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(),
                        e.Member.Username + " kapott egy banhammert!", false);
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
        }

        public static async Task EmojiUpdated(GuildEmojisUpdateEventArgs e, DiscordClient client)
        {
            DiscordEmoji added = e.EmojisAfter.Except(e.EmojisBefore).FirstOrDefault();

            await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(), "Hinnye! Új emoji lett létrehozva " + added.CreationTimestamp +
                "-kor " + added.Name + " néven!");
        }

        public static async Task ChannelDeleted(ChannelDeleteEventArgs e, DiscordClient client, JsonConfig config)
        {
            var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1, null, AuditLogActionType.ChannelDelete);

            var mandatoryChannels = config.mandatoryVoiceChannels;

            if (log == null || log.Count < 1)
            {
                throw new ArgumentException("No entry found");
            }

            var user = log.FirstOrDefault().UserResponsible;

            Console.WriteLine(user.Username + " deleted the " + e.Channel.Name + " channel!");

            if (mandatoryChannels.Contains(e.Channel.Name))
            {
                await client.Guilds.FirstOrDefault().Value.CreateChannelAsync(e.Channel.Name, e.Channel.Type, e.Channel.Parent,
                    e.Channel.Bitrate, e.Channel.UserLimit, e.Channel.PermissionOverwrites, "Mandatory channel recreation.");

                await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(),
                    "A(z) " + e.Channel.Name + " kötött csatorna, nem törölhető! " + user.Username + ", ellenőrző!");
            }
        }
        #endregion

        #region private methods
        private static string GetTimeLapsedString(DateTime to, DateTime from)
        {
            StringBuilder sb = new StringBuilder();
            var timelapsed = to - from;
            sb = sb.Append(timelapsed.Hours + "óra " + timelapsed.Minutes + "perc " + timelapsed.Seconds + "másodperc ");

            return sb.ToString();
        }
        #endregion
    }
}
