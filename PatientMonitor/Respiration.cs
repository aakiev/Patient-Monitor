using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class Respiration
    {
        private double amplitude = 0.0;
        private double frequency = 0.0;


        public double Amplitude { get => amplitude; set => amplitude = value; }
        public double Frequency { get => frequency; set => frequency = value; }


        public Respiration(double amplitude, double frequency)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        public double NextSample(double timeIndex)
        {
            double period = 1.0 / frequency; 
            return 2 * amplitude * (timeIndex / period - Math.Floor(timeIndex / period + 0.5));
        }

    }
}
