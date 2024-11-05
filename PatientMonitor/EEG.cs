using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class EEG
    {
        private double amplitude = 0.0;
        private double frequency = 0.0;


        public double Amplitude { get => amplitude; set => amplitude = value; }
        public double Frequency { get => frequency; set => frequency = value; }


        public EEG(double amplitude, double frequency)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        public double NextSample(double timeIndex)
        {
            double sample = 0.0;
            double signalLength = 1.0 / frequency;
            double stepindex = timeIndex % signalLength;

            if (stepindex <= (signalLength / 2))
            {
                sample = -amplitude + (2*amplitude * (1 - Math.Exp(-5.0*(stepindex/(signalLength/2)))));
            }
            else
            {
                sample = amplitude - (2*amplitude * (1 - Math.Exp(-5.0 * ((stepindex-signalLength/2)/(signalLength / 2)))));
            }
            return sample;
        }

    }
}
