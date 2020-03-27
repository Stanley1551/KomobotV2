using System;
using System.Collections.Generic;
using System.Text;

namespace Corona.ResponseModels
{
    public class CountryInfo
    {
        public int _id { get; set; }
        public int lat { get; set; }
        public int @long { get; set; }
        public string flag { get; set; }
        public string iso3 { get; set; }
        public string iso2 { get; set; }
    }

    public class CountryCasesResponse
    {
        public string country { get; set; }
        public CountryInfo countryInfo { get; set; }
        public int cases { get; set; }
        public int todayCases { get; set; }
        public int deaths { get; set; }
        public int todayDeaths { get; set; }
        public int recovered { get; set; }
        public int active { get; set; }
        public int critical { get; set; }
        public int casesPerOneMillion { get; set; }
        public int deathsPerOneMillion { get; set; }
    }
}
