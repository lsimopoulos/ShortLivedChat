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
        private readonly GroupsManager _groupsManager;

        public ChatHub(GroupsManager groupsManager)
        {
            _groupsManager = groupsManager;
        }

        public async Task  SendFromServer(string message)
        {
           await  Clients.Group(_groupsManager.GetNewRoom().roomName).SendAsync(nameof(SendFromServer), $"{message}");
        }

        /// <inheritdoc />
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Send($"{GetUserName()} left the chat");

            await base.OnDisconnectedAsync(exception);
        }

        public Task Send(string message)
        {
            return Clients.Group(_groupsManager.GetNewRoom().roomName).SendAsync("Send", $"{message}");
        }

        public Task SendFromClient(string message)
        {
            return Send($"{GetUserName()}: {message}");
        }

        /// <inheritdoc />
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Welcome",
                $"Welcome to ShortLiveChat. Available rooms to join: 1. {_groupsManager.GetNewRoom().roomName}");
           
            await base.OnConnectedAsync();

        }

        public async Task JoinToGroup(string groupName)
        {
            _groupsManager.AddUserToGroup(Context.ConnectionId, _groupsManager.GetNewRoom().roomName);
            await Groups.AddToGroupAsync(Context.ConnectionId, _groupsManager.GetNewRoom().roomName);
            
            //Sends message to already connected clients
            await SendFromServer($"{GetUserName()} joined the chat room {_groupsManager.GetNewRoom().roomName}");
        }

        

        //TODO:find a better way to get the user's name.
        private string GetUserName()
        {
            var user = ISConfig.GetUsers().FirstOrDefault(x => x.SubjectId == Context.User.GetDisplayName());
            return user.Username;
        }
    }
}