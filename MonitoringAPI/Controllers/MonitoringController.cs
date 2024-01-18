using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MonitoringAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        public MonitoringController() { }

        [HttpPost("image")]
        public async Task<IActionResult> ReceiveImage([FromForm] IFormCollection collection)
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

            string outDirectory = string.Format("{0}/images/{1}/",
                AppDomain.CurrentDomain.BaseDirectory,
                Request.HttpContext.Connection.RemoteIpAddress
            );
            string outFile = string.Format("{0}-{1}-{2}_{3}-{4}-{5}.jpg",
                time.Year,
                time.Month,
                time.Day,
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
