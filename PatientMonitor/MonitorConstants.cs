using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    /// <summary>
    /// Statische Klasse zur Definition von Konstanten und Enumerationen, die in der gesamten Anwendung verwendet werden.
    /// </summary>
    internal static class MonitorConstants
    {
        /// <summary>
        /// Aufzählung der verschiedenen physiologischen Parameter, die überwacht werden können.
        /// </summary>
        public enum Parameter
        {
            ECG = 0,         // Elektrokardiogramm
            EEG = 1,         // Elektroenzephalogramm
            EMG = 2,         // Elektromyogramm
            Respiration = 3  // Atmung
        }

        /// <summary>
        /// Aufzählung der verfügbaren Kliniken, in denen die Patienten behandelt werden.
        /// </summary>
        public enum clinic
        {
            Cardiology = 0,   // Kardiologie
            Neurology = 1,    // Neurologie
            Orthopedics = 2,  // Orthopädie
            Surgery = 3,      // Chirurgie
            Dermatology = 4,  // Dermatologie
            Radiology = 5,    // Radiologie
            Oftalmology = 6,  // Augenheilkunde
            Pediatrics = 7    // Pädiatrie
        }

        /// <summary>
        /// Aufzählung der Kriterien, nach denen Patienten sortiert werden können.
        /// </summary>
        public enum compareAfter
        {
            Name = 0,         // Sortierung nach Name
            Age = 1,          // Sortierung nach Alter
            Clinic = 2,       // Sortierung nach Klinik
            Ambulatory = 3,   // Sortierung nach ambulanten Patienten
            Stationary = 4    // Sortierung nach stationären Patienten
        }
    }
}
