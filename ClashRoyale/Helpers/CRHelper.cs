using ClashRoyale.Results;
using System;
using System.Text;

namespace ClashRoyale.Helpers
{
    internal static class CRHelper
    {
        internal static string ConstructInfoString(PlayerResult result)
        {
            StringBuilder builder = new StringBuilder();
            builder = builder.AppendLine("Tag: " + result.tag);
            builder = builder.AppendLine("Név: " + result.name);
            builder = builder.AppendLine("Clan: " + result.clan.name);
            builder = builder.AppendLine("Trófeák: " + result.trophies);
            builder = builder.AppendLine("Winek: " + result.wins);
            builder = builder.AppendLine("Szint: " + result.expLevel);

            return builder.ToString();
        }

        internal static string URLEncodeCRTag(string tag)
        {
            return tag.Replace("#", "%23");
        }
    }
}
