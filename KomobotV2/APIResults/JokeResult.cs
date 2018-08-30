using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KomobotV2.APIResults
{
    public class JokeResult
    {
        public JokeResult() { }

        public JokeResult(string json)
        {
            JToken token = JToken.Parse(json);
            Type = (string)token["type"];
            Id = (int)token["value"]["id"];
            Joke = (string)token["value"]["joke"];
        }

        public string Type { get; set; }
        public int Id { get; set; }
        public string Joke { get; set; }
    }
}
