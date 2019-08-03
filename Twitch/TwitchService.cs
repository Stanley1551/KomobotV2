using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch.Helpers;
using Twitch.Responses;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams;
using TwitchLib.Api.Services;

namespace Twitch
{
    public class TwitchService : ITwitchService
    {
        private static TwitchAPI API = new TwitchAPI();
        public static LiveStreamMonitorService Monitor = new LiveStreamMonitorService(API);
        private List<string> ChannelsToMonitor { get; set; }

        public TwitchService(string clientid, string accesstoken, string channelsToMonitor)
        {
            ChannelsToMonitor = channelsToMonitor.Split(";").ToList();
            API.Settings.ClientId = clientid;
            API.Settings.AccessToken = accesstoken;
        }

        //public async Task ConfigLiveMonitorAsync(string [] channelsToMonitor)
        //{
        //    Monitor.OnChannelsSet += (sender, e) => TwitchHelper.Monitor_OnChannelsSet(sender, e);

        //    //EITHER
        //    //var users = await API.V5.Users.GetUserByNameAsync("stanleyhun15");
        //    //var userId = users.Matches[0].Id;

        //    //List<string> lst = new List<string> { userId };

        //    //OR
        //    List<string> TwitchChannelIDs = new List<string>();

        //    var channelsToMonitorList = channelsToMonitor.ToList();
        //    if (channelsToMonitor != null && channelsToMonitorList.Count >= 1)
        //    {
        //        var tasks = new List<Task<string>>();
        //        //This is some next level async shit
        //        channelsToMonitorList.ForEach((x) => tasks.Add(GetTwitchIDAsync(API, x)));
        //        var results = await Task.WhenAll(tasks);

        //        foreach (string userId in results)
        //        {
        //            if (userId != null && !string.IsNullOrEmpty(userId))
        //            {
        //                TwitchChannelIDs.Add(userId);
        //            }
        //        }
        //    }

        //    //úgyse lesz benne semmi ha valami nem jó
        //    if (TwitchChannelIDs.Count >= 1)
        //    {
        //        Monitor.SetChannelsById(TwitchChannelIDs);
        //    }

        //    //Monitor.OnStreamOnline += async (sender, e) => await DiscordEventHandler.Monitor_OnStreamOnline(sender, e, client, API);
        //    //Monitor.OnStreamOffline += Monitor_OnStreamOffline;
        //    //Monitor.OnStreamUpdate += Monitor_OnStreamUpdate;


        //    Monitor.Start(); //Keep at the end!

        //}

        public async Task<GetStreamOnlineResponse> GetStreamOnline(string channel)
        {
            GetStreamsResponse getStreamResponse = new GetStreamsResponse();

            try
            {
                getStreamResponse = await RetrieveTwitchChannelInfoAsync(channel);
            }
            catch(Exception) { return new GetStreamOnlineResponse() { Online = false, ViewerCount = 0, TimeLapsedString = string.Empty }; }

            if (getStreamResponse.Streams != null && getStreamResponse.Streams.Count() > 0)
            {
                return new GetStreamOnlineResponse()
                {
                    Online = true,
                    TimeLapsedString = TwitchHelper.GetTimeLapsedFormattedString(getStreamResponse.Streams[0].StartedAt),
                    ViewerCount = getStreamResponse.Streams[0].ViewerCount
                };
            }
            else return new GetStreamOnlineResponse() { Online = false, ViewerCount = 0, TimeLapsedString = string.Empty };
        }

        private async Task<string> GetTwitchIDAsync(TwitchAPI api, string username)
        {
            var user = await api.V5.Users.GetUserByNameAsync(username);

            return user.Matches[0].Id;
        }

        private async Task<GetStreamsResponse> RetrieveTwitchChannelInfoAsync(string channel)
        {
            //Előbb kell ID, mert csak azzal megy a hivás...
            var response = await API.V5.Users.GetUserByNameAsync(channel);
            if (response.Total == 0)
            {
                throw new Exception("The GetUserByNameAsync returned with no data.");
            }

            var userid = response.Matches[0].Id;

            return await API.Helix.Streams.GetStreamsAsync(null, null, 1, null, null, "all", new List<string>() { userid }, null);
        }
    }
}
