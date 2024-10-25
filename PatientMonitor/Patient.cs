using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    class Patient
    {

        ECG ecg;
        private string patientName;
        private DateTime dateOfStudy;
        private int age;

        public string PatientName { get => patientName; set => patientName = value; }
        public DateTime DateOfStudy { get => dateOfStudy; set => dateOfStudy = value; }
        public int Age { get => age; set => age = value; }

        public double NextSample(double timeIndex)
        {
            return ecg.NextSample(timeIndex);               //Hier geben wir über die Klasse Patient Zuriff auf die Methode der Klasse ECG
        }

        public Patient(string patientName, DateTime dateOfStudy, int age, double amplitude, double frequency, int harmonics)
        {
            ecg = new ECG(amplitude, frequency, harmonics);

            this.patientName = patientName;
            this.dateOfStudy = dateOfStudy;
            this.age = age;
        }

        public double ECGAmplitude { get => ecg.Amplitude; set => ecg.Amplitude = value; }
        public double ECGFrequency { get => ecg.Frequency; set => ecg.Frequency = value; }
        public int ECGHarmonics { get => ecg.Harmonics; set => ecg.Harmonics = value; }

    }
}
