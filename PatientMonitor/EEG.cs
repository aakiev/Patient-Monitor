using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    // Die Klasse EEG erbt von der abstrakten Klasse PhysioParameters und implementiert das Interface IPhysioParameters.
    // Sie stellt die spezifische Implementierung für ein EEG-Signal (Elektroenzephalogramm) bereit.
    internal class EEG : PhysioParameters, IPhysioParameters
    {
        // Konstruktor: Initialisiert die Amplitude und Frequenz für das EEG-Signal.
        // Die Anzahl der Harmonischen wird hier fest auf 0 gesetzt, da EEG-Signale keine Harmonischen berücksichtigen.
        public EEG(double amplitude, double frequency) : base(amplitude, frequency, 0) { }

        // Überschreiben der Methode NextSample zur Berechnung des nächsten EEG-Signalwerts
        public override double NextSample(double timeIndex)
        {
            double sample = 0.0; // Initialisierung des Sample-Werts
            double signalLength = 1.0 / Frequency; // Berechnung der Signalperiode
            double stepIndex = timeIndex % signalLength; // Modulo zur Bestimmung des Zeitpunkts innerhalb der Periode

            // Berechnung des Sample-Werts basierend auf der Position innerhalb der Periode
            if (stepIndex <= (signalLength / 2))
            {
                // Ansteigende Flanke des EEG-Signals
                sample = -Amplitude + (2 * Amplitude * (1 - Math.Exp(-5.0 * (stepIndex / (signalLength / 2)))));
            }
            else
            {
                // Abfallende Flanke des EEG-Signals
                sample = Amplitude - (2 * Amplitude * (1 - Math.Exp(-5.0 * ((stepIndex - signalLength / 2) / (signalLength / 2)))));
            }

            return sample; // Gibt das berechnete Signal-Sample zurück
        }

        // Neue Implementierung der Methode displayHighAlarm, um den oberen Alarm für EEG zu setzen
        public new void displayHighAlarm(double frequency, double alarmHigh)
        {
            // Überprüfung, ob die Frequenz den unteren Alarmwert unterschreitet
            if (frequency <= LowAlarm)
            {
                LowAlarmString = "Low Alarm: " + LowAlarm;
            }
            else
            {
                LowAlarmString = " ";
            }
        }

        // Neue Implementierung der Methode displayLowAlarm, um den unteren Alarm für EEG zu setzen
        public new void displayLowAlarm(double frequency, double alarmLow)
        {
            // Überprüfung, ob die Frequenz den oberen Alarmwert überschreitet
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
