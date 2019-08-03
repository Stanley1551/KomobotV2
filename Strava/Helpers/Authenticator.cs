using RestSharp.Portable;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Strava.Helpers
{
    internal class Authenticator : IAuthenticator
    {
        public string AccessToken { get; set; }

        public Authenticator(string accessToken)
        {
            AccessToken = accessToken;
        }

        public bool CanHandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response)
        {
            return false;
        }

        public bool CanPreAuthenticate(IRestClient client, IRestRequest request, ICredentials credentials)
        {
            return true;
        }

        public bool CanPreAuthenticate(IHttpClient client, IHttpRequestMessage request, ICredentials credentials)
        {
            return false;
        }

        public Task HandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response)
        {
            throw new NotImplementedException();
        }

        public Task PreAuthenticate(IRestClient client, IRestRequest request, ICredentials credentials)
        {
            

            return Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(AccessToken))
                    request.AddHeader("Authorization", "Bearer " + AccessToken);
            });
        }

        public Task PreAuthenticate(IHttpClient client, IHttpRequestMessage request, ICredentials credentials)
        {
            throw new NotImplementedException();
        }
    }
}
