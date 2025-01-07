using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    // Die Klasse ECG erbt von der abstrakten Klasse PhysioParameters und implementiert das Interface IPhysioParameters.
    // Sie stellt die spezifische Implementierung für ein EKG-Signal (Elektrokardiogramm) bereit.
    internal class ECG : PhysioParameters, IPhysioParameters
    {
        // Konstruktor, der die Werte für Amplitude, Frequenz und Harmonische setzt und die Basisklassen-Konstruktor aufruft
        public ECG(double amplitude, double frequency, int harmonics) : base(amplitude, frequency, harmonics) { }

        // Überschreiben der Methode NextSample, um das nächste Sample eines EKG-Signals zu berechnen
        public override double NextSample(double timeIndex)
        {
            // Berechnung des Basissignals mit der Grundfrequenz
            double sample = Amplitude * Math.Cos(2 * Math.PI * Frequency * timeIndex);

            // Hinzufügen der Harmonischen zum Basissignal je nach eingestellter Anzahl der Harmonischen
            switch (Harmonics)
            {
                case 1:
                    sample += Amplitude / 2 * Math.Cos(2 * Math.PI * 2 * Frequency * timeIndex);
                    break;
                case 2:
                    sample += Amplitude / 3 * Math.Cos(2 * Math.PI * 3 * Frequency * timeIndex)
                              + Amplitude / 2 * Math.Cos(2 * Math.PI * 2 * Frequency * timeIndex);
                    break;
                case 3:
                    sample += Amplitude / 4 * Math.Cos(2 * Math.PI * 4 * Frequency * timeIndex)
                              + Amplitude / 3 * Math.Cos(2 * Math.PI * 3 * Frequency * timeIndex)
                              + Amplitude / 2 * Math.Cos(2 * Math.PI * 2 * Frequency * timeIndex);
                    break;
                default:
                    break; // Keine weiteren Harmonischen hinzufügen, wenn Harmonics außerhalb des Bereichs liegt
            }

            return sample; // Gibt das berechnete Signal-Sample zurück
        }

        // Neue Implementierung der Methode displayHighAlarm für das EKG, um den oberen Alarm anzuzeigen
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

        // Neue Implementierung der Methode displayLowAlarm für das EKG, um den unteren Alarm anzuzeigen
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
