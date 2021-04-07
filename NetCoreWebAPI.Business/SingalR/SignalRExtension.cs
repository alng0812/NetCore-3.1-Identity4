using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreWebAPI.Common;

namespace NetCoreWebAPI.Business.SingalR
{
    public static class SignalRExtension
    {
        public static IServiceCollection AddSignalRExtension(this IServiceCollection services, IConfiguration configuration)
        {
            var redis = configuration.GetSection("AppSettings").Get<AppSettings>().SignalRRedis;
            services.AddSignalR()
                .AddMessagePackProtocol()
                .AddStackExchangeRedis(redis, options =>
                {
                    options.Configuration.ChannelPrefix = "SignalR_";
                    options.Configuration.DefaultDatabase = 2;
                });
            return services;
        }

        public static HubEndpointConventionBuilder MapMessageHub(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapHub<MessageHub>("/messagehub");//.RequireAuthorization();
        }
    }
}
