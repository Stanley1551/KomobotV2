using Newtonsoft.Json;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using Strava.Helpers;
using StravaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Strava
{
    public class StravaService : IStravaService
    {

        public StravaService()
        {
           
        }

        public async Task<string> StravaInfo(int id, string secret, string clientid, string refreshToken)
        {
            var accessToken = await RefreshToken(secret, clientid, refreshToken);

            Client client = new Client(new Authenticator(accessToken));

            var result = await client.Athletes.Get(id);

            return StravaHelper.ConstructResponseMessage(result);
        }

        private async Task<string> RefreshToken(string secret, string clientid, string refreshToken)
        {
            RestClient client = new RestClient(@"https://www.strava.com");
            RestRequest request = new RestRequest(@"/oauth/token", Method.POST);

            client.AddDefaultParameter("grant_type", "refresh_token");
            request.AddOrUpdateHeader("accept-encoding", "gzip, deflate");
            request.AddOrUpdateHeader("cache-control", "no-cache");
            request.AddOrUpdateParameter("grant_type", "refresh_token");
            request.AddOrUpdateParameter("client_secret", secret);
            request.AddOrUpdateParameter("client_id", clientid);
            request.AddOrUpdateParameter("refresh_token", refreshToken);

            try
            {
                var refreshResponse = await client.Execute (request);
                var definition = new { token_type = "", access_token = "", expires_at = "", expires_in = "", refresh_token = "" };
                var responseJson = JsonConvert.DeserializeAnonymousType(refreshResponse.Content, definition);

                return responseJson.access_token;
            }
            catch (Exception e)
            {
                return string.Empty;
            }

        }
    }
}
