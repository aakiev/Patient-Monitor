using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PatientMonitor
{
    internal class Patient
    {

        ECG ecg;
        EEG eeg;
        EMG emg;
        Respiration respiration;

        private MonitorConstants.clinic clinic;
        string clinictype;

        private string patientName;
        private DateTime dateOfStudy;
        private int age;

        const int maxSamples = 1024;
        List<double> sampleList = new List<double>(maxSamples);

        public string PatientName { get => patientName; set => patientName = value; }
        public DateTime DateOfStudy { get => dateOfStudy; set => dateOfStudy = value; }
        public int Age { get => age; set => age = value; }
        public string Clinictype { get => clinictype; set => clinictype = value; }
        public MonitorConstants.clinic Clinic
        {
            get { return clinic; }
            set { clinic = value; }
        }

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

            if(sampleList.Count <= maxSamples)
            {
                sampleList.Add(NextSample);
            }
            else
            {
                sampleList.RemoveAt(0);
                sampleList.Add(NextSample);
            }
            
            return NextSample;
        }

        public Patient(string patientName, DateTime dateOfStudy, int age, double amplitude, double frequency, int harmonics, MonitorConstants.clinic clinic)
        {
            ecg = new ECG(amplitude, frequency, harmonics);
            eeg = new EEG(amplitude, frequency);
            emg = new EMG(amplitude, frequency);
            respiration = new Respiration(amplitude, frequency);

            this.patientName = patientName;
            this.dateOfStudy = dateOfStudy;
            this.age = age;
            this.clinic = clinic;

            switch (clinic)
            {
                case MonitorConstants.clinic.Cardiology: clinictype = "Cardiology"; break;
                case MonitorConstants.clinic.Dermatology: clinictype = "Dermatology"; break;
                case MonitorConstants.clinic.Neurology: clinictype = "Neurology"; break;
                case MonitorConstants.clinic.Oftalmology: clinictype = "Oftalmology"; break;
                case MonitorConstants.clinic.Orthopedics: clinictype = "Othopedics"; break;
                case MonitorConstants.clinic.Pediatrics: clinictype = "Pediatrics"; break;
                case MonitorConstants.clinic.Radiology: clinictype = "Radiologoy"; break;
                case MonitorConstants.clinic.Surgery: clinictype = "Surgery"; break;
                default: break;
            }

        }

        public void displayLowAlarm(MonitorConstants.Parameter parameter, double frequency, double lowAlarm)
        {
            switch (parameter)
            {
                case MonitorConstants.Parameter.ECG: ecg.displayLowAlarm(frequency, lowAlarm); break;
                case MonitorConstants.Parameter.EEG: eeg.displayLowAlarm(frequency, lowAlarm); break;
                case MonitorConstants.Parameter.EMG: emg.displayLowAlarm(frequency, lowAlarm); break;
                case MonitorConstants.Parameter.Respiration: respiration.displayLowAlarm(frequency, lowAlarm); break;
                default: break;
            }
        }
        public void displayHighAlarm(MonitorConstants.Parameter parameter, double frequency, double highAlarm)
        {
            switch (parameter)
            {
                case MonitorConstants.Parameter.ECG: ecg.displayHighAlarm(frequency, highAlarm); break;
                case MonitorConstants.Parameter.EEG: eeg.displayHighAlarm(frequency, highAlarm); break;
                case MonitorConstants.Parameter.EMG: emg.displayHighAlarm(frequency, highAlarm); break;
                case MonitorConstants.Parameter.Respiration: respiration.displayHighAlarm(frequency, highAlarm); break;
                default: break;
            }
        }


        //Properties for ECG
        public ECG Ecg { get => ecg;}
        public double ECGAmplitude { get => ecg.Amplitude; set => ecg.Amplitude = value; }
        public double ECGFrequency { get => ecg.Frequency; set => ecg.Frequency = value; }
        public int ECGHarmonics { get => ecg.Harmonics; set => ecg.Harmonics = value; }
        public double ECGLowAlarm { get => ecg.LowAlarm; set => ecg.LowAlarm = value; }
        public double ECGHighAlarm { get => ecg.HighAlarm; set => ecg.HighAlarm = value; }
        public string ECGLowAlarmString { get => ecg.LowAlarmString; }
        public string ECGHighAlarmString { get => ecg.HighAlarmString; }


        //Properties for EEG
        public EEG Eeg { get => eeg; }
        public double EEGAmplitude { get => eeg.Amplitude; set => eeg.Amplitude = value; }
        public double EEGFrequency { get => eeg.Frequency; set => eeg.Frequency = value; }
        public double EEGLowAlarm { get => eeg.LowAlarm; set => eeg.LowAlarm = value; }
        public double EEGHighAlarm { get => eeg.HighAlarm; set => eeg.HighAlarm = value; }
        public string EEGLowAlarmString { get => eeg.LowAlarmString; }
        public string EEGHighAlarmString { get => eeg.HighAlarmString; }

        //Properties for EMG
        public EMG Emg { get => emg; }
        public double EMGAmplitude { get => emg.Amplitude; set => emg.Amplitude = value; }
        public double EMGFrequency { get => emg.Frequency; set => emg.Frequency = value; }
        public double EMGLowAlarm { get => emg.LowAlarm; set => emg.LowAlarm = value; }
        public double EMGHighAlarm { get => emg.HighAlarm; set => emg.HighAlarm = value; }
        public string EMGLowAlarmString { get => emg.LowAlarmString; }
        public string EMGHighAlarmString { get => emg.HighAlarmString; }


        //Properties for Respiration
        public Respiration Respiration { get => respiration; }
        public double RespirationAmplitude { get => respiration.Amplitude; set => respiration.Amplitude = value; }
        public double RespirationFrequency { get => respiration.Frequency; set => respiration.Frequency = value; }
        public double RespirationLowAlarm { get => respiration.LowAlarm; set => respiration.LowAlarm = value; }
        public double RespirationHighAlarm { get => respiration.HighAlarm; set => respiration.HighAlarm = value; }
        public string RespirationLowAlarmString { get => respiration.LowAlarmString; }
        public string RespirationHighAlarmString { get => respiration.HighAlarmString; }

        //Property for the sample list
        public List<double> SampleList { get => sampleList; set => sampleList = value; }

    }
}
