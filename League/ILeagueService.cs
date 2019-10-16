using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace League
{
    public interface ILeagueService
    {
        Task<string> GetSummonerInfoMsg(string summonerName);
    }
}
