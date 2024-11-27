using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace PatientMonitor
{
    internal class MRImaging
    {
        private List<BitmapImage> imageList = new List<BitmapImage>();
        private int currentImageIndex = 0;
        private int maxImages = 10;

        public int MaxImages
        {
            get => maxImages;
            set {
                if (value > 0 && value <= imageList.Count)
                {
                    maxImages = value;
                }
                else
                {
                    maxImages = imageList.Count; // Limited to the total uploaded images
                }
            }
        }

        public void LoadImage(string imageFile)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageFile, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            imageList.Add(bitmap);
            currentImageIndex = imageList.Count - 1;
        }


        public BitmapImage GetCurrentImage()
        {
            return imageList[currentImageIndex];
        }

        public BitmapImage ForwardImage()
        {
            currentImageIndex = (currentImageIndex + 1) % maxImages;
            return GetCurrentImage();
        }

        public BitmapImage BackImage()
        {
            currentImageIndex = (currentImageIndex - 1 + maxImages) % maxImages;
            return GetCurrentImage();
        }
    }
}
