using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    // Die Klasse EMG repräsentiert ein EMG-Signal (Elektromyogramm) und erbt von der abstrakten Klasse PhysioParameters.
    // Sie implementiert das Interface IPhysioParameters.
    internal class EMG : PhysioParameters, IPhysioParameters
    {
        // Konstruktor: Initialisiert Amplitude und Frequenz des EMG-Signals.
        // Die Anzahl der Harmonischen wird hier auf 0 gesetzt, da EMG-Signale keine Harmonischen beinhalten.
        public EMG(double amplitude, double frequency) : base(amplitude, frequency, 0) { }

        // Überschreiben der Methode NextSample zur Berechnung des nächsten EMG-Signalwerts
        public override double NextSample(double timeIndex)
        {
            double sample;                  // Speichert den berechneten Sample-Wert
            double periodeInTicks = 0.0;    // Länge einer Periode in Zeit-Ticks
            double step = 0.0;              // Aktueller Schritt innerhalb der Periode

            // Berechnung der Periodendauer basierend auf der Frequenz
            periodeInTicks = 1.0 / Frequency;
            // Berechnung des aktuellen Zeitpunkts innerhalb der Periode
            step = timeIndex % periodeInTicks;

            // Bestimmung des Signalwerts (Rechtecksignal)
            if (step > (periodeInTicks / 2.0))
            {
                sample = 1; // Oberer Signalwert
            }
            else
            {
                sample = -1; // Unterer Signalwert
            }

            // Rückgabe des skalierten Signalwerts (Amplitude wird berücksichtigt)
            return Amplitude * sample;
        }

        // Neue Implementierung der Methode displayHighAlarm, um den oberen Alarm für EMG zu setzen
        public new void displayHighAlarm(double frequency, double alarmHigh)
        {
            // Überprüfung, ob die Frequenz den unteren Alarmwert unterschreitet
            if (frequency <= LowAlarm)
            {
                LowAlarmString = "Low Alarm: " + LowAlarm;
            }
            else
            {
                LowAlarmString = " "; // Kein Alarm
            }
        }

        // Neue Implementierung der Methode displayLowAlarm, um den unteren Alarm für EMG zu setzen
        public new void displayLowAlarm(double frequency, double alarmLow)
        {
            // Überprüfung, ob die Frequenz den oberen Alarmwert überschreitet
            if (frequency >= HighAlarm)
            {
                HighAlarmString = "High Alarm: " + HighAlarm;
            }
            else
            {
                HighAlarmString = " "; // Kein Alarm
            }
        }
    }
}
