using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using MonitorServer.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    WebRootPath = "../Client/dist/"
});

// Add services to the container.

UriBuilder uri = new UriBuilder(args[0]);
uri.Scheme = "http";

builder.Services.AddControllers();
builder.Services.AddDbContext<ScreenshotContext>(opt => 
    opt.UseInMemoryDatabase("screenshots"));
builder.WebHost.UseUrls(uri.ToString(), "http://127.0.0.1:80");


var app = builder.Build();

app.UseRewriter(new RewriteOptions()
    .AddRewrite("^save_videos", "/save_videos.html", true)
);

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
