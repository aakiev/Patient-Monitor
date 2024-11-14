using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class ECG : PhysioParameters, IPhysioParameters
    {
        public ECG(double amplitude, double frequency, int harmonics) : base(amplitude, frequency, harmonics) { }   //Konstruktor überschreibt hier in Elternklasse

        public override double NextSample(double timeIndex)     //Überladen der Methode
        {
            double sample = Amplitude * Math.Cos(2 * Math.PI * Frequency * timeIndex);

            switch (Harmonics)
            {
                case 1: sample += Amplitude / 2 * Math.Cos(2 * Math.PI * 2 * Frequency * timeIndex); break;
                case 2: sample += Amplitude / 3 * Math.Cos(2 * Math.PI * 3 * Frequency * timeIndex) + Amplitude / 2 * Math.Cos(2 * Math.PI * 2 * Frequency * timeIndex); break;
                case 3: sample += Amplitude / 4 * Math.Cos(2 * Math.PI * 4 * Frequency * timeIndex) + Amplitude / 3 * Math.Cos(2 * Math.PI * 3 * Frequency * timeIndex)
                        + Amplitude / 2 * Math.Cos(2 * Math.PI * 2 * Frequency * timeIndex); break;
                default: break;
            }

            return sample;
        }

        public new void displayHighAlarm(double frequency, double alarmHigh)
        {
             if(frequency <= LowAlarm)
            {
                LowAlarmString = "Low Alarm: " + LowAlarm;
            } else
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
