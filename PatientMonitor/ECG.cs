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

            // fundemental wave (Base frequency)
            sample += amplitude * Math.Sin(2 * Math.PI * frequency * timeIndex);

            // Additional waves based on harmonics count
            for (int h = 1; h <= harmonics; h++)
            {
                // Additional frequency component as an odd multiple of the base frequency
                double additionalFrequency = (2 * h + 1) * frequency;

                // Lower magnitude for every additional wave
                double additionalAmplitude = amplitude / (2 * h + 1);

                // Addition of the wave
                sample += additionalAmplitude * Math.Sin(2 * Math.PI * additionalFrequency * timeIndex);
            }

            return sample;
        }





    }
}

