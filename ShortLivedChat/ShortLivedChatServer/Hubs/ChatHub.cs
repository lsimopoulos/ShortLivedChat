using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using ShortLivedChatServer.IdentityServerConfig;
using ShortLivedChatServer.Interfaces;

namespace ShortLivedChatServer.Hubs
{
    /// <summary>
    /// Chat hub.
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub, ISender
    {
        private readonly IApplicationLifetime _applicationLifetime;

        public ChatHub(IApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
        }

        public void SendMessageFromServer(string message)
        {
           Clients.All.SendAsync("ServerMessage", $"{message}");
        }

        public void CloseChatChannel()
        {
            Clients.All.SendAsync("Close");
            //restart server
            _applicationLifetime.StopApplication();
            
        }

        /// <inheritdoc />
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.All.SendAsync("Send", $"{GetUserName()} left the chat");

            return base.OnDisconnectedAsync(exception);
        }

        public Task Send(string message)
        {
            return Clients.All.SendAsync("Send", $"{message}");
        }

        public Task SendFromClient(string message)
        {
            return Clients.All.SendAsync("Send", $"{GetUserName()}: {message}");
        }


        /// <inheritdoc />
        public override Task OnConnectedAsync()
        {
            Clients.All.SendAsync("Send", $"{GetUserName()} joined the chat");
            return base.OnConnectedAsync();
        }

        //TODO:find a better way to get the user's name.
        private string GetUserName()
        {
            var user = ISConfig.GetUsers().FirstOrDefault(x => x.SubjectId == Context.User.GetDisplayName());
            return user.Username;
        }
    }
}