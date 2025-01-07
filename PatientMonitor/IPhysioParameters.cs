using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    // Das Interface IPhysioParameters definiert grundlegende physiologische Parameter und deren Verhalten.
    // Es soll von Klassen implementiert werden, die mit physikalischen Signalen arbeiten, wie z.B. ECG, EMG, EEG oder Respiration.
    public interface IPhysioParameters
    {
        // Eigenschaft für die Amplitude des Signals
        double Amplitude { get; set; }

        // Eigenschaft für die Frequenz des Signals
        double Frequency { get; set; }

        // Eigenschaft für die Anzahl der Harmonischen im Signal (nur für bestimmte Signalarten relevant)
        int Harmonics { get; set; }

        // Methode, die das nächste Signal-Sample basierend auf dem aktuellen Zeitindex berechnet
        double NextSample(double timerIndex);

        // Methode zur Anzeige und Verarbeitung des unteren Alarmgrenzwertes
        void displayLowAlarm(double frequency, double alarmLow);

        // Methode zur Anzeige und Verarbeitung des oberen Alarmgrenzwertes
        void displayHighAlarm(double frequency, double alarmHigh);
    }
}
