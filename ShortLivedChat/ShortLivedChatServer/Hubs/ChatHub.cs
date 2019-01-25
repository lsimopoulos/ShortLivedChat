using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShortLivedChatServer.Classes;
using ShortLivedChatServer.IdentityServerConfig;

namespace ShortLivedChatServer.Hubs
{
    //TODO refactor hub
    /// <summary>
    /// Chat hub.
    /// </summary>
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly TimerHelper _timerHelper;

        public ChatHub(TimerHelper timerHelper)
        {
            _timerHelper = timerHelper;
        }

        public void SendFromServer(string message)
        {
            Clients.All.SendAsync(nameof(SendFromServer), $"{message}");
        }

        /// <inheritdoc />
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Send($"{GetUserName()} left the chat");

            return base.OnDisconnectedAsync(exception);
        }

        public Task Send(string message)
        {
            return Clients.All.SendAsync("Send", $"{message}");
        }

        public Task SendFromClient(string message)
        {
            return Send($"{GetUserName()}: {message}");
        }

        /// <inheritdoc />
        public override Task OnConnectedAsync()
        {
            //temporary solution. will be replaced later
            if (!_timerHelper.FirstUserLoggedIn)
            {
                _timerHelper.FirstUserLoggedIn = true;
                _timerHelper.CreateTimer();
            }

            Send($"{GetUserName()} joined the chat");
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