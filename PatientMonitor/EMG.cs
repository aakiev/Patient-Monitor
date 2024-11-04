﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class EMG
    {
        private double amplitude = 0.0;
        private double frequency = 0;
        public double Amplitude { get => amplitude; set => amplitude = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public EMG(double amplitude, double frequency)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        public double NextSample(double timeIndex)
        {
            double sample;
            double periodeInTicks = 0.0;
            double step = 0.0;

            periodeInTicks = (double)(1.0/frequency);
            step = (double)(timeIndex % periodeInTicks);
            if (step > (periodeInTicks / 2.0))
            {
                sample = 1;
            }
            else
            {
                sample = -1;
            }



            return (amplitude*sample);
        }

    }
}