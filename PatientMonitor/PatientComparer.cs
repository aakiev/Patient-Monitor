using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    /// <summary>
    /// Die Klasse PatientComparer implementiert das Interface IComparer<Patient>,
    /// um eine Vergleichslogik zwischen zwei Patient-Objekten zu definieren.
    /// Der Vergleich erfolgt anhand eines ausgewählten Kriteriums (z. B. Alter, Name, Kliniktyp).
    /// </summary>
    class PatientComparer : IComparer<Patient>
    {
        // Feld zur Speicherung des Vergleichskriteriums
        private MonitorConstants.compareAfter ca;

        /// <summary>
        /// Property zur Festlegung und Abruf des Vergleichskriteriums.
        /// </summary>
        public MonitorConstants.compareAfter CA
        {
            set { ca = value; }
            get { return ca; }
        }

        /// <summary>
        /// Methode zum Vergleich zweier Patienten anhand des festgelegten Kriteriums.
        /// </summary>
        /// <param name="x">Der erste Patient</param>
        /// <param name="y">Der zweite Patient</param>
        /// <returns>
        /// Ein negativer Wert, wenn x kleiner als y ist,
        /// 0, wenn x gleich y ist,
        /// ein positiver Wert, wenn x größer als y ist.
        /// </returns>
        public int Compare(Patient x, Patient y)
        {
            int result = 0;

            switch (ca)
            {
                // Vergleich nach Alter
                case MonitorConstants.compareAfter.Age:
                    result = x.Age.CompareTo(y.Age);
                    break;

                // Vergleich nach Patientennamen (alphabetisch)
                case MonitorConstants.compareAfter.Name:
                    result = string.Compare(x.PatientName, y.PatientName, StringComparison.Ordinal);
                    break;

                // Vergleich nach Kliniktyp (alphabetisch)
                case MonitorConstants.compareAfter.Clinic:
                    result = string.Compare(x.Clinic.ToString(), y.Clinic.ToString(), StringComparison.Ordinal);
                    break;

                // Vergleich nach Patiententyp (ambulant oder stationär)
                case MonitorConstants.compareAfter.Ambulatory:
                    result = (x is StationaryPatient).CompareTo(y is StationaryPatient);
                    break;

                // Vergleich nach Patiententyp (stationär zuerst, dann ambulant)
                case MonitorConstants.compareAfter.Stationary:
                    result = (y is StationaryPatient).CompareTo(x is StationaryPatient);
                    break;

                // Standardfall, falls kein Vergleichskriterium festgelegt wurde
                default:
                    result = 0;
                    break;
            }

            return result;
        }
    }
}
