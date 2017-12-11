using Steganography.ROT;
using Steganography.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Extract
{
    /// <summary>
    /// Extracts text from an image
    /// </summary>
    public class ExtractText
    {
        /// <summary>
        /// Extracts the image for text.
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="decoderPixelWidth">Width of the decoder pixel.</param>
        /// <param name="decoderPixelHeight">Height of the decoder pixel.</param>
        /// <returns></returns>
        public string ExtractImageForText(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight)
        {
            int colorUnitIndex = 0;
            int charValue = 0;

            var secondPixel = PixelColor.GetPixelBgra8(sourcePixels, 0, 1, decoderPixelWidth, decoderPixelHeight);
            int rotatedAmount = secondPixel.B &= 0x1f;

            string extractedText = String.Empty;
            for (var i = 0; i < decoderPixelHeight; i++)
            {
                for (var j = 0; j < decoderPixelWidth; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    if (i == 0 && j == 1)
                    {
                        continue;
                    }
                    var pixelColor = PixelColor.GetPixelBgra8(sourcePixels, i, j, decoderPixelWidth, decoderPixelHeight);

                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    charValue = charValue * 2 + pixelColor.R % 2;
                                }
                                break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixelColor.G % 2;
                                }
                                break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixelColor.B % 2;
                                }
                                break;
                        }

                        colorUnitIndex++;

                        if (colorUnitIndex % 8 == 0)
                        {
                            charValue = this.reverseBits(charValue);


                            var letter = (char)charValue;

                            extractedText += letter.ToString();
                            if (extractedText.Contains("!EOM!"))
                            {
                                j = (int)decoderPixelWidth;
                                i = (int)decoderPixelHeight;
                            }
                        }
                    }
                }
            }
            extractedText = extractedText.Replace("!EOM!", "");
            if ((secondPixel.B &= 1 << 0) == 1 && 1 <= rotatedAmount && rotatedAmount <= 25)
            {
                extractedText = extractedText.ReverseRot(rotatedAmount);
            }

            return extractedText;
        }

        /// <summary>
        /// Extracts the image for textwith two pixels.
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="decoderPixelWidth">Width of the decoder pixel.</param>
        /// <param name="decoderPixelHeight">Height of the decoder pixel.</param>
        /// <returns></returns>
        public string ExtractImageForTextwithTwoPixels(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight)
        {
            int colorUnitIndex = 0;

            var secondPixel = PixelColor.GetPixelBgra8(sourcePixels, 0, 1, decoderPixelWidth, decoderPixelHeight);
            int rotatedAmount = secondPixel.B &= 0x1f;

            string singleByte = "";
            string extractedText = String.Empty;
            for (var i = 0; i < decoderPixelHeight; i++)
            {
                for (var j = 0; j < decoderPixelWidth; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    if (i == 0 && j == 1)
                    {
                        continue;
                    }
                    var pixelColor = PixelColor.GetPixelBgra8(sourcePixels, i, j, decoderPixelWidth, decoderPixelHeight);

                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    int r = pixelColor.R ^= (1 << 0);
                                    if ((r & (1 << 0)) == 0)
                                    {
                                        singleByte += "1";
                                    }
                                    else
                                    {
                                        singleByte += "0";
                                    }
                                    r ^= (1 << 1);
                                    if ((r & (1 << 1)) == 0)
                                    {
                                        singleByte += "1";
                                    }
                                    else
                                    {
                                        singleByte += "0";
                                    }
                                }
                                break;
                            case 1:
                                {

                                    int g = pixelColor.G ^= (1 << 0);
                                    if ((g & (1 << 0)) == 0)
                                    {
                                        singleByte += "1";
                                    }
                                    else
                                    {
                                        singleByte += "0";
                                    }
                                    g = pixelColor.G ^= (1 << 1);
                                    if ((g & (1 << 1)) == 0)
                                    {
                                        singleByte += "1";
                                    }
                                    else
                                    {
                                        singleByte += "0";
                                    }
                                }
                                break;
                            case 2:
                                {
                                    int b = pixelColor.B ^= (1 << 0);
                                    if ((b & (1 << 0)) == 0)
                                    {
                                        singleByte += "1";
                                    }
                                    else
                                    {
                                        singleByte += "0";
                                    }
                                    b = pixelColor.B ^= (1 << 1);
                                    if ((b & (1 << 1)) == 0)
                                    {
                                        singleByte += "1";
                                    }
                                    else
                                    {
                                        singleByte += "0";
                                    }
                                }
                                break;
                        }

                        colorUnitIndex++;

                        if (colorUnitIndex % 4 == 0)
                        {
                            int number = Convert.ToInt32(singleByte, 2);
                            singleByte = String.Empty;
                            var charValue = number;
                            charValue = this.reverseBits(charValue);


                            var letter = (char)charValue;

                            extractedText += letter.ToString();
                            if (extractedText.Contains("!EOM!"))
                            {
                                j = (int)decoderPixelWidth;
                                i = (int)decoderPixelHeight;
                            }
                        }
                    }
                }
            }
            extractedText = extractedText.Replace("!EOM!", "");
            if ((secondPixel.B &= 1 << 0) == 1 && 1 <= rotatedAmount && rotatedAmount <= 25)
            {
                extractedText = extractedText.ReverseRot(rotatedAmount);
            }
            return extractedText;
        }

        private int reverseBits(int charValue)
        {
            var bit = 0;
            for (int i = 0; i < 8; i++)
            {
                bit = bit * 2 + charValue % 2;

                charValue /= 2;
            }
            return bit;
        }

    }
}
