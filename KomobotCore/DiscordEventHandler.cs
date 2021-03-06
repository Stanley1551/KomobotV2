﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using DSharpPlus;
using TwitchLib;
using TwitchLib.Api;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using KomobotCore;
using KomobotV2.Logger;
using TwitchLib.Api.Services.Events;
using KomoBase;
using KomobotCore.DataAccess;

namespace KomobotV2
{
    public static class DiscordEventHandler
    {
        private static KomoLogger logger = new KomoLogger();
        private static Dictionary<DiscordMember, DateTime> gameStartedDictionary = new Dictionary<DiscordMember, DateTime>();

        #region Events
        public static async Task ChannelCreated(ChannelCreateEventArgs e, DiscordClient client)
        {
            try
            {
                var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1, null, AuditLogActionType.ChannelCreate);

                if (!log.FirstOrDefault().UserResponsible.IsBot)
                {
                    await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(), "Nocsak! Új " +
                        "kommunikációs csatorna " + e.Channel.Name + " néven!");
                }
            }
            catch(Exception ex) { logger.Error(ex.Message); }
            
        }

        public static async Task PresenceUpdated(PresenceUpdateEventArgs e, DiscordClient client)
        {
            if (!e.Member.IsBot)
            {
                try
                {
                    using (KomoBaseAccess kba = new KomoBaseAccess())
                    {
                        bool subStatus = kba.SubStatus(e.Member.Username);

                        bool isStopped = (e.Game == null || e.Game.Name == null || e.Game.Name == string.Empty) && gameStartedDictionary.ContainsKey(e.Member);
                        int points = 0;
                        string msg = string.Empty;
                        //add points
                        if (isStopped)
                        {
                            msg = GetTimeLapsedString(DateTime.Now, gameStartedDictionary[e.Member], out points);
                            kba.AddPoints(e.Member.Username, points);
                        }
                        //if just started playing
                        if (e.Game != null && e.Game.Name != null && !gameStartedDictionary.ContainsKey(e.Member) && (e.PresenceBefore.Game == null || e.PresenceBefore.Game.Name == string.Empty))
                        {
                            gameStartedDictionary.Add(e.Member, DateTime.Now);
                        }
                        //if ended 
                        else if (isStopped)
                        {
                            gameStartedDictionary.Remove(e.Member);
                        }

                        if (subStatus == true && isStopped)
                        {
                            DiscordDmChannel dm = await client.CreateDmAsync(e.Member);

                            await client.SendMessageAsync(dm, "No! Ennyit függtél most: " +
                                msg,
                                false, null);
                        }
                    }
                }
                catch (Exception ex) { logger.Error(DateTime.UtcNow.ToString() + ": " + ex.Message); }
            }
        }

        public static async Task GuildMemberRemoved(GuildMemberRemoveEventArgs e, DiscordClient client)
        {
            var log = await client.Guilds.FirstOrDefault().Value.GetAuditLogsAsync(1, null);

            //Only bcoz of the kick command
            if(!log.FirstOrDefault().UserResponsible.IsBot)
            {
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

            logger.Debug(user.Username + " deleted the " + e.Channel.Name + " channel!");

            if (mandatoryChannels.Contains(e.Channel.Name))
            {
                await client.Guilds.FirstOrDefault().Value.CreateChannelAsync(e.Channel.Name, e.Channel.Type, e.Channel.Parent,
                    e.Channel.Bitrate, e.Channel.UserLimit, e.Channel.PermissionOverwrites, "Mandatory channel recreation.");

                await client.SendMessageAsync(e.Guild.Channels.Where(x => x.Name == "general").FirstOrDefault(),
                    "A(z) " + e.Channel.Name + " kötött csatorna, nem törölhető! " + user.Username + ", ellenőrző!");
            }
        }

        internal static async Task MessageCreated(MessageCreateEventArgs e, DiscordClient client, string maintainedChannel)
        {
            try
            {
                if (e.Channel.Name != null && e.Channel.Name == maintainedChannel)
                {
                    if (!e.Message.Attachments.Any())
                    {
                        var dmChannel = await client.CreateDmAsync(e.Author);
                        await dmChannel.SendMessageAsync("Hóha! A " + maintainedChannel + " csatornába erősen ajánlott csak képeket küldeni!");
                        await e.Message.DeleteAsync();
                    }
                }
            }catch(Exception ex) { Console.WriteLine(ex.Message); }
            
        }

        public static async Task Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e, DiscordClient client, TwitchAPI API)
        {
            try
            {
                var gameResp = await API.Helix.Games.GetGamesAsync(new List<string> { e.Stream.GameId });
                var userResp = await API.V5.Users.GetUserByIDAsync(e.Stream.UserId);

                var gameName = gameResp.Games[0].Name;
                var userName = userResp.Name;


                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                {
                    Title = userName + " élőben közvetíti: " + gameName + "!",
                    Color = DiscordColor.Azure,
                    Description = e.Stream.Title,
                    ImageUrl = userResp.Logo,
                    Url = new Uri(@"https://www.twitch.tv/"+userName).ToString()
                };

                await client.SendMessageAsync(client.Guilds.FirstOrDefault().Value.Channels.Where(x => x.Name == "general").FirstOrDefault(), null, false,
                    builder.Build());
            }
            catch (Exception ex) { logger.Error("Kivétel! --> " + ex.Message); }
            
        }

        public static async Task SocketClosed(SocketCloseEventArgs e)
        {
            logger.Error(e.Client.CurrentUser.Username + " socket closed: "+ e.CloseMessage);
        }
        #endregion

        #region private methods
        private static string GetTimeLapsedString(DateTime to, DateTime from,out int points)
        {
            StringBuilder sb = new StringBuilder();
            var timelapsed = to - from;
            sb = sb.Append(timelapsed.Hours + "óra " + timelapsed.Minutes + "perc " + timelapsed.Seconds + "másodperc ");
            points = timelapsed.Minutes + timelapsed.Hours*60;

            return sb.ToString();
        }
        #endregion
    }
}
