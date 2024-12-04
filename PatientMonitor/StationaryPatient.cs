using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class StationaryPatient : Patient
    {
        private string roomNumber;

        public string RoomNumber { get => roomNumber; set => roomNumber = value; }

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
           : base(patientName, dateOfStudy, age, amplitude, frequency, harmonics, clinic) 
        {
            this.roomNumber = roomNumber;
        }
    }
}

