using Steganography.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Encrypter
{
    /// <summary>
    /// Encrypts an embedded image.
    /// </summary>
    public class ImageEncrypter
    {
        private const int Size = 4;

        public void EncryptImage(byte[] sourceOfOri, uint oriPixelWidth, uint oriPixelHeight)
        {
            var first = sourceOfOri.GetFirstQuadrant(oriPixelWidth, oriPixelHeight);
        }
    }
}
