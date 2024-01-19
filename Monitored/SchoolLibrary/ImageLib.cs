using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SchoolLibrary
{
    public class ImageLib
    {
        public static Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.AllScreens[0].Bounds;

            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);

            g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
            g.Dispose();
            
            return bitmap;
        }
    }
}
