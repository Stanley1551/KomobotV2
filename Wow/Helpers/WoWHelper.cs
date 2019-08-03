using System;
using System.Collections.Generic;
using System.Text;
using Wow.Responses;

namespace Wow.Helpers
{
    internal class WoWHelper
    {
        internal static DateTime GetDateTimeFromTimeStamp(long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 1, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timestamp);
        }

        internal static string ConstructFeedStringDependingOnType(Feed feed)
        {
            string retVal = GetDateTimeFromTimeStamp(long.Parse(feed.timestamp.ToString())).ToString() + " ";
            switch (feed.type)
            {
                case "BOSSKILL":
                    retVal += "Legyőzte " + feed.achievement.title + "-t (" + feed.quantity + ". alkalommal)";
                    break;
                case "LOOT":
                    retVal += "Lootolta a " + feed.itemId + " itemID-s itemet.";
                    break;
                case "ACHIEVEMENT":
                    retVal += "Megszerezte a " + feed.achievement.title + " achit! (" + feed.achievement.points + " pont)";
                    break;

            }

            return retVal;
        }
    }
}
