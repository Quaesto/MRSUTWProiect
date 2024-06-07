using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MRSTWEb.BusinessLogic.Interfaces;

namespace MRSTWEb.BusinessLogic.Services
{
    public class ExternalLoginService : IExternalLoginService
    {
        public JObject GetUserFromGoogleAPI(string code)
        {
            var client = new RestClient("https://www.googleapis.com/oauth2/v4/token");
            var request = new RestRequest("", Method.Post);
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("redirect_uri", "https://localhost:44385/Account/GoogleLoginCallback");

            request.AddParameter("client_id", "800643618878-kp8dt4fo0sc7h0nmt05i4in8rdni36g9.apps.googleusercontent.com");
            request.AddParameter("client_secret", "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");

            RestResponse response = client.Execute(request);
            var content = response.Content;
            var res = (JObject)JsonConvert.DeserializeObject(content);
            var client2 = new RestClient("https://www.googleapis.com/oauth2/v1/userinfo");
            client2.AddDefaultHeader("Authorization", "Bearer" + res["access_token"]);

            request = new RestRequest();

            var response2 = client2.Execute(request);
            var content2 = response2.Content;

            var user = (JObject)JsonConvert.DeserializeObject(content2);

            return user;
        }
    }
}
