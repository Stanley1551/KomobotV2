using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.Responses
{
    public class FixerLatestResult
    {
        public FixerLatestResult()
        {

        }

        public FixerLatestResult(string json)
        {
            JToken token = JToken.Parse(json);
            this.success = (bool)token["success"];
            this.timestamp = (int)token["timestamp"];
            this.@base = (string)token["base"];
            this.date = (string)token["date"];
            this.HUF = (double)token["rates"]["HUF"];
            this.GBP = (double)token["rates"]["GBP"];
            this.USD = (double)token["rates"]["USD"];
        }

        #region Properties
        public bool success { get; set; }
        public int timestamp { get; set; }
        public string @base {get; set;}
        public string date { get; set; }
        public double HUF { get; set; }
        public double GBP { get; set; }
        public double USD { get; set; }
    }
#endregion
}
