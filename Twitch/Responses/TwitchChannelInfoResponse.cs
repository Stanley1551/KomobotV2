using System;
using System.Collections.Generic;
using System.Text;

namespace Twitch.Responses
{
    public class GetStreamOnlineResponse
    {
        public bool Online { get;  internal set; }
        public string TimeLapsedString { get; internal set; }
        public int ViewerCount { get; internal set; }
    }
}
