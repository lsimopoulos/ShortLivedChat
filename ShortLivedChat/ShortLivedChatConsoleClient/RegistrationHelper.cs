using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShortLivedChatConsoleClient
{
    /// <summary>
    /// Class that uses httpclient in order to register new user.
    /// </summary>
    public static class RegistrationHelper
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task<bool> RegisterUser(string baseUrl, RegistrationRequestModel registrationRequestModel)
        {
            HttpClient.BaseAddress = new Uri(baseUrl);
            var content = new StringContent(JsonConvert.SerializeObject(registrationRequestModel), Encoding.UTF8,
                "application/json");
            var resp = await HttpClient.PostAsync("api/Users", content);
            return resp.StatusCode == HttpStatusCode.OK;
        }
    }
}
