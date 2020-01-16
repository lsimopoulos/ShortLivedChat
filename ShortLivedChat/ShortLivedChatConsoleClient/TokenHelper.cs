using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;

namespace ShortLivedChatConsoleClient
{
    /// <summary>
    /// Helper class that returns the access token given the credentials are correct.
    /// </summary>
    internal static class TokenHelper
    {
        private const string TokenEndPoint = "connect/token";
        /// <summary>
        /// Get access token.
        /// </summary>
        /// <param name="baseUrl">the base url</param>
        /// <param name="userName">the username</param>
        /// <param name="password">the password</param>
        public static async Task<string> GetAccessToken(string baseUrl,string userName,string password)
        {
            //TODO replace all hardcoded strings
            var tokenOptions = new TokenClientOptions
            {
                Address = baseUrl + TokenEndPoint,
                ClientId = "chat_console_client",
                ClientSecret = "superdupersecret"
            };
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, xCertificate2, chain, errors) => true

            };
            // Get a token.
            var tokenClient = new TokenClient(new HttpMessageInvoker(handler), tokenOptions);
            var tokenResponse = await tokenClient.RequestPasswordTokenAsync(userName, password, "shortlivedchat");

            if (!tokenResponse.IsError) return tokenResponse.AccessToken;
            Console.WriteLine(tokenResponse.Error);
            return null;
        }
    }
}
