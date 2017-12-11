using Steganography.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Extract
{
    /// <summary>
    /// Extracts image from image
    /// </summary>
    public class ExtractImage
    {
        /// <summary>
        /// Extracts the whole image.
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="decoderPixelWidth">Width of the decoder pixel.</param>
        /// <param name="decoderPixelHeight">Height of the decoder pixel.</param>
        public void ExtractWholeImage(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight)
        {
            for (var i = 0; i < decoderPixelHeight; i++)
            {
                for (var j = 0; j < decoderPixelWidth; j++)
                {
                    var pixelColor = PixelColor.GetPixelBgra8(sourcePixels, i, j, decoderPixelWidth, decoderPixelHeight);
                    if (j == 0 && i == 0)
                    {
                        continue;
                    }
                    else if (j == 1 && i == 0)
                    {
                        continue;
                    }
                    else if (pixelColor.B % 2 == 0)
                    {
                        pixelColor.B = 0;
                        pixelColor.R = 0;
                        pixelColor.G = 0;
                    }
                    else
                    {
                        pixelColor.B = 255;
                        pixelColor.R = 255;
                        pixelColor.G = 255;
                    }

                    PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);

                }
            }
        }
    }
}
