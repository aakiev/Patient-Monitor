using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class EEG : PhysioParameters, IPhysioParameters
    {
        public EEG(double amplitude, double frequency) : base(amplitude, frequency, 0) { }

        public override double NextSample(double timeIndex)
        {
            double sample = 0.0;
            double signalLength = 1.0 / Frequency;
            double stepIndex = timeIndex % signalLength;

            if (stepIndex <= (signalLength / 2))
            {
                sample = -Amplitude + (2 * Amplitude * (1 - Math.Exp(-5.0 * (stepIndex / (signalLength / 2)))));
            }
            else
            {
                sample = Amplitude - (2 * Amplitude * (1 - Math.Exp(-5.0 * ((stepIndex - signalLength / 2) / (signalLength / 2)))));
            }

            return sample;
        }

        public new void displayHighAlarm(double frequency, double alarmHigh)
        {
            if (frequency <= LowAlarm)
            {
                LowAlarmString = "Low Alarm: " + LowAlarm;
            }
            else
            {
                LowAlarmString = " ";
            }
        }

        public new void displayLowAlarm(double frequency, double alarmLow)
        {
            if (frequency >= HighAlarm)
            {
                HighAlarmString = "High Alarm: " + HighAlarm;
            }
            else
            {
                HighAlarmString = " ";
            }
        }
    }
}
