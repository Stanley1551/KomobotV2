using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Twitch.Responses;

namespace Twitch
{
    public interface ITwitchService
    {
        //Task ConfigLiveMonitorAsync(string[] channelsToMonitor);
        Task<GetStreamOnlineResponse> GetStreamOnline(string channel);
    }
}
