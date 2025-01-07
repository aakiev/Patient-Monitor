using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    // Die Klasse Respiration repräsentiert ein Atemfrequenzsignal und erbt von der abstrakten Klasse PhysioParameters.
    // Sie implementiert das Interface IPhysioParameters.
    internal class Respiration : PhysioParameters, IPhysioParameters
    {
        // Konstruktor: Initialisiert Amplitude und Frequenz des Respiration-Signals.
        // Die Anzahl der Harmonischen wird hier auf 0 gesetzt, da das Signal keine Harmonischen besitzt.
        public Respiration(double amplitude, double frequency) : base(amplitude, frequency, 0) { }

        // Überschreiben der Methode NextSample zur Berechnung des nächsten Atemsignalwerts
        public override double NextSample(double timeIndex)
        {
            double period = 1.0 / Frequency; // Berechnung der Periodendauer basierend auf der Frequenz
            // Erzeugung eines Sägezahnsignals, das typisch für Atemmuster ist
            return 2 * Amplitude * (timeIndex / period - Math.Floor(timeIndex / period + 0.5));
        }

        // Neue Implementierung der Methode displayHighAlarm, um den oberen Alarm für das Atemsignal zu setzen
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

        // Neue Implementierung der Methode displayLowAlarm, um den unteren Alarm für das Atemsignal zu setzen
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
