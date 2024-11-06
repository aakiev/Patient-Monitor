using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class Respiration : PhysioParameters
    {
        public Respiration(double amplitude, double frequency) : base(amplitude, frequency, 0) { }

        public override double NextSample(double timeIndex)
        {
            double period = 1.0 / Frequency;
            return 2 * Amplitude * (timeIndex / period - Math.Floor(timeIndex / period + 0.5));
        }
    }
}
