using System;

namespace PatientMonitor
{
    internal class PhysioParameters
    {
        // Variablen
        private double amplitude = 0.0;
        private double frequency = 0.0;
        private int harmonics = 1;

        // Neue Variablen
        private double lowAlarm = 0.0;
        private double highAlarm = 0.0;
        private string lowAlarmString = " ";
        private string highAlarmString = " ";

        // Properties
        public double Amplitude { get => amplitude; set => amplitude = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public int Harmonics { get => harmonics; set => harmonics = value; }
        public string LowAlarmString { get => lowAlarmString; set => lowAlarmString = value; }
        public string HighAlarmString { get => highAlarmString; set => highAlarmString = value; }


        public double LowAlarm {

            get
            {
                return lowAlarm;
            }

            set
            {
                lowAlarm = value;
                displayLowAlarm(frequency, lowAlarm);
                displayHighAlarm(frequency, highAlarm);
            }
        }

        public double HighAlarm
        {

            get
            {
                return highAlarm;
            }

            set
            {
                highAlarm = value;
                displayLowAlarm(frequency, lowAlarm);
                displayHighAlarm(frequency, highAlarm);
            }
        }

        // Konstruktor
        public PhysioParameters(double amplitude, double frequency, int harmonics)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.harmonics = harmonics;
            displayLowAlarm(this.frequency, this.lowAlarm);
            displayHighAlarm(this.frequency, this.highAlarm);
        }

        // Virtuelle Methode NextSample
        public virtual double NextSample(double timeIndex)  //wird als virtual deklariert, damit diese von den Tochterklassen mit "override" überladen werden kann
        {
            return 1.0;       //Diese Methode soll nicht ausgeführt werden, sondern nur überschrieben. Deshalb returnen wir einfach nur 1.0
        }

        public void displayLowAlarm(double frequency, double lowAlarm)
        {
            if(frequency <= lowAlarm)
            {
                lowAlarmString = "Low Alarm: " + lowAlarm;
            } else
            {
                lowAlarmString = " ";
            }
        }

        public void displayHighAlarm(double frequency, double highAlarm)
        {
            if (frequency >= highAlarm)
            {
                highAlarmString = "High Alarm: " + highAlarm;
            }
            else
            {
                highAlarmString = " ";
            }
        }
    }
}
