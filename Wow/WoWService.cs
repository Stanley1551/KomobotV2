﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wow.Responses;

namespace Wow
{
    public class WoWService : IWoWService
    {
        public string CharInfoEndpoint { get; set; }
        public string Client_secret { get; set; }
        public string Client_id { get; set; }
        public string OauthCheckTokenEndpoint { get; set; }
        public string OauthAccessTokenEndpoint { get; set; }
        private string Token { get; set; }

        public WoWService(string blizzardCharInfoEndpoint, string blizzardOauthAccessTokenEndpoint, string blizzardOauthCheckTokenEndpoint, string client_id, string client_secret)
        {
            CharInfoEndpoint = blizzardCharInfoEndpoint;
            OauthAccessTokenEndpoint = blizzardOauthAccessTokenEndpoint;
            OauthCheckTokenEndpoint = OauthCheckTokenEndpoint;
            Client_id = client_id;
            Client_secret = client_secret;
        }

        public async Task<CharInfoResponse> GetCharInfo(string realm, string charname)
        {
            var client = await ConstructBlizzardCharClient(realm, charname);

            var resp = await client.ExecuteTaskAsync(new RestRequest());
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<CharInfoResponse>(resp.Content);
            }
            else throw new Exception("Error during CharInfoRequest...");
        }

        public async Task<CharInfoResponse> GetCharInfo(string username)
        {
            string charname;
            string realm;

            using (KomoBase.KomoBaseAccess kba = new KomoBase.KomoBaseAccess())
            {
                charname = kba.GetWoWCharName(username);
                realm = kba.GetWoWRealmName(username);
            }

            if (!String.IsNullOrEmpty(charname) && !String.IsNullOrEmpty(realm))
            {
                return await GetCharInfo(realm, charname);
            }
            else throw new ArgumentException("Charname and/or realm name is not present in the database.");
        }

        private async Task<bool> ValidateToken()
        {
            string resultString = string.Empty;

            RestClient client = new RestClient(OauthCheckTokenEndpoint);
            RestRequest request = new RestRequest(OauthCheckTokenEndpoint, Method.GET, DataFormat.Json);
            //ez parameter is a faszért van elírva a dokumentációban
            request.AddParameter("token", Token);
            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        private async Task<string> GetAuthTokenFromBlizzard()
        {
            var url = OauthAccessTokenEndpoint;

            var client = new RestClient(url);
            client.AddDefaultParameter("grant_type", "client_credentials");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", Client_id);
            request.AddParameter("client_secret", Client_secret);

            var respone = await client.ExecuteTaskAsync(request);

            return JsonConvert.DeserializeObject<AccessTokenResponse>(respone.Content).access_token;
        }

        private async Task<string> RetrieveAuthToken()
        {
            using (KomoBase.KomoBaseAccess kba = new KomoBase.KomoBaseAccess())
            {
                var tokenFromDB = kba.GetAuthToken();

                if (String.IsNullOrEmpty(tokenFromDB))
                {
                    string token = await GetAuthTokenFromBlizzard();
                    kba.SetAuthToken(token);
                    return token;
                }

                if (await ValidateToken() != true)
                {
                    string token = await GetAuthTokenFromBlizzard();
                    kba.SetAuthToken(token);
                    return token;
                }

                return tokenFromDB;
            }
        }

        private async Task<RestClient> ConstructBlizzardCharClient(string realm, string charname)
        {
            Token = await RetrieveAuthToken();

            string url = CharInfoEndpoint + @"/" + realm + @"/" + charname;
            RestClient client = new RestClient(url);
            client.AddDefaultParameter(new Parameter("locale", "en_US", ParameterType.QueryString));
            client.AddDefaultParameter(new Parameter("access_token", Token, ParameterType.QueryString));

            return client;
        }

        
    }
}