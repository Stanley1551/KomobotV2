using System;
using Football.Enums;

namespace Football.Helpers
{
    internal static class FootballDataHelper
    {
        public static string GetLeagueNames()
        {
            var retVal = string.Empty;

            foreach (string name in Enum.GetNames(typeof(FootballLeagues)))
            {
                retVal += name + " ";
            }

            return retVal;
        }

        public static string CreateEndpoint(string url, object id)
        {
            return url + "competitions/" + (int)id + "/standings";
        }
    }
}
