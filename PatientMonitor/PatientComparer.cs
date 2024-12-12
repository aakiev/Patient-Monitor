using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PatientMonitor
{
    class PatientComparer : IComparer<Patient>
    {
        MonitorConstants.compareAfter ca;
        public MonitorConstants.compareAfter CA
        {
            set { ca = value; }
            get { return ca; }
        }
        public int Compare(Patient x, Patient y)
        {
            int result = 0;

            switch (ca)
            {
                case MonitorConstants.compareAfter.Age: result = x.Age.CompareTo(y.Age); break;
                case MonitorConstants.compareAfter.Name: result = string.Compare(x.PatientName, y.PatientName, StringComparison.Ordinal); break;
                case MonitorConstants.compareAfter.Clinic: result = string.Compare(x.Clinic.ToString(), y.Clinic.ToString(), StringComparison.Ordinal); break;
                case MonitorConstants.compareAfter.Ambulatory: result = (x is StationaryPatient).CompareTo(y is StationaryPatient); break;
                case MonitorConstants.compareAfter.Stationary: result = (x is StationaryPatient).CompareTo(y is StationaryPatient);
                result = (y is StationaryPatient).CompareTo(x is StationaryPatient); break;
                default: result = 0; break;
            }
            return result;
        }
    }
}