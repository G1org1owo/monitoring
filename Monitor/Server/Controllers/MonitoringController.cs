using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MonitoringAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        public MonitoringController() { }

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

            string outDirectory = string.Format("{0}/images/{1}/{2}-{3}-{4}/",
                _env.WebRootPath,
                Request.HttpContext.Connection.RemoteIpAddress,
                time.Year,
                time.Month,
                time.Day
            );
            string outFile = string.Format("{0}-{1}-{2}.jpg",
                time.Hour,
                time.Minute,
                time.Second
            );

            Directory.CreateDirectory(outDirectory);

            using (Stream stream = collection.Files[1].OpenReadStream())
            {
                using (FileStream fileStream = new FileStream(outDirectory + outFile, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }

            return NoContent();
        }
    }
}
