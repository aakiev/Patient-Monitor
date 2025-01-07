using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PatientMonitor
{
    /// <summary>
    /// Die Klasse MRImaging verwaltet eine Liste von MRT-Bildern und ermöglicht deren Navigation sowie das Laden neuer Bilder.
    /// </summary>
    internal class MRImaging
    {
        // Liste zur Speicherung der geladenen Bitmap-Bilder
        private List<BitmapImage> imageList = new List<BitmapImage>();

        // Aktueller Index des angezeigten Bildes
        private int currentImageIndex = 0;

        // Maximale Anzahl an anzeigbaren Bildern
        private int maxImages = 0;

        /// <summary>
        /// Eigenschaft zum Abrufen der Liste der geladenen Bilder.
        /// </summary>
        public List<BitmapImage> ImageList { get => imageList; }

        /// <summary>
        /// Eigenschaft zum Festlegen und Abrufen des aktuellen Bildindex.
        /// </summary>
        public int CurrentImageIndex { get => currentImageIndex; set => currentImageIndex = value; }

        /// <summary>
        /// Eigenschaft zur Festlegung der maximalen Anzahl der darstellbaren Bilder.
        /// </summary>
        public int MaxImages
        {
            get => maxImages;
            set
            {
                // MaxImages wird nur gesetzt, wenn der Wert positiv ist und nicht die Anzahl der geladenen Bilder überschreitet
                if (value > 0 && value <= imageList.Count)
                {
                    maxImages = value;
                }
                else
                {
                    // Falls der eingegebene Wert ungültig ist, wird MaxImages auf die Anzahl der geladenen Bilder begrenzt
                    maxImages = imageList.Count;
                }
            }
        }

        /// <summary>
        /// Lädt ein Bild aus einer Datei und fügt es der Liste hinzu.
        /// </summary>
        /// <param name="imageFile">Pfad zur Bilddatei</param>
        public void LoadImage(string imageFile)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageFile, UriKind.Absolute); // Absoluter Pfad zur Bilddatei
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Laden des gesamten Bildes in den Speicher
            bitmap.EndInit();

            imageList.Add(bitmap); // Füge das geladene Bild der Liste hinzu
            maxImages = imageList.Count; // Aktualisiere die maximale Anzahl verfügbarer Bilder
        }

        /// <summary>
        /// Löscht alle geladenen Bilder und setzt die Indizes zurück.
        /// </summary>
        public void ClearImages()
        {
            imageList.Clear();
            currentImageIndex = 0;
            maxImages = 0;
        }

        /// <summary>
        /// Gibt das aktuell angezeigte Bild zurück.
        /// </summary>
        /// <returns>Das aktuelle Bild als BitmapImage</returns>
        public BitmapImage GetCurrentImage()
        {
            return imageList[currentImageIndex];
        }

        /// <summary>
        /// Zeigt das nächste Bild in der Liste an und aktualisiert den Index.
        /// </summary>
        /// <returns>Das nächste Bild als BitmapImage</returns>
        public BitmapImage ForwardImage()
        {
            // Inkrementiere den Index und berechne den neuen Index mit Modulo, um den Anfang der Liste zu erreichen, wenn das Ende erreicht ist
            currentImageIndex = (currentImageIndex + 1) % maxImages;
            return GetCurrentImage();
        }

        /// <summary>
        /// Zeigt das vorherige Bild in der Liste an und aktualisiert den Index.
        /// </summary>
        /// <returns>Das vorherige Bild als BitmapImage</returns>
        public BitmapImage BackImage()
        {
            // Dekrementiere den Index und verwende Modulo, um zur letzten Position zu springen, wenn der Index negativ wird
            currentImageIndex = (currentImageIndex - 1 + maxImages) % maxImages;
            return GetCurrentImage();
        }
    }
}
