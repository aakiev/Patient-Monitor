using System;
using System.Drawing;


namespace PatientMonitor
{
    internal class MRImaging
    {
        private Bitmap anImage;
        public MRImaging() { }
        public void loadImages(string imageFile)
        {
            anImage = new Bitmap(imageFile);
        }

        public Bitmap AnImage { get => anImage; set => anImage = value; }
    }
}
