using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    /// <summary>
    /// Die Klasse StationaryPatient repräsentiert einen stationären Patienten, der in einem bestimmten Zimmer einer Klinik untergebracht ist.
    /// </summary>
    internal class StationaryPatient : Patient
    {
        // Zimmernummer des stationären Patienten
        private string roomNumber;

        /// <summary>
        /// Eigenschaft zum Zugriff auf die Zimmernummer.
        /// </summary>
        public string RoomNumber { get => roomNumber; set => roomNumber = value; }

        /// <summary>
        /// Konstruktor: Erstellt einen neuen stationären Patienten mit den angegebenen Parametern.
        /// </summary>
        /// <param name="patientName">Name des Patienten.</param>
        /// <param name="dateOfStudy">Datum der Untersuchung.</param>
        /// <param name="age">Alter des Patienten.</param>
        /// <param name="amplitude">Amplitude des Signals.</param>
        /// <param name="frequency">Frequenz des Signals.</param>
        /// <param name="harmonics">Anzahl der Harmonischen.</param>
        /// <param name="clinic">Die Klinik, in der der Patient untergebracht ist.</param>
        /// <param name="roomNumber">Zimmernummer des Patienten.</param>
        public StationaryPatient(
           string patientName,
           DateTime dateOfStudy,
           int age,
           double amplitude,
           double frequency,
           int harmonics,
           MonitorConstants.clinic clinic,
           string roomNumber
        )
           : base(patientName, dateOfStudy, age, amplitude, frequency, harmonics, clinic) // Aufruf des Konstruktors der Basisklasse Patient
        {
            this.roomNumber = roomNumber; // Setzt die Zimmernummer des Patienten
        }
    }
}
