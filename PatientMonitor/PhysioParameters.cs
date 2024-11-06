using System;

namespace PatientMonitor
{
    internal class PhysioParameters
    {
        // Variablen
        private double amplitude = 0.0;
        private double frequency = 0.0;
        private int harmonics = 1;

        // Properties
        public double Amplitude { get => amplitude; set => amplitude = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public int Harmonics { get => harmonics; set => harmonics = value; }

        // Konstruktor
        public PhysioParameters(double amplitude, double frequency, int harmonics)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.harmonics = harmonics;
        }

        // Virtuelle Methode NextSample
        public virtual double NextSample(double timeIndex)  //wird als virtual deklariert, damit diese von den Tochterklassen mit "override" überladen werden kann
        {
            return 1.0;       //Diese Methode soll nicht ausgeführt werden, sondern nur überschrieben. Deshalb returnen wir einfach nur 1.0
        }
    }
}
