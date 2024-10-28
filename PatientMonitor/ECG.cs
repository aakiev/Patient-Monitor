using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    class ECG
    {

        //Variablen
        private double amplitude = 0.0;
        private double frequency = 0;
        private int harmonics = 1;

        //Properties
        public double Amplitude { get => amplitude; set => amplitude = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public int Harmonics { get => harmonics; set => harmonics = value; }

        public ECG(double amplitude, double frequency, int harmonics)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.harmonics = harmonics;
        }

        public double NextSample(double timeIndex)
        {
            double sample = 0.0;

            // Grundwelle berechnen (Basisfrequenz)
            sample += amplitude * Math.Sin(2 * Math.PI * frequency * timeIndex);

            // Zusätzliche Wellen basierend auf der Anzahl der „Harmonischen“
            for (int h = 1; h <= harmonics; h++)
            {
                // Zusätzliche Frequenzkomponente als ungerades Vielfaches der Grundfrequenz
                double additionalFrequency = (2 * h + 1) * frequency;

                // Kleinere Amplitude für jede zusätzliche Welle
                double additionalAmplitude = amplitude / (2 * h + 1);

                // Füge die zusätzliche Welle hinzu
                sample += additionalAmplitude * Math.Sin(2 * Math.PI * additionalFrequency * timeIndex);
            }

            return sample;
        }





    }
}

