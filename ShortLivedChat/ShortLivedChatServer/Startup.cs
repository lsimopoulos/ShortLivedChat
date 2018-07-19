using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ShortLivedChatServer.Hubs;
using ShortLivedChatServer.IdentityServerConfig;
using ShortLivedChatServer.Interfaces;

namespace ShortLivedChatServer
{
    public class Startup
    {
        private static Timer _timer;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR().AddMessagePackProtocol();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(ISConfig.GetApiResources())
                .AddInMemoryClients(ISConfig.GetClients())
                .AddTestUsers(ISConfig.GetUsers())
               .AddSigningCredential(Cert.Get("thecert.pfx", "somepassword"));
            
            services.AddMvcCore()
                .AddAuthorization();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44359/";
                    options.RequireHttpsMetadata = true;
                    options.ApiName = "shortlivedchat";
                });


            services.AddTransient<ISender, ChatHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseIdentityServer();
            app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); });
            var iSender = serviceProvider.GetService<ISender>();
            Task.Factory.StartNew(() =>
            {
                var customTimerObject = new Tuple<ISender, DateTime>(iSender, DateTime.UtcNow);
                _timer = new Timer(TimerCallback, customTimerObject, 0, 1000);
            });
        }

        //TODO: Refactor
        /// <summary>
        /// callback fo the timer.
        /// </summary>
        /// <param name="state"></param>
        private static void TimerCallback(object state)
        {
            var complexObject = state as Tuple<ISender, DateTime>;
            if (DateTime.UtcNow - complexObject.Item2 > TimeSpan.FromSeconds(150) &&
                DateTime.UtcNow - complexObject.Item2 < TimeSpan.FromSeconds(180))
                complexObject.Item1.SendMessageFromServer(
                    $"This chat channel is about to close in {180 - (DateTime.UtcNow - complexObject.Item2).TotalSeconds:##}s");

            if (DateTime.UtcNow - complexObject.Item2 <= TimeSpan.FromSeconds(179)) return;
            complexObject.Item1.CloseChatChannel();
            _timer.Dispose();
        }
    }
}