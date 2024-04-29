using System.Net;

namespace MonitorServer.Middleware
{
    public class MonitorMiddlewarePipeline
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                IPAddress? localIpAddress = context.Connection.LocalIpAddress;
                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                if (localIpAddress != null
                    && remoteIpAddress != null
                    && !localIpAddress.Equals(remoteIpAddress))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
                
                await next(context);
            });
        }
    }
}
