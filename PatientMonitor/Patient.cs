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
        EMG emg;

        private string patientName;
        private DateTime dateOfStudy;
        private int age;

        public string PatientName { get => patientName; set => patientName = value; }
        public DateTime DateOfStudy { get => dateOfStudy; set => dateOfStudy = value; }
        public int Age { get => age; set => age = value; }

        public double NextSample(double timeIndex, MonitorConstants.Parameter parameter)
        {
            double NextSample = 0.0;

            switch (parameter)
            {
                case MonitorConstants.Parameter.ECG: NextSample = ecg.NextSample(timeIndex); break;
                case MonitorConstants.Parameter.EEG: break;     //Fehlt die klasse noch
                case MonitorConstants.Parameter.EMG: NextSample = emg.NextSample(timeIndex); break;
                case MonitorConstants.Parameter.Respiration: break; //Fehlt die klasse noch
                default: break;

            }

            return NextSample;
        }

        public Patient(string patientName, DateTime dateOfStudy, int age, double amplitude, double frequency, int harmonics)
        {
            ecg = new ECG(amplitude, frequency, harmonics);
            emg = new EMG(amplitude, frequency);

            this.patientName = patientName;
            this.dateOfStudy = dateOfStudy;
            this.age = age;
        }


        //Properties for ECG
        public double ECGAmplitude { get => ecg.Amplitude; set => ecg.Amplitude = value; }
        public double ECGFrequency { get => ecg.Frequency; set => ecg.Frequency = value; }
        public int ECGHarmonics { get => ecg.Harmonics; set => ecg.Harmonics = value; }

        //Properties for EMG
        public double EMGAmplitude { get => emg.Amplitude; set => emg.Amplitude = value; }
        public double EMGFrequency { get => emg.Frequency; set => emg.Frequency = value; }

    }
}
