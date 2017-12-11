using Steganography.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Steganography.Embed
{
    /// <summary>
    /// Embeds a image with another image.
    /// </summary>
    public class ImageEmbedder
    {
        /// <summary>
        /// Embeds the whole image.
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="decoderPixelWidth">Width of the decoder pixel.</param>
        /// <param name="decoderPixelHeight">Height of the decoder pixel.</param>
        /// <param name="sourceOfMono">The source of mono.</param>
        /// <param name="monoPixelWidth">Width of the mono pixel.</param>
        /// <param name="monoPixelHeight">Height of the mono pixel.</param>
        public void EmbedWholeImage(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight, byte[] sourceOfMono, uint monoPixelWidth, uint monoPixelHeight)
    {
        for (var i = 0; i < monoPixelWidth; i++)
        {
            for (var j = 0; j < monoPixelHeight; j++)
            {
                var pixelColor = PixelColor.GetPixelBgra8(sourcePixels, i, j, decoderPixelWidth, decoderPixelHeight);
                Color monoPixelColor;

                monoPixelColor = PixelColor.GetPixelBgra8ForMono(sourceOfMono, i, j, monoPixelWidth, monoPixelHeight);


                if (j == 0 && i == 0)
                {
                    pixelColor.R = 119;
                    pixelColor.B = 119;
                    pixelColor.G = 119;
                }
                else if (j == 1 && i == 0)
                {
                    pixelColor.R &= 0xfe;
                }
                else if (monoPixelColor.R != 0 && monoPixelColor.B != 0 && monoPixelColor.G != 0)
                {
                    pixelColor.B |= 1;
                }
                else
                {
                    pixelColor.B &= 0xfe;
                }

                PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);

            }
        }
    }
}
}
