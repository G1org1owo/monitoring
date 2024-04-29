using Microsoft.AspNetCore.Mvc;
using MonitorServer.Models;
using Newtonsoft.Json;

namespace MonitorServer.Controllers
{
    [Route("api")]
    [ApiController]
    public class ExposedMonitoringController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ScreenshotContext _context;

        public ExposedMonitoringController(IWebHostEnvironment env, ScreenshotContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormCollection collection)
        {
            DateTime time;
            string username;

            using (Stream stream = collection.Files[0].OpenReadStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = await reader.ReadToEndAsync();
                    Dictionary<string, string> info = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;

                    time = DateTime.Parse(info["time"]);
                    username = info["username"];
                }
            }

            string basePath = _env.WebRootPath;

            string outDirectory = string.Format("/images/{0:D4}-{1:D2}-{2:D2}/{3}/",
                time.Year,
                time.Month,
                time.Day,
                username
            );
            string outFile = string.Format("{0:D2}-{1:D2}-{2:D2}.jpg",
                time.Hour,
                time.Minute,
                time.Second
            );

            Directory.CreateDirectory(basePath + outDirectory);

            await using (Stream stream = collection.Files[1].OpenReadStream())
            {
                await using (FileStream fileStream = new FileStream(basePath + outDirectory + outFile,
                                 FileMode.OpenOrCreate,
                                 FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }

            Screenshot? screenshot = await _context.Screenshots!.FindAsync(username);

            if (screenshot == null)
            {
                screenshot = new Screenshot(outDirectory + outFile, time, username);
                _context.Screenshots!.Add(screenshot);
            }
            else
            {
                screenshot.imageUrl = outDirectory + outFile;
                screenshot.timestamp = time;
                _context.Screenshots!.Update(screenshot);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}