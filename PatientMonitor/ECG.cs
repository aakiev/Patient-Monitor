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

            // Grundfrequenz berechnen (erste harmonische Komponente)
            sample += amplitude * Math.Sin(2 * Math.PI * frequency * timeIndex);

            // Hinzufügen der zusätzlichen harmonischen Komponenten
            for (int h = 1; h <= harmonics; h++)
            {
                double harmonicFrequency = frequency * (h + 1); // Die Frequenz der harmonischen Komponente als Vielfaches
                double harmonicAmplitude = amplitude / (h + 1); // Die Amplitude wird proportional zur harmonischen Ordnung reduziert

                // Hinzufügen der harmonischen Schwingungen
                sample += harmonicAmplitude * Math.Sin(2 * Math.PI * harmonicFrequency * timeIndex);
            }

            return sample;
        }


    }
}

