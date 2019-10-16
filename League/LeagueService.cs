using RiotSharp;
using System;
using System.Threading.Tasks;
using RiotSharp.Endpoints.SummonerEndpoint;

namespace League
{
    public class LeagueService : ILeagueService
    {
        private RiotApi API;

        public LeagueService(string apiKey)
        {
            API = RiotApi.GetDevelopmentInstance(apiKey);
        }

        public async Task<string> GetSummonerInfoMsg(string summonerName)
        {
            Summoner summoner;

            if (string.IsNullOrEmpty(summonerName))
            {
                throw new ArgumentException("A summoner név hibás!");
            }
            try
            {
                summoner = await API.Summoner.GetSummonerByNameAsync(RiotSharp.Misc.Region.Eune, summonerName);
            }
            catch (Exception e) { throw e; }

            if (summoner != null)
            {
                return summoner.Name + ", szintje " + summoner.Level + "!";
            }
            else throw new Exception("Valami hiba történt!");
        }
    }
}
