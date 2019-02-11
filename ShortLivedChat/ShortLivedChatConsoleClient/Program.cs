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
            var isRegistration = false;
            var hasClientPromptedForJoin = false;
            var hasNotJoinedYet = true;
            //Get token
            Console.WriteLine("Do you want to Log in or Register? Press R or L?");
           
            var input =  Console.ReadLine();
            if (input.Equals("R"))
            {
                isRegistration = true;
            }
            Console.WriteLine("Please enter the username");
            var userName = Console.ReadLine();
            Console.WriteLine("Please enter the password");
            var password = Console.ReadLine();

            if (isRegistration)
            {
                var rrm = new RegistrationRequestModel {UserName = userName, Password = password};
                var result = await RegistrationHelper.RegisterUser(BaseUrl, rrm);
                if (!result)
                {
                    Console.WriteLine("The registration failed.  Please try again");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            var token = TokenHelper.GetAccessToken(BaseUrl,userName,password);
            if (token.Result == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Token could not be obtained by the server : {BaseUrl}");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Login was successful");
            //connect to chat hub
            _chatConnection = await ChatClient.ConnectToChat(token, BaseUrl);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Connected to the chat server");


            _chatConnection.On<string>("Welcome",  message =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{message}");
                Console.WriteLine();
                Console.WriteLine("Please enter the number of room you want to join");
                hasClientPromptedForJoin = true;
            });

         

            _chatConnection.On("Close", async () =>
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The chat channel is closed.Press any key to exit the app.");
                Console.ReadKey();
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

           

            while (true)
            {
                Console.ResetColor();
                if (hasClientPromptedForJoin)
                {
                    await _chatConnection.SendAsync("JoinToGroup",Console.ReadLine());
                    hasClientPromptedForJoin = hasNotJoinedYet = false;
                    
                }
                else if(!hasClientPromptedForJoin && !hasNotJoinedYet)
                     await _chatConnection.SendAsync("SendFromClient", Console.ReadLine());
            }

           
        }
    }
}