using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib.Api.Services.Events;

namespace Twitch.Helpers
{
    internal class TwitchHelper
    {
        internal static void Monitor_OnChannelsSet(object sender, OnChannelsSetArgs e)
        {
            foreach (string channel in e.Channels)
            {
                string msg = "Twitch monitor set to channel: " + channel;
                Console.WriteLine(msg);
            }
        }

        internal static string GetTimeLapsedFormattedString(DateTime startedAt)
        {
            var timeLapsed = DateTime.UtcNow - startedAt.ToUniversalTime();
            return timeLapsed.Hours + " órája, " + timeLapsed.Minutes + " perce";
        }
    }
}
