using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientMonitor
{
    internal class Patient
    {

        ECG ecg;
        EEG eeg;
        EMG emg;
        Respiration respiration;
        

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
                case MonitorConstants.Parameter.EEG: NextSample = eeg.NextSample(timeIndex); break;
                case MonitorConstants.Parameter.EMG: NextSample = emg.NextSample(timeIndex); break;
                case MonitorConstants.Parameter.Respiration: NextSample = respiration.NextSample(timeIndex); break;
                default: break;

            }

            return NextSample;
        }

        public Patient(string patientName, DateTime dateOfStudy, int age, double amplitude, double frequency, int harmonics)
        {
            ecg = new ECG(amplitude, frequency, harmonics);
            eeg = new EEG(amplitude, frequency);
            emg = new EMG(amplitude, frequency);
            respiration = new Respiration(amplitude, frequency);


            this.patientName = patientName;
            this.dateOfStudy = dateOfStudy;
            this.age = age;
        }


        //Properties for ECG
        public double ECGAmplitude { get => ecg.Amplitude; set => ecg.Amplitude = value; }
        public double ECGFrequency { get => ecg.Frequency; set => ecg.Frequency = value; }
        public int ECGHarmonics { get => ecg.Harmonics; set => ecg.Harmonics = value; }


        //Properties for EEG
        public double EEGAmplitude { get => eeg.Amplitude; set => eeg.Amplitude = value; }
        public double EEGFrequency { get => eeg.Frequency; set => eeg.Frequency = value; }

        //Properties for EMG
        public double EMGAmplitude { get => emg.Amplitude; set => emg.Amplitude = value; }
        public double EMGFrequency { get => emg.Frequency; set => emg.Frequency = value; }


        //Properties for Respiration
        public double RespirationAmplitude { get => respiration.Amplitude; set => respiration.Amplitude = value; }
        public double RespirationFrequency { get => respiration.Frequency; set => respiration.Frequency = value; }


    }
}
