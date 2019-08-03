using System;
using System.Threading.Tasks;
using Currency.Responses;

namespace Currency
{
    public interface ICurrencyService
    {
        bool Validate(string currency);

        Task<FixerLatestResult> GetCurrencyResultsAsync(Uri uri);

    }
}
