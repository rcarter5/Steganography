using Steganography.ROT;
using Steganography.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Embed
{
    /// <summary>
    /// Embeds text inside an image
    /// </summary>
    public class TextEmbedder
    {
        /// <summary>
        /// Embeds the text using one bit.
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="decoderPixelWidth">Width of the decoder pixel.</param>
        /// <param name="decoderPixelHeight">Height of the decoder pixel.</param>
        /// <param name="text">The text.</param>
        /// <param name="isEncrypted">if set to <c>true</c> [is encrypted].</param>
        /// <param name="rotated">The rotated.</param>
        public void EmbedTextUsingOneBit(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight, string text, bool isEncrypted, int rotated)
        {
            if (isEncrypted)
            {
                text = text.ForwardRot(rotated);
            }
            text += "!EOM!";
            long pixelElementIndex = 0;
            int charIndex = 0;
            int charValue = 0;
            bool notDone = true;
            for (var i = 0; i < decoderPixelHeight; i++)
            {
                for (var j = 0; j < decoderPixelWidth; j++)
                {
                    var pixelColor = PixelColor.GetPixelBgra8(sourcePixels, i, j, decoderPixelWidth, decoderPixelHeight);

                    var r = pixelColor.R - pixelColor.R % 2;
                    var g = pixelColor.G - pixelColor.G % 2;
                    var b = pixelColor.B - pixelColor.B % 2;
                    if (j == 0 && i == 0)
                    {
                        pixelColor.R = 119;
                        pixelColor.B = 119;
                        pixelColor.G = 119;
                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);
                    }
                    else if (j == 1 && i == 0)
                    {
                        pixelColor.R |= 1;
                        g &= ~(1 << 0);
                        g &= ~(1 << 1);
                        pixelColor.G = (byte)g;
                        pixelColor.G |= 1 << 0;
                        b &= ~(1 << 7);
                        if (isEncrypted)
                        {
                            b |= 1 << 7;
                            b = this.changeFiveLSB(b);
                            b += rotated;
                            pixelColor.B = (byte)b;
                        }

                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);
                    }
                    else
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            if (pixelElementIndex % 8 == 0)
                            {
                                if (charIndex >= text.Length)
                                {
                                    notDone = false;
                                }
                                else
                                {
                                    charValue = text[charIndex++];
                                }
                            }
                            switch (pixelElementIndex % 3)
                            {
                                case 0:
                                    {
                                        if (notDone)
                                        {
                                            r += charValue % 2;

                                            charValue /= 2;
                                        }
                                        pixelColor.R = (byte)r;
                                    }
                                    break;
                                case 1:
                                    {
                                        if (notDone)
                                        {
                                            g += charValue % 2;

                                            charValue /= 2;
                                        }
                                        pixelColor.G = (byte)g;
                                    }
                                    break;
                                case 2:
                                    {
                                        if (notDone)
                                        {
                                            b += charValue % 2;

                                            charValue /= 2;
                                        }
                                        pixelColor.B = (byte)b;

                                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth,
                                            decoderPixelHeight);
                                    }
                                    break;
                            }
                            pixelElementIndex++;

                        }
                    }
                }
            }
        }



        /// <summary>
        /// Embeds the text using two bits.
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="decoderPixelWidth">Width of the decoder pixel.</param>
        /// <param name="decoderPixelHeight">Height of the decoder pixel.</param>
        /// <param name="text">The text.</param>
        /// <param name="isEncrypted">if set to <c>true</c> [is encrypted].</param>
        /// <param name="rotated">The rotated.</param>
        public void EmbedTextUsingTwoBits(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight, string text, bool isEncrypted, int rotated)
        {
            if (isEncrypted)
            {
                text = text.ForwardRot(rotated);
            }
            text += "!EOM!";
            long pixelElementIndex = 0;
            int charIndex = 0;
            bool notDone = true;
            string wholebyte = "";
            for (var i = 0; i < decoderPixelHeight; i++)
            {
                for (var j = 0; j < decoderPixelWidth; j++)
                {
                    var pixelColor = PixelColor.GetPixelBgra8(sourcePixels, i, j, decoderPixelWidth, decoderPixelHeight);

                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;

                    r &= ~(1 << 0);
                    r &= ~(1 << 1);
                    g &= ~(1 << 0);
                    g &= ~(1 << 1);
                    b &= ~(1 << 0);
                    b &= ~(1 << 1);

                    if (j == 0 && i == 0)
                    {
                        pixelColor.R = 119;
                        pixelColor.B = 119;
                        pixelColor.G = 119;
                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);
                    }
                    else if (j == 1 && i == 0)
                    {
                        pixelColor.R |= 1;
                        pixelColor.G = (byte)g;
                        pixelColor.G |= 1 << 1;

                        b &= ~(1 << 7);
                        if (isEncrypted)
                        {
                            b |= 1 << 7;
                            b = this.changeFiveLSB(b);
                            b += rotated;
                            pixelColor.B = (byte)b;
                        }

                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);
                    }
                    else
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            if (pixelElementIndex % 4 == 0)
                            {
                                if (charIndex >= text.Length)
                                {
                                    notDone = false;
                                }
                                else
                                {
                                    int charValue = text[charIndex++];
                                    wholebyte = Convert.ToString(charValue, 2).PadLeft(8, '0');
                                }
                            }
                            switch (pixelElementIndex % 3)
                            {
                                case 0:
                                    {
                                        if (notDone)
                                        {
                                            if (Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                r |= 0 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else
                                            {
                                                r |= 1 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                r |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else
                                            {
                                                r |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                        }
                                        pixelColor.R = (byte)r;
                                    }
                                    break;
                                case 1:
                                    {
                                        if (notDone)
                                        {
                                            if (Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                g |= 0 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else
                                            {
                                                g |= 1 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                g |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else
                                            {
                                                g |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                        }
                                        pixelColor.G = (byte)g;
                                    }
                                    break;
                                case 2:
                                    {
                                        if (notDone)
                                        {
                                            if (Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                b |= 0 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else
                                            {
                                                b |= 1 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                b |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else
                                            {
                                                b |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                        }
                                        pixelColor.B = (byte)b;

                                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth,
                                            decoderPixelHeight);
                                    }
                                    break;
                            }
                            pixelElementIndex++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Embeds the text using three bits.
        /// </summary>
        /// <param name="sourcePixels">The source pixels.</param>
        /// <param name="decoderPixelWidth">Width of the decoder pixel.</param>
        /// <param name="decoderPixelHeight">Height of the decoder pixel.</param>
        /// <param name="text">The text.</param>
        public void EmbedTextUsingThreeBits(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight, string text)
        {
            long pixelElementIndex = 0;
            int charIndex = 0;
            string wholebyte = "";
            int bitsThatAreSet = 0;
            for (var i = 0; i < decoderPixelHeight; i++)
            {
                if (bitsThatAreSet % 9 == 0) { }
                for (var j = 0; j < decoderPixelWidth; j++)
                {
                    var pixelColor = PixelColor.GetPixelBgra8(sourcePixels, i, j, decoderPixelWidth, decoderPixelHeight);

                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;

                    r &= ~(1 << 0);
                    r &= ~(1 << 1);
                    g &= ~(1 << 0);
                    g &= ~(1 << 1);
                    b &= ~(1 << 0);
                    b &= ~(1 << 1);

                    if (j == 0 && i == 0)
                    {
                        pixelColor.R = 119;
                        pixelColor.B = 119;
                        pixelColor.G = 119;
                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);
                    }
                    else if (j == 1 && i == 0)
                    {
                        pixelColor.R |= 1;
                        pixelColor.G = (byte)g;
                        pixelColor.G |= 1 << 1;
                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth, decoderPixelHeight);
                    }
                    else
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            if (pixelElementIndex % 4 == 0)
                            {
                                if (charIndex >= text.Length)
                                {
                                    PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth,
                                        decoderPixelHeight);
                                }
                                else
                                {
                                    int charValue = text[charIndex++];
                                    wholebyte = Convert.ToString(charValue, 2).PadLeft(8, '0');
                                }
                            }
                            switch (pixelElementIndex % 3)
                            {
                                case 0:
                                    {
                                        if (charIndex < text.Length)
                                        {
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                r |= 0 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                r |= 1 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                r |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                r |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                r |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                r |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                        }
                                        pixelColor.R = (byte)r;
                                    }
                                    break;
                                case 1:
                                    {
                                        if (charIndex < text.Length)
                                        {
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                g |= 0 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                g |= 1 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                g |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                g |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                g |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                g |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                        }
                                        pixelColor.G = (byte)g;
                                    }
                                    break;
                                case 2:
                                    {
                                        if (charIndex < text.Length)
                                        {
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                b |= 0 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                b |= 1 << 0;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                b |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                b |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 0)
                                            {
                                                b |= 0 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                            else if (wholebyte.Length != 0 && Convert.ToInt32(wholebyte.Substring(wholebyte.Length - 1)) == 1)
                                            {
                                                b |= 1 << 1;
                                                wholebyte = wholebyte.Remove(wholebyte.Length - 1);
                                            }
                                        }
                                        pixelColor.B = (byte)b;

                                        PixelColor.SetPixelBgra8(sourcePixels, i, j, pixelColor, decoderPixelWidth,
                                            decoderPixelHeight);
                                    }
                                    break;
                            }
                            pixelElementIndex++;
                        }
                    }
                }
            }
        }

        private byte changeFiveLSB(int pixelColorB)
        {

            pixelColorB &= ~(1 << 0);
            pixelColorB &= ~(1 << 1);
            pixelColorB &= ~(1 << 2);
            pixelColorB &= ~(1 << 3);
            pixelColorB &= ~(1 << 4);

            return (byte)pixelColorB;
        }
    }
}
