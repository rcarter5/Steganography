using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Steganography.Extensions
{

    /// <summary>
    ///  Provides extensions for storage file
    /// </summary>
    public static class ImageExtensions
    {
        #region Methods

        /// <summary>
        /// Makes the copy of image.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A bitMap from the storageFile</returns>
        public static async Task<BitmapImage> MakeCopyOfImage(this StorageFile file)
        {
            IRandomAccessStream inputstream = await file.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputstream);
            return newImage;

        }

        #endregion
    }
}
