using Corona.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Corona
{
    public interface ICoronaService
    {
        Task<OverallCasesResponse> GetOverallCases();
        Task<CountryCasesResponse> GetCountrySpecificCases(string country);
    }
}
