using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wow.Responses;

namespace Wow
{
    public interface IWoWService
    {
        Task<CharInfoResponse> GetCharInfo(string realm, string charname);
        Task<CharInfoResponse> GetCharInfo(string username);

        string CharInfoEndpoint { get; set; }
        string Client_secret { get; set; }
        string Client_id { get; set; }
        string OauthCheckTokenEndpoint { get; set; }
        string OauthAccessTokenEndpoint { get; set; }
    }
}
