using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    public interface IPhysioParameters
    {
        double Amplitude { get; set; }
        double Frequency { get; set; }
        int Harmonics { get; set; }
        double NextSample(double timerIndex);
        void displayLowAlarm(double frequency, double alarmLow);
        void displayHighAlarm(double frequency, double alarmHigh);
    }
}
