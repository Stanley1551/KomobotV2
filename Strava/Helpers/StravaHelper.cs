using StravaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Strava.Helpers
{
    internal class StravaHelper
    {
        internal static string ConstructResponseMessage(Athlete result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(result.FirstName + " " + result.LastName);
            sb.AppendLine(result.Country + ", " + result.City);
            sb.AppendLine(result.CreatedAt.Year+"."+result.CreatedAt.Month+"."+result.CreatedAt.Day + " óta felhasználó");
            sb.AppendLine(result.FollowerCount + " követő");

            return sb.ToString();
        }
    }
}
