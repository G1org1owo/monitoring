using System.Security.Cryptography;
using System.Text;
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

            await using (Stream stream = AesContext.Decrypt(
                             collection.Files[1].OpenReadStream(),
                             username))
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

        [HttpPost("key")]
        public async Task<IActionResult> GetSymmetricKey()
        {
            Dictionary<string, dynamic> requestBody = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(
                await new StreamReader(Request.Body).ReadToEndAsync()
            )!;

            string publicKeyPem = requestBody["pem"];
            string username = requestBody["username"];

            Aes aes = Aes.Create();
            aes.KeySize = 128;
            aes.GenerateIV();
            aes.GenerateKey();

            AesContext.Put(username, aes);

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);

            byte[] encriptedBytes = rsa.Encrypt(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                {
                    keySize = aes.KeySize,
                    key = Convert.ToBase64String(aes.Key),
                    iv = Convert.ToBase64String(aes.IV),
                })),
                RSAEncryptionPadding.Pkcs1
            );

            // Explicit conversion to base64 as ASP.NET  would handle it
            // internally and it would break client-side conversion
            return await Task.Run(() => Ok(Convert.ToBase64String(encriptedBytes)));
        }
    }
}