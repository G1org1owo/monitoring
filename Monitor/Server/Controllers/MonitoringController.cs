using System.Net;
using Microsoft.AspNetCore.Mvc;
using MonitorServer.Models;
using Newtonsoft.Json;

namespace MonitoringAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        private static Dictionary<string, Screenshot> clients = new Dictionary<string, Screenshot>();

        private readonly IWebHostEnvironment _env;

        public MonitoringController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormCollection collection)
        {
            DateTime time;

            using (Stream stream = collection.Files[0].OpenReadStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = await reader.ReadToEndAsync();
                    Dictionary<string, string> info = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;

                    time = DateTime.Parse(info["time"]);
                }
            }

            IPAddress? hostIp = Request.HttpContext.Connection.RemoteIpAddress;
            string basePath = _env.WebRootPath;

            string outDirectory = string.Format("/images/{0}/{1}-{2}-{3}/",
                hostIp,
                time.Year,
                time.Month,
                time.Day
            );
            string outFile = string.Format("{0}-{1}-{2}.jpg",
                time.Hour,
                time.Minute,
                time.Second
            );

            Directory.CreateDirectory(basePath + outDirectory);

            await using (Stream stream = collection.Files[1].OpenReadStream())
            {
                await using (FileStream fileStream = new FileStream(basePath + outDirectory + outFile, FileMode.OpenOrCreate,
                           FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }

            clients[hostIp + ""] = new Screenshot(outDirectory + outFile, time);

            return NoContent();
        }

        [HttpGet("image")]
        public async Task<IActionResult> GetLatestImage([FromQuery] string ipAddress)
        {
            return await Task.Run(() =>
            {
            try
            {
                if ((DateTime.Now - clients[ipAddress].timestamp).TotalSeconds > 60 * 2)
                {
                    clients.Remove(ipAddress);
                        return Ok(JsonConvert.SerializeObject(new { error = true, info = "Connection Lost" }));
                }

                    return (IActionResult) Ok(JsonConvert.SerializeObject(clients[ipAddress]));
            }
            catch (KeyNotFoundException e)
            {
                    return BadRequest(JsonConvert.SerializeObject(new { error = true, info = e.Message }));
            }
            });
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetConnectedClients()
        {
            return await Task.Run(() => Ok(JsonConvert.SerializeObject(clients.Keys)));
        }
    }
}
