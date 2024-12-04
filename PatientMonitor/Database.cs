using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class Database
    {
        const int maxActivePatients = 100;
        private List<Patient> data = new List<Patient>();

        public List<Patient> Data { get => data; set => data = value; }

        public void AddPatient(Patient patient)
        {
            if (data.Count < maxActivePatients)
            {
                data.Add(patient);
            }
            else
            {
                Console.WriteLine("Maximale Anzahl an Patienten erreicht. Der Patient wurde nicht hinzugefügt.");
            }
        }

    }
}
