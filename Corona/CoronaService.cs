using Corona.ResponseModels;
using System;
using System.Threading.Tasks;
using RestSharp;

namespace Corona
{
    public class CoronaService : ICoronaService
    {
        public CoronaService(string url)
        {
            Url = url;
        }

        public async Task<OverallCasesResponse> GetOverallCases()
        {
            try
            {
                string uri = Url + PathToAll;
                var client = new RestClient(uri);
                var request = new RestRequest(Method.GET);

                var response = await client.ExecuteGetAsync<OverallCasesResponse>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.Data;

                }

                else throw new Exception("Valami hiba történt a kérés során!");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<CountryCasesResponse> GetCountrySpecificCases(string country)
        {
            try
            {
                string uri = Url + PathToCC + country;
                var client = new RestClient(uri);
                var request = new RestRequest(Method.GET);

                var response = await client.ExecuteGetAsync<CountryCasesResponse>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.Data;

                }

                else throw new Exception("Valami hiba történt a kérés során!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private readonly string Url;
        private const string PathToAll = @"/all";
        private const string PathToCC = @"/countries/";
    }
}
