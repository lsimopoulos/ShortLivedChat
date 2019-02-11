using System;
using System.Threading;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ShortLivedChatServer.Classes;
using ShortLivedChatServer.Hubs;
using ShortLivedChatServer.IdentityServerConfig;

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

            services.AddMvcCore()
                .AddJsonFormatters()
                .AddAuthorization();

            //https://github.com/IdentityServer/IdentityServer4/issues/2846
            //adding a dummy cookie handler

            services.AddIdentityServer(options => options.Authentication.CookieAuthenticationScheme = "dummy")
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(ISConfig.GetApiResources())
                .AddInMemoryClients(ISConfig.GetClients())
                .AddInMemoryPersistedGrants()
                .AddTestUsers(ISConfig.GetUsers())
                //.AddSigningCredential(Cert.Get("theCert.pfx", "somePassword"))
                .AddDeveloperSigningCredential();





            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:5001/";
                    options.RequireHttpsMetadata = true;
                    options.ApiName = "shortlivedchat";
                }).AddCookie("dummy")
                ;
            
            services.AddSingleton<GroupsManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {


            app.UseIdentityServer();
            app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); });


            app.Map("/api", builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapRoute(
                        "controller",
                        "api/{controller}");
                    routes.MapRoute(
                        "controllerAndAction",
                        "api/{controller}/{action}");
                    routes.MapRoute(
                        "controllerAndActionAndId",
                        "api/{controller}/{action}/{id?}");
                });
            });

            //Task.Factory.StartNew(() =>
            //{
            //    _timer = new Timer(TimerCallback, DateTime.UtcNow, 0, 1000);
            //});
        }

        //TODO: Refactor
        /// <summary>
        /// callback fo the timer.
        /// </summary>
        /// <param name="state"></param>
        //private static void TimerCallback(object state)
        //{

        //    if (DateTime.UtcNow - complexObject.Item2 > TimeSpan.FromSeconds(150) &&
        //        DateTime.UtcNow - complexObject.Item2 < TimeSpan.FromSeconds(180))
        //        complexObject.Item1.SendMessageFromServer(
        //            $"This chat channel is about to close in {180 - (DateTime.UtcNow - complexObject.Item2).TotalSeconds:##}s");

        //    if (DateTime.UtcNow - complexObject.Item2 <= TimeSpan.FromSeconds(179)) return;
        //    complexObject.Item1.CloseChatChannel();
        //    _timer.Dispose();
        //}
    }
}