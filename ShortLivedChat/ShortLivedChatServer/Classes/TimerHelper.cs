using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using ShortLivedChatServer.Hubs;

namespace ShortLivedChatServer.Classes
{
    //the class is going to be refactored after I implement the logic for chat rooms.
    public class TimerHelper
    {
        private readonly IHubContext<ChatHub> _hubContext;
        //temporary property
        public bool FirstUserLoggedIn { get; set; }

        public TimerHelper(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public void CreateTimer()
        {
            var timer = new Timer(TimerCallback,null, 60000, 0);
        }

        private void TimerCallback(object state)
        {
            _hubContext.Clients.All.SendAsync("Close");
        }
    }
}
