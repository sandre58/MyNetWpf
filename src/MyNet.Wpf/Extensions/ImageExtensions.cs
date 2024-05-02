// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows.Media.Imaging;

namespace MyNet.Wpf.Extensions
{
    public static class ImageExtensions
    {
        #region Public Methods and Operators

        public static BitmapSource? ToBitmap(this byte[] objSource)
        {
            if (objSource.Length > 0)
            {
                var image = new BitmapImage();
                using (var mem = new MemoryStream(objSource))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();

                }

                image.Freeze();
                return image;
            }
            return null;
        }

        public static byte[] ToBytes(this BitmapSource objSource)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(objSource));
            byte[] data;
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        #endregion Public Methods and Operators
    }
}
