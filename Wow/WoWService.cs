using Newtonsoft.Json;
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
        public string Realm { get; set; }
        public string CharName { get; set; }
        public string CharInfoEndpoint { get; set; }
        public string client_secret { get; set; }
        public string client_id { get; set; }
        public string OauthCheckTokenEndpoint { get; set; }
        public string OauthAccessTokenEndpoint { get; set; }
        private string Token { get; set; }

        public async Task<CharInfoResponse> GetCharInfo()
        {
            var client = await ConstructBlizzardCharClient();

            var resp = await client.ExecuteTaskAsync(new RestRequest());
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<CharInfoResponse>(resp.Content);
            }
            else throw new Exception("Error during CharInfoRequest...");
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
            request.AddParameter("client_id", client_id);
            request.AddParameter("client_secret", client_secret);

            var respone = await client.ExecuteTaskAsync(request);

            return JsonConvert.DeserializeObject<AccessTokenResponse>(respone.Content).access_token;
        }

        private async Task<string> RetrieveAuthToken()
        {
            using (KomoBase.KomoBaseAccess kba = new KomoBase.KomoBaseAccess())
            {
                var tokenFromDB = kba.GetAuthToken();

                if (tokenFromDB == string.Empty)
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

        private async Task<RestClient> ConstructBlizzardCharClient()
        {
            Token = await RetrieveAuthToken();

            string url = CharInfoEndpoint + @"/" + Realm + @"/" + CharName;
            RestClient client = new RestClient(url);
            client.AddDefaultParameter(new Parameter("locale", "en_US", ParameterType.QueryString));
            client.AddDefaultParameter(new Parameter("access_token", Token, ParameterType.QueryString));

            return client;
        }
    }
}
