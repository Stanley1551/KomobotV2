using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Football.Contracts;
using Football.Enums;
using Football.Helpers;
using Newtonsoft.Json;

namespace Football
{
    public class FootballDataService : IFootballDataService
    {
        public FootballDataService()
        {

        }

        public async Task<string> GetLeagueStanding(string league, string endpoint, string key)
        {
            var id = Enum.Parse(typeof(FootballLeagues), league);

            string url = FootballDataHelper.CreateEndpoint(endpoint, id);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers.Set("X-Auth-Token", key);

            var response = (HttpWebResponse)await request.GetResponseAsync();

            var code = response.StatusCode;

            if (HttpStatusCode.OK == code)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string resultString = await reader.ReadToEndAsync();

                var result = JsonConvert.DeserializeObject<FootballDataStandingsResult>(resultString);

                StringBuilder builder = new StringBuilder();
                builder.AppendLine(id.ToString() + " tabella:");

                foreach (var standing in result.standings.FirstOrDefault().table)
                {
                    builder.AppendLine(standing.position + ". " + standing.team.name + "\t " + standing.points + " pont, " + standing.playedGames + " meccs");
                }

                return builder.ToString();
            }
            else
            {
                throw new Exception("A http status nem OK!");
            }
        }

        public bool ValidateLeagueName(string league)
        {
            if (!Enum.GetNames(typeof(FootballLeagues)).Any(x => x.Equals(league)))
            {
                return false;
            }

            return true;
        }

        public string GetLeagueNames()
        {
            return FootballDataHelper.GetLeagueNames();
        }
    }
}
