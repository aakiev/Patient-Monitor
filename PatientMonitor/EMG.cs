using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class EMG : PhysioParameters, IPhysioParameters
    {
        public EMG(double amplitude, double frequency) : base(amplitude, frequency, 0) { }

        public override double NextSample(double timeIndex)
        {
            double sample;
            double periodeInTicks = 0.0;
            double step = 0.0;

            periodeInTicks = (double)(1.0 / Frequency);
            step = (double)(timeIndex % periodeInTicks);
            if (step > (periodeInTicks / 2.0))
            {
                sample = 1;
            }
            else
            {
                sample = -1;
            }

            return (Amplitude * sample);
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
