using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using MonitorServer.Models;

Task runRequestEndpoint()
{
    var requestEndpointBuilder = WebApplication.CreateBuilder(new WebApplicationOptions()
    {
        Args = args,
        WebRootPath = "../Client/dist/"
    });

    // Add services to the container.

    UriBuilder uri = new UriBuilder(args[0])
    {
        Scheme = "http"
    };

    requestEndpointBuilder.Services.AddControllers();
    requestEndpointBuilder.Services.AddDbContext<ScreenshotContext>(opt =>
        opt.UseInMemoryDatabase("screenshots"));
    requestEndpointBuilder.WebHost.UseUrls(uri.ToString());

    var requestEndpointApp = requestEndpointBuilder.Build();

    requestEndpointApp.UseAuthorization();
    requestEndpointApp.MapControllers();

    return requestEndpointApp.RunAsync();
}
Task runClientEndpoint()
{
    var clientBuilder = WebApplication.CreateBuilder(new WebApplicationOptions()
    {
        WebRootPath = "../Client/dist/"
    });

    clientBuilder.Services.AddControllers();
    clientBuilder.Services.AddDbContext<ScreenshotContext>(opt =>
        opt.UseInMemoryDatabase("screenshots"));
    clientBuilder.WebHost.UseUrls("http://127.0.0.1:80");

    var clientApp = clientBuilder.Build();

    clientApp.UseRewriter(new RewriteOptions()
        .AddRewrite("^save_videos", "/save_videos.html", true)
    );

    clientApp.UseDefaultFiles();
    clientApp.UseStaticFiles();

    clientApp.UseAuthorization();
    clientApp.MapControllers();

    return clientApp.RunAsync();
}

Task[] tasks = { runRequestEndpoint(), runClientEndpoint() };
await Task.WhenAll(tasks);