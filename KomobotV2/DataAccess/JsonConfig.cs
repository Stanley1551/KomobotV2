﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomobotV2.DataAccess
{
    public class JsonConfig
    {
        public JsonConfig()
        {
            DiscordAPIKey = string.Empty;
            tutorialdoc = string.Empty;
            fixerAPIKey = string.Empty;
            fixerPrefix = string.Empty;
            jokesAPIKey = string.Empty;
            jokesPrefix = string.Empty;
            mandatoryVoiceChannels = string.Empty;
            crAPIKEY = string.Empty;
            crEndpoint = string.Empty;
        }
        public string DiscordAPIKey { get; set; }
        public string tutorialdoc { get; set; }
        public string fixerAPIKey { get; set; }
        public string fixerPrefix { get; set; }
        public string jokesAPIKey { get; set; }
        public string jokesPrefix { get; set; }
        public string mandatoryVoiceChannels { get; set; }
        public string crEndpoint { get; set; }
        public string crAPIKEY { get; set; }
    }
}