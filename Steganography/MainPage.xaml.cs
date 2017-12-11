using Steganography.Embed;
using Steganography.Extensions;
using Steganography.Extract;
using Steganography.File.io;
using Steganography.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Steganography
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Data members

        private WriteableBitmap bitMapImage;
        private string textRead = string.Empty;
        private StorageFile imageToEncript;
        private StorageFile imageUsedToEncrypt;
        private double dpiX;
        private double dpiY;
        private WriteableBitmap decryptedImage;
        private bool toEncrypt;
        private readonly ObservableCollection<string> bitsToEmbed = new ObservableCollection<string>();
        private readonly ObservableCollection<string> rotationAmount = new ObservableCollection<string>();

        #endregion

        #region Constructors

        public MainPage()
        {
            this.InitializeComponent();

            this.decryptedImage = null;
            this.bitMapImage = null;
            this.loadPictureEncryptionButton.IsEnabled = false;
            this.loadTextEncryptionButton.IsEnabled = false;
            this.textToEncryptBox.IsReadOnly = true;
            this.bitsToEncryptBox.IsEnabled = false;
            this.rotationBox.IsEnabled = false;
            this.dpiX = 0;
            this.dpiY = 0;
            this.toEncrypt = false;
            this.bitsToEmbed.ToPopulateListWithValue(3);
            this.rotationAmount.ToPopulateListWithValue(25);
        }
        #endregion

        #region Methods

        private void populateBitsToEmbed()
        {
            this.bitsToEmbed.Add("1");
            this.bitsToEmbed.Add("2");
            this.bitsToEmbed.Add("3");
        }

        private async void LoadPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var imageExtract = new ExtractImage();
            var textExtract = new ExtractText();
            var sourceImageFile = await FileOption.SelectSourceImageFile();
            if (sourceImageFile == null)
            {
                return;
            }

            var copyBitmapImage = await sourceImageFile.MakeCopyOfImage();

            using (var fileStream = await sourceImageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(copyBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(copyBitmapImage.PixelHeight)
                };

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                );

                var sourceOriPixels = pixelData.DetachPixelData();
                var firstPixelcolor =
                    PixelColor.GetPixelBgra8(sourceOriPixels, 0, 0, decoder.PixelWidth, decoder.PixelHeight);
                var secondPixelcolor =
                    PixelColor.GetPixelBgra8(sourceOriPixels, 0, 1, decoder.PixelWidth, decoder.PixelHeight);

                if (firstPixelcolor.R == 119 && firstPixelcolor.G == 119 && firstPixelcolor.B == 119 &&
                    (secondPixelcolor.R & 1) == 1)
                {
                    this.messageTextBlock.Text = "There is a message here";
                    if ((secondPixelcolor.G & (1 << 1)) == 0)
                    {
                        this.textToEncryptBox.Text =
                            textExtract.ExtractImageForText(sourceOriPixels, decoder.PixelWidth,
                                decoder.PixelHeight);
                    }
                    else if ((secondPixelcolor.G & (1 << 0)) == 0)
                    {
                        this.textToEncryptBox.Text =
                            textExtract.ExtractImageForTextwithTwoPixels(sourceOriPixels, decoder.PixelWidth,
                                decoder.PixelHeight);
                    }
                }
                else if (firstPixelcolor.R == 119 && firstPixelcolor.G == 119 && firstPixelcolor.B == 119 &&
                         (secondPixelcolor.R & 1) == 0)
                {
                    this.messageTextBlock.Text = "There is a message here";
                    imageExtract.ExtractWholeImage(sourceOriPixels, decoder.PixelWidth, decoder.PixelHeight);
                    this.decryptedImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    using (var writeStream = this.decryptedImage.PixelBuffer.AsStream())
                    {
                        await writeStream.WriteAsync(sourceOriPixels, 0, sourceOriPixels.Length);
                        this.encrptedImage.Source = this.decryptedImage;
                    }
                }
                else
                {
                    this.messageTextBlock.Text = "There is no message here";
                }
                this.imageToEncript = sourceImageFile;
                this.imageDisplayBox.Source = copyBitmapImage;
            }
        }

        private async void LoadPictureEncryptionButton_Click(object sender, RoutedEventArgs e)
        {
            this.sizeErrorMessage.Visibility = Visibility.Collapsed;
            var sourceImageFile = await FileOption.SelectSourceImageFile();
            if (sourceImageFile == null)
            {
                return;
            }
            var copyBitmapImage = await sourceImageFile.MakeCopyOfImage();
            this.imageUsedToEncrypt = sourceImageFile;
            this.encryptingImage.Source = copyBitmapImage;
        }

        private async void LoadTextEncryptionButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceTextFile = await FileOption.SelectTextSourceImageFile();
            if (sourceTextFile == null)
            {
                return;
            }
            this.readContentsofTheFile(sourceTextFile);
        }

        private async void readContentsofTheFile(StorageFile sourceImageFile)
        {
            IRandomAccessStream inputstream = await sourceImageFile.OpenReadAsync();
            var readFile = new StreamReader(inputstream.AsStream());
            this.textRead = readFile.ReadLine();
            this.textToEncryptBox.Text = this.textRead;
        }

        private async void embedButton_Click(object sender, RoutedEventArgs e)
        {
            var imageEmbedder = new ImageEmbedder();

            if (this.bitsToEncryptBox.SelectedItem == null)
            {
                this.bitsToEncryptBox.SelectedIndex = 0;
            }
            if (this.rotationBox.SelectedItem == null)
            {
                this.rotationBox.SelectedIndex = 0;
            }

            if (this.imageToEncript != null && this.imageUsedToEncrypt != null ||
                this.imageToEncript != null && !this.textRead.Equals(string.Empty))
            {
                if (this.textEncryptionRadio.IsEnabled == false)
                {
                    await this.embedText(imageEmbedder);
                }
                else
                {
                    var copyOriBitImageFile = await this.imageToEncript.MakeCopyOfImage();

                    using (var fileStream = await this.imageToEncript.OpenAsync(FileAccessMode.Read))
                    {
                        var decoder = await BitmapDecoder.CreateAsync(fileStream);

                        var transform = new BitmapTransform
                        {
                            ScaledWidth = Convert.ToUInt32(copyOriBitImageFile.PixelWidth),
                            ScaledHeight = Convert.ToUInt32(copyOriBitImageFile.PixelHeight)
                        };

                        this.dpiX = decoder.DpiX;
                        this.dpiY = decoder.DpiY;

                        var pixelData = await decoder.GetPixelDataAsync(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Straight,
                            transform,
                            ExifOrientationMode.IgnoreExifOrientation,
                            ColorManagementMode.DoNotColorManage
                        );

                        var sourceOriPixels = pixelData.DetachPixelData();

                        this.chooseWhichBitsToEmbed(sourceOriPixels, decoder.PixelWidth, decoder.PixelHeight);
                        this.bitMapImage =
                            new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);

                        using (var writeStream = this.bitMapImage.PixelBuffer.AsStream())
                        {
                            await writeStream.WriteAsync(sourceOriPixels, 0, sourceOriPixels.Length);
                            this.encrptedImage.Source = this.bitMapImage;
                        }
                    }
                }
            }
            else
            {
                var imageNeedToBeLoaded = new ContentDialog
                {
                    Title = "SomeThing Needs Loading",
                    Content = "A image needs to be loaded",
                    CloseButtonText = "Ok"
                };
                await imageNeedToBeLoaded.ShowAsync();
            }
        }

        private async Task embedText(ImageEmbedder imageEmbedder)
        {
            var copyOriBitImageFile = await this.imageToEncript.MakeCopyOfImage();
            var imageToAddCopy = await this.imageUsedToEncrypt.MakeCopyOfImage();

            using (var toEncryptStream = await this.imageUsedToEncrypt.OpenAsync(FileAccessMode.Read))
            {
                using (var fileStream = await this.imageToEncript.OpenAsync(FileAccessMode.Read))
                {
                    var decoder = await BitmapDecoder.CreateAsync(fileStream);
                    var imageToUseDecoder = await BitmapDecoder.CreateAsync(toEncryptStream);

                    var transform = new BitmapTransform
                    {
                        ScaledWidth = Convert.ToUInt32(copyOriBitImageFile.PixelWidth),
                        ScaledHeight = Convert.ToUInt32(copyOriBitImageFile.PixelHeight)
                    };

                    var imageToUseTransform = new BitmapTransform
                    {
                        ScaledWidth = Convert.ToUInt32(imageToAddCopy.PixelWidth),
                        ScaledHeight = Convert.ToUInt32(imageToAddCopy.PixelHeight)
                    };

                    this.dpiX = decoder.DpiX;
                    this.dpiY = decoder.DpiY;

                    var pixelData = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage
                    );

                    var imageToUsePixelData = await imageToUseDecoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        imageToUseTransform,
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage
                    );

                    var sourceOriPixels = pixelData.DetachPixelData();
                    var sourceOfMono = imageToUsePixelData.DetachPixelData();

                    if (secretImageIsLargerThanSourceImage(imageToUseDecoder, decoder))
                    {
                        this.sizeErrorMessage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.sizeErrorMessage.Visibility = Visibility.Collapsed;

                        imageEmbedder.EmbedWholeImage(sourceOriPixels, decoder.PixelWidth, decoder.PixelHeight,
                            sourceOfMono, imageToUseDecoder.PixelWidth, imageToUseDecoder.PixelHeight);

                        this.bitMapImage =
                            new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);

                        using (var writeStream = this.bitMapImage.PixelBuffer.AsStream())
                        {
                            await writeStream.WriteAsync(sourceOriPixels, 0, sourceOriPixels.Length);
                            this.encrptedImage.Source = this.bitMapImage;
                        }
                    }
                }
            }
        }

        private static bool secretImageIsLargerThanSourceImage(BitmapDecoder imageToUseDecoder, BitmapDecoder decoder)
        {
            return imageToUseDecoder.PixelWidth > decoder.PixelWidth ||
                   imageToUseDecoder.PixelHeight > decoder.PixelHeight ||
                   imageToUseDecoder.PixelWidth > decoder.PixelWidth &&
                   imageToUseDecoder.PixelHeight > decoder.PixelHeight;
        }

        private void chooseWhichBitsToEmbed(byte[] sourcePixels, uint decoderPixelWidth, uint decoderPixelHeight)
        {
            var embed = new TextEmbedder();
            if (this.bitsToEncryptBox.SelectedIndex == -1 || this.bitsToEncryptBox.SelectedIndex == 0)
            {
                embed.EmbedTextUsingOneBit(sourcePixels, decoderPixelWidth, decoderPixelHeight,
                    this.textToEncryptBox.Text, this.toEncrypt, Convert.ToInt32(this.rotationBox.SelectedItem));
            }
            else if (this.bitsToEncryptBox.SelectedIndex == 1)
            {
                embed.EmbedTextUsingTwoBits(sourcePixels, decoderPixelWidth, decoderPixelHeight,
                    this.textToEncryptBox.Text, this.toEncrypt, Convert.ToInt32(this.rotationBox.SelectedItem));
            }
        }

        private void textEncryptionRadio_Checked(object sender, RoutedEventArgs e)
        {
            this.loadTextEncryptionButton.IsEnabled = true;
            this.loadPictureEncryptionButton.IsEnabled = false;
            this.bitsToEncryptBox.IsEnabled = true;
            this.pictureEncryptionRadio.IsEnabled = false;
            this.rotationBox.IsEnabled = true;
        }

        private void pictureEncryptionRadio_Click(object sender, RoutedEventArgs e)
        {
            this.loadPictureEncryptionButton.IsEnabled = true;
            this.loadTextEncryptionButton.IsEnabled = false;
            this.bitsToEncryptBox.IsEnabled = false;
            this.textEncryptionRadio.IsEnabled = false;
            this.rotationBox.IsEnabled = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.saveWritableBitMap();
        }

        private async void saveWritableBitMap()
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };

            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> { ".png" });
            var savefile = await fileSavePicker.PickSaveFileAsync();

            if (savefile != null)
            {
                var stream = await savefile.OpenAsync(FileAccessMode.ReadWrite);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                var pixelStream = this.bitMapImage.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)this.bitMapImage.PixelWidth,
                    (uint)this.bitMapImage.PixelHeight, this.dpiX, this.dpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }

        private void encryptBox_Checked(object sender, RoutedEventArgs e)
        {
            this.toEncrypt = true;
        }

        private void encryptBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.toEncrypt = false;
        }

        #endregion
    }
}
