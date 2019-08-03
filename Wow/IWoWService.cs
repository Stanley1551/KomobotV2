using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wow.Responses;

namespace Wow
{
    public interface IWoWService
    {
        Task<CharInfoResponse> GetCharInfo();

        string Realm { get; set; }
        string CharName { get; set; }
        string CharInfoEndpoint { get; set; }
        string client_secret { get; set; }
        string client_id { get; set; }
        string OauthCheckTokenEndpoint { get; set; }
        string OauthAccessTokenEndpoint { get; set; }
    }
}
