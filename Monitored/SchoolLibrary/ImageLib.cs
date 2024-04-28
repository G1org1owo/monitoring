using System.Drawing;
using System.Windows.Forms;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace SchoolLibrary
{
    public class ImageLib
    {
        public static Bitmap CaptureScreen()
        {
            Rectangle screenBounds = Screen.AllScreens[0].Bounds;
            Rectangle imageBounds = new Rectangle(0, 0, 1920, 1080);

            Bitmap bitmap = new Bitmap(screenBounds.Width, screenBounds.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);

            g.CopyFromScreen(screenBounds.Left, screenBounds.Top, 0, 0, screenBounds.Size);
            g.Dispose();
            
            // Resize to 1920x1080 to account for different screen resolutions and aspect ratios
            return new Bitmap(bitmap, imageBounds.Width, imageBounds.Height);
        }
    }
}
