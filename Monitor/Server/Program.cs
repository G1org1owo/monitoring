var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    WebRootPath = "../Client/dist/"
});

// Add services to the container.

UriBuilder uri = new UriBuilder(args[0]);
uri.Scheme = "http";

builder.Services.AddControllers();
builder.WebHost.UseUrls(uri.ToString(), "http://127.0.0.1:80");


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
