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
        private double frequency = 0.0;
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
            sample = amplitude * Math.Cos(2 * Math.PI * frequency * timeIndex);

            // Additional waves based on harmonics count
            switch (harmonics)
            {
                case 1: sample += amplitude/2 * Math.Cos(2 * Math.PI * 2 * frequency * timeIndex); break;
                case 2: sample += amplitude/3 * Math.Cos(2 * Math.PI * 3 * frequency * timeIndex) + amplitude/2 * Math.Cos(2 * Math.PI * 2 * frequency * timeIndex); break;
                case 3: sample += amplitude/4 * Math.Cos(2 * Math.PI * 4 * frequency * timeIndex) + amplitude/3 * Math.Cos(2 * Math.PI * 3 * frequency * timeIndex) 
                        + amplitude/2 * Math.Cos(2 * Math.PI * 2 * frequency * timeIndex); break;
                default: break;
            }

            return sample;
        }





    }
}

