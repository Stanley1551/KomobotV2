using ClashRoyale.Helpers;
using ClashRoyale.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyale
{
    public class ClashRoyaleService : IClashRoyaleService
    {
        

        public async Task<string> GetInfo(string tag, string endpoint, string key)
        {
            var result = await GetCRPlayerData(tag, endpoint, key);

            return CRHelper.ConstructInfoString(result);
        }

        public async Task<int> GetTrophies(string tag, string endpoint, string key)
        {
            var result = await GetCRPlayerData(tag, endpoint, key);

            return result.trophies;
        }

        public async Task<int> GetWins(string tag, string endpoint, string key)
        {
            var result = await GetCRPlayerData(tag, endpoint, key);

            return result.wins;
        }

        private static async Task<PlayerResult> GetCRPlayerData(string tag, string endpoint, string apikey)
        {
            tag = CRHelper.URLEncodeCRTag(tag);
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(endpoint);
                request.Method = "GET";
                request.Accept = "application/json";
                request.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + apikey);

                var response = (HttpWebResponse)await request.GetResponseAsync();

                var code = response.StatusCode;

                if (HttpStatusCode.OK == code)
                {
                    System.IO.Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);

                    string resultString = await reader.ReadToEndAsync();

                    return JsonConvert.DeserializeObject<PlayerResult>(resultString);
                }
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }

            return new PlayerResult();
        }


    }
}
