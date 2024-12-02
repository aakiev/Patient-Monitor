using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PatientMonitor
{
    internal class MRImaging
    {
        private List<BitmapImage> imageList = new List<BitmapImage>();
        private int currentImageIndex = 0;
        private int maxImages = 0;

        public List<BitmapImage> ImageList { get => imageList;}
        public int CurrentImageIndex { get => currentImageIndex; set => currentImageIndex = value; }
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
            maxImages = imageList.Count; // Maximal verfügbare Bilder aktualisieren
        }

        public void ClearImages()
        {
            imageList.Clear();
            currentImageIndex = 0;
            maxImages = 0;
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
