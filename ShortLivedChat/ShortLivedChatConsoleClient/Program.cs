using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ShortLivedChatConsoleClient
{
    internal static class Program
    {
        //hardcoded port and baseurl
        private const string ServerPort = "5001";
        private static readonly string BaseUrl = $"https://localhost:{ServerPort}/";

        private static HubConnection _chatConnection;
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            //Get token
            //TODO : prompt the user to enter username and password in console 
            var token = TokenHelper.GetAccessToken(BaseUrl,"birbilis","pass");
            if (token.Result == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Token could not be obtained by the server : {BaseUrl}");
                Console.ReadKey();
                return;
            }

            //conect to chat hub
            _chatConnection = await ChatClient.ConnectToChat(token, BaseUrl);

            
            _chatConnection.On("Close", async () =>
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The chat channel closed.");
                await _chatConnection.StopAsync();
                Environment.Exit(-1);
            });


            _chatConnection.On<string>("SendFromServer", message =>
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"{message}");
            });

            _chatConnection.On<string>("Send", message =>
            {
                Console.Beep();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{message}");
            });



            while (true) await _chatConnection.SendAsync("SendFromClient", Console.ReadLine());
        }
    }
}