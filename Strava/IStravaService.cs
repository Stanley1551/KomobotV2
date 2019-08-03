using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Strava
{
    public interface IStravaService
    {
        Task<string> StravaInfo(int id, string token, string clientid, string refreshToken);
    }
}
