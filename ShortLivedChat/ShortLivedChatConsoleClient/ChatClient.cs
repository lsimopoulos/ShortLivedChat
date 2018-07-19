using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ShortLivedChatConsoleClient
{
   /// <summary>
   /// Chat client class that connects to chat hub.
   /// </summary>
    internal static class ChatClient
    {
        //hardcoded endpoint
        private const string HubEndPoint = "chat";

        /// <summary>
        /// Connects to the chat hub and return the hubconnection.
        /// </summary>
        /// <param name="accessToken">the access token</param>
        /// <param name="baseUrl">the base url</param>
        public static async Task<HubConnection> ConnectToChat(Task<string> accessToken, string baseUrl)
        {
            //skip the validation for self signed certificate
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            var connection = new HubConnectionBuilder()
                .WithUrl
                (
                    baseUrl + HubEndPoint,
                    options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                        options.AccessTokenProvider = () => accessToken;
                        options.HttpMessageHandlerFactory = h => handler;
                        options.WebSocketConfiguration = wsc =>
                        {
                            wsc.RemoteCertificateValidationCallback =
                                (httpRequestMessage, cert, cetChain, policyErrors) => true;
                        };
                    })
                .AddMessagePackProtocol()
                .Build();

            //connect to the hub
            await connection.StartAsync();
            return connection;
        }
    }
}
