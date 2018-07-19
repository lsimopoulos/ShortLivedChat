using System;
using System.Threading.Tasks;
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
        /// Get acccess token.
        /// </summary>
        /// <param name="baseUrl">the base url</param>
        /// <param name="userName">the username</param>
        /// <param name="password">the password</param>
        public static async Task<string> GetAccessToken(string baseUrl,string userName,string password)
        {
            //TODO replace all hardcoded strings
            var tokenClient = new TokenClient(baseUrl + TokenEndPoint, "chat_console_client", "superdupersecret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(userName, password, "shortlivedchat");

            if (!tokenResponse.IsError) return tokenResponse.AccessToken;
            Console.WriteLine(tokenResponse.Error);
            return null;
        }
    }
}
