using Steganography.Extract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Extensions
{
    public static class ByteExtensions
    {

        public static byte[] GetFirstQuadrant(this byte[] source ,uint oriPixelWidth, uint oriPixelHeight)
        {
            var extract = new ExtractImage();
            extract.ExtractWholeImage(source, oriPixelWidth, oriPixelHeight);
            var first = (source.Length/2) / 2;
            return source.Take(first).ToArray();
        }
        

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
