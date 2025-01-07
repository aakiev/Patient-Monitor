using System;
using System.Collections.Generic;

namespace PatientMonitor
{
    /// <summary>
    /// Die abstrakte Klasse PhysioParameters dient als Basisklasse für physiologische Parameter (z. B. ECG, EEG, EMG, Respiration).
    /// Sie definiert gemeinsame Eigenschaften und Methoden, die von den abgeleiteten Klassen genutzt und überschrieben werden.
    /// </summary>
    internal abstract class PhysioParameters
    {
        // Felder zur Speicherung der Amplitude, Frequenz, Harmonischen und Alarmgrenzen
        private double amplitude = 0.0; // Signal-Amplitude
        private double frequency = 0.0; // Signal-Frequenz
        private int harmonics = 1;      // Anzahl der Harmonischen

        private double lowAlarm = 0.0;  // Untere Alarmgrenze
        private double highAlarm = 0.0; // Obere Alarmgrenze
        private string lowAlarmString = " ";  // Darstellung des Low-Alarms als String
        private string highAlarmString = " "; // Darstellung des High-Alarms als String

        // Eigenschaften (Properties) zur Kapselung der Felder
        public double Amplitude { get => amplitude; set => amplitude = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public int Harmonics { get => harmonics; set => harmonics = value; }
        public string LowAlarmString { get => lowAlarmString; set => lowAlarmString = value; }
        public string HighAlarmString { get => highAlarmString; set => highAlarmString = value; }

        /// <summary>
        /// Eigenschaft für die untere Alarmgrenze. Setzt die Grenze und aktualisiert die Anzeige.
        /// </summary>
        public double LowAlarm
        {
            get { return lowAlarm; }
            set
            {
                lowAlarm = value;
                displayLowAlarm(frequency, lowAlarm);
                displayHighAlarm(frequency, highAlarm);
            }
        }

        /// <summary>
        /// Eigenschaft für die obere Alarmgrenze. Setzt die Grenze und aktualisiert die Anzeige.
        /// </summary>
        public double HighAlarm
        {
            get { return highAlarm; }
            set
            {
                highAlarm = value;
                displayLowAlarm(frequency, lowAlarm);
                displayHighAlarm(frequency, highAlarm);
            }
        }

        /// <summary>
        /// Konstruktor: Initialisiert die physiologischen Parameter mit Amplitude, Frequenz und Harmonischen.
        /// </summary>
        /// <param name="amplitude">Signalamplitude</param>
        /// <param name="frequency">Signalfrequenz</param>
        /// <param name="harmonics">Anzahl der Harmonischen</param>
        public PhysioParameters(double amplitude, double frequency, int harmonics)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.harmonics = harmonics;
            displayLowAlarm(this.frequency, this.lowAlarm);  // Initialisierung der Alarmanzeige
            displayHighAlarm(this.frequency, this.highAlarm);
        }

        /// <summary>
        /// Abstrakte Methode zur Berechnung des nächsten Signalsamples. 
        /// Muss in den abgeleiteten Klassen implementiert werden.
        /// </summary>
        /// <param name="timeIndex">Der aktuelle Zeitindex</param>
        /// <returns>Das nächste Signal-Sample</returns>
        public abstract double NextSample(double timeIndex);

        /// <summary>
        /// Methode zur Anzeige des Low-Alarms, falls die Frequenz die untere Alarmgrenze unterschreitet.
        /// </summary>
        /// <param name="frequency">Aktuelle Frequenz</param>
        /// <param name="lowAlarm">Untergrenze für den Alarm</param>
        public void displayLowAlarm(double frequency, double lowAlarm)
        {
            if (frequency <= lowAlarm)
            {
                lowAlarmString = "Low Alarm: " + lowAlarm;
            }
            else
            {
                lowAlarmString = " ";
            }
        }

        /// <summary>
        /// Methode zur Anzeige des High-Alarms, falls die Frequenz die obere Alarmgrenze überschreitet.
        /// </summary>
        /// <param name="frequency">Aktuelle Frequenz</param>
        /// <param name="highAlarm">Obergrenze für den Alarm</param>
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
