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
            footballDataEndpoint = string.Empty;
            footballDataKey = string.Empty;
            blizzardOauthAccessTokenEndpoint = string.Empty;
            blizzardOauthCheckTokenEndpoint = string.Empty;
            client_id = string.Empty;
            client_secret = string.Empty;
            blizzardCharInfoEndpoint = string.Empty;
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
        public string footballDataEndpoint { get; set; }
        public string footballDataKey { get; set; }
        public string blizzardOauthAccessTokenEndpoint { get; set; }
        public string blizzardOauthCheckTokenEndpoint { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string blizzardCharInfoEndpoint { get; set; }
    }
}
