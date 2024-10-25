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
        public double Frequency { get => frequency ; set => frequency = value; }
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

            // Grundwelle (Grundfrequenz) berechnen
            sample += amplitude * Math.Sin(2 * Math.PI * frequency * timeIndex);

            // Hinzufügen der Harmonischen
            for (int h = 1; h <= harmonics; h++)
            {
                // Die Amplitude jeder harmonischen Frequenz wird reduziert, um die Wellen realistischer zu machen
                double harmonicAmplitude = amplitude / (h + 1); // Reduziere die Amplitude der höheren Harmonischen
                double harmonicFrequency = frequency * (h + 1); // Frequenz der Harmonischen ist ein Vielfaches

                // Hinzufügen der harmonischen Frequenz
                sample += harmonicAmplitude * Math.Sin(2 * Math.PI * harmonicFrequency * timeIndex);
            }

            return sample;
        }

    }
}

