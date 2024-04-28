using FFMpegCore.Pipes;
using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FFMpegCore.Enums;
using FFMpegCore.Extensions.System.Drawing.Common;

namespace SchoolLibrary
{
    public class VideoLib
    {
        private static IEnumerable<IVideoFrame> GetFrames(string directory)
        {
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                yield return new BitmapVideoFrameWrapper(new Bitmap(file));
            }

            yield break;
        }

        public static async Task<Stream> ConvertToVideo(string directory)
        {
            var videoFramesSource = new RawVideoPipeSource(GetFrames(directory))
            {
                FrameRate = 1
            };

            MemoryStream finalStream;
            using (MemoryStream outputStream = new MemoryStream())
            {
                await FFMpegArguments
                    .FromPipeInput(videoFramesSource)
                    .OutputToPipe(new StreamPipeSink(outputStream), options => options
                        .WithVideoCodec(VideoCodec.LibX264)
                        .ForceFormat("ismv"))
                    .ProcessAsynchronously();

                outputStream.Seek(0, SeekOrigin.Begin);
                finalStream = new MemoryStream(outputStream.ToArray());
            }
            return finalStream;
        }
    }
}
