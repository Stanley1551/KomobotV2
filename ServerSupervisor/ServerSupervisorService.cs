using System;
using System.Threading.Tasks;

namespace ServerSupervisor
{
    public class ServerSupervisorService : IServerSupervisorService
    {
        public Task<ulong> BanUserByID(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> BanUserByUserName(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> KickUserByID(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> KickUserByUsername(string username)
        {
            throw new NotImplementedException();
        }
    }
}
