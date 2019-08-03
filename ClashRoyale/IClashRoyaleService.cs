using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyale
{
    public interface IClashRoyaleService
    {
        Task<int> GetTrophies(string tag, string endpoint, string key);
        Task<int> GetWins(string tag, string endpoint, string key);
        Task<string> GetInfo(string tag, string endpoint, string key);
    }
}
