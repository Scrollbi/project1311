using System;
using System.Windows.Media.Imaging;

namespace project2
{
    public class ImageHandler
    {
        public BitmapImage LoadProductImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return CreateSimplePlaceholder();
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return CreateSimplePlaceholder();
            }
        }

       
        private BitmapImage CreateSimplePlaceholder()
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Resources/no-image.png", UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }
}
