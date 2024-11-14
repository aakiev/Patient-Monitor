using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class Respiration : PhysioParameters, IPhysioParameters
    {
        public Respiration(double amplitude, double frequency) : base(amplitude, frequency, 0) { }

        public override double NextSample(double timeIndex)
        {
            double period = 1.0 / Frequency;
            return 2 * Amplitude * (timeIndex / period - Math.Floor(timeIndex / period + 0.5));
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
