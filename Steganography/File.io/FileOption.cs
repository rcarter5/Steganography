using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Steganography.File.io
{
    public static class FileOption
    {

        /// <summary>
        /// Selects the source image file.
        /// </summary>
        /// <returns>Storage File That Was Selected</returns>
        public static async Task<StorageFile> SelectSourceImageFile()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            return file;

        }

        /// <summary>
        /// Selects the text source image file.
        /// </summary>
        /// <returns>Storage File That was selected</returns>
        public static async Task<StorageFile> SelectTextSourceImageFile()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add(".txt");

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }


    }
}
