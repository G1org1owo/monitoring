using System.Web;
using Microsoft.AspNetCore.Mvc;
using MonitorServer.Middleware;
using MonitorServer.Models;
using Newtonsoft.Json;
using SchoolLibrary;

namespace MonitorServer.Controllers
{
    [Route("api")]
    [MiddlewareFilter<MonitorMiddlewarePipeline>]
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

        [HttpGet("image")]
        public async Task<IActionResult> GetLatestImage([FromQuery] string username)
        {
            return await Task.Run(async () =>
            {
                Screenshot? screenshot = await _context.Screenshots!.FindAsync(username);
                if (screenshot == null)
                {
                    return BadRequest(JsonConvert.SerializeObject(new { error = true, info = $"User {username} not found" }));
                }
                if((DateTime.Now - screenshot.timestamp).TotalSeconds > 60 * 2)
                {
                    _context.Remove(screenshot);
                    await _context.SaveChangesAsync();
                    return Ok(JsonConvert.SerializeObject(new { error = true, info = "Connection Lost" }));
                }

                return (IActionResult) Ok(JsonConvert.SerializeObject(screenshot));
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
