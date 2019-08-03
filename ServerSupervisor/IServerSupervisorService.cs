using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerSupervisor
{
    public interface IServerSupervisorService
    {
        Task<ulong> KickUserByUsername(string username);
        Task<ulong> KickUserByID(ulong id);
        Task<ulong> BanUserByUserName(string username);
        Task<ulong> BanUserByID(ulong id);
    }
}
