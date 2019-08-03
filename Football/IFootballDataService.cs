using Football.Contracts;
using System.Threading.Tasks;

namespace Football
{
    public interface IFootballDataService
    {
        Task<string> GetLeagueStanding(string league, string endpoint, string key);
        bool ValidateLeagueName(string league);
        string GetLeagueNames();
    }
}
