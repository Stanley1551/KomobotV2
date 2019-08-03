using System;
using System.Threading.Tasks;
using Currency.Responses;
using Currency.Core;
using RestSharp;

namespace Currency
{
    public class CurrencyService : ICurrencyService
    {
        public async Task<FixerLatestResult> GetCurrencyResultsAsync(Uri uri)
        {
            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);

            var response = await client.ExecuteGetTaskAsync(request);

            return new FixerLatestResult(response.Content);
        }

        public bool Validate(string currency)
        {
            if(Currencies.ValidCurrencies.Contains(currency))
            {
                return true;
            }
            return false;
        }

    }
}
