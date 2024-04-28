using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using MonitorServer.Models;
using Newtonsoft.Json;
using SchoolLibrary;

namespace MonitoringAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ScreenshotContext _context;

        public MonitoringController(IWebHostEnvironment env, ScreenshotContext context)
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
                await using (FileStream fileStream = new FileStream(basePath + outDirectory + outFile, FileMode.OpenOrCreate,
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

        [HttpGet("image")]
        public async Task<IActionResult> GetLatestImage([FromQuery] string username)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    Screenshot? screenshot = await _context.Screenshots!.FindAsync(username);
                    if ((DateTime.Now - screenshot!.timestamp).TotalSeconds > 60 * 2)
                    {
                        _context.Remove(screenshot);
                        await _context.SaveChangesAsync();
                        return Ok(JsonConvert.SerializeObject(new { error = true, info = "Connection Lost" }));
                    }

                    return (IActionResult) Ok(JsonConvert.SerializeObject(screenshot));
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
            List<string> clients = _context.Screenshots!.Select(screenshot => screenshot.username).ToList();
            return await Task.Run(() => Ok(JsonConvert.SerializeObject(clients)));
        }

        [HttpGet("video_tree")]
        public async Task<IActionResult> GetVideoTree()
        {
            return await Task.Run(() => Ok(JsonConvert.SerializeObject(
                GetDirectoryTree(_env.WebRootPath + "/images/", _env.WebRootPath),
                Formatting.Indented)
            ));
        }

        [HttpGet("~/video/{directory}")]
        public async Task<IActionResult> GetVideo(string directory)
        {
            directory = HttpUtility.UrlDecode(directory);
            return File(await VideoLib.ConvertToVideo(_env.WebRootPath + directory), "video/mp4");
        }

        private object GetDirectoryTree(string path, string parent)
        {
            string formatFileName(string path, string parent)
            {
                string fileName = Path.GetRelativePath(parent, path)
                    .Replace(@"\", "/");

                if (fileName.Equals("."))
                {
                    fileName = "/";
                }

                if (!fileName.StartsWith('/'))
                {
                    fileName = '/' + fileName;
                }

                return fileName;
            }

            if (System.IO.File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                List<object> children = new List<object>();
                foreach (string file in Directory.EnumerateFileSystemEntries(path))
                {
                    children.Add(GetDirectoryTree(file, parent));
                }
                
                return new
                {
                    name = formatFileName(path, parent),
                    type = "directory",
                    children = children
                };
            }

            return new
            {
                name = formatFileName(path, parent),
                type = "file"
            };
        }
    }
}
