﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using MonitorServer.Models;
using Newtonsoft.Json;

namespace MonitoringAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        private static Dictionary<string, Screenshot> images = new Dictionary<string, Screenshot>();

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

            using (Stream stream = collection.Files[1].OpenReadStream())
            {
                using (FileStream fileStream = new FileStream(basePath + outDirectory + outFile, FileMode.OpenOrCreate,
                           FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }

            images[hostIp + ""] = new Screenshot(outDirectory + outFile, time);

            return NoContent();
        }

        [HttpGet("latestImage")]
        public IActionResult GetLatestImage([FromQuery] string ipAddress)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(images[ipAddress]));
            }
            catch (KeyNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}