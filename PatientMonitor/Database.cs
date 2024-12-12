using System;
using System.Collections.Generic;
using System.IO;

namespace PatientMonitor
{
    internal class Database
    {
        const int maxActivePatients = 100;
        private List<Patient> data = new List<Patient>();

        public List<Patient> Data { get => data; }

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

        public void SaveData(string dataPath)
        {
            try
            {
                using (Stream ausgabe = File.Create(dataPath))
                {
                    using (BinaryWriter writer = new BinaryWriter(ausgabe))
                    {
                        writer.Write(data.Count);
                        foreach (Patient patient in data)
                        {
                            writer.Write(patient is StationaryPatient);
                            writer.Write(patient.PatientName);
                            writer.Write(patient.Age);
                            writer.Write(patient.DateOfStudy.ToString("o")); // ISO 8601-Format für Datum
                            writer.Write((int)patient.Clinic);

                            // Parameter in Datei laden
                            writer.Write(patient.ECGAmplitude);
                            writer.Write(patient.ECGFrequency);
                            writer.Write(patient.ECGHarmonics);
                            writer.Write(patient.ECGLowAlarm);
                            writer.Write(patient.ECGHighAlarm);

                            writer.Write(patient.EEGAmplitude);
                            writer.Write(patient.EEGFrequency);
                            writer.Write(patient.EEGLowAlarm);
                            writer.Write(patient.EEGHighAlarm);

                            writer.Write(patient.EMGAmplitude);
                            writer.Write(patient.EMGFrequency);
                            writer.Write(patient.EMGLowAlarm);
                            writer.Write(patient.EMGHighAlarm);

                            writer.Write(patient.RespirationAmplitude);
                            writer.Write(patient.RespirationFrequency);
                            writer.Write(patient.RespirationLowAlarm);
                            writer.Write(patient.RespirationHighAlarm);

                            if (patient is StationaryPatient stationaryPatient)
                            {
                                writer.Write(stationaryPatient.RoomNumber);
                            }
                        }
                    }
                }
            }
            catch (ArgumentException) { }
        }

        public void LoadData(string dataPath)
        {
            try
            {
                using (Stream eingabe = File.OpenRead(dataPath))
                {
                    using (BinaryReader reader = new BinaryReader(eingabe))
                    {
                        data.Clear();
                        int patientCount = reader.ReadInt32();

                        for (int i = 0; i < patientCount; i++)
                        {
                            bool isStationary = reader.ReadBoolean();
                            string patientName = reader.ReadString();
                            int age = reader.ReadInt32();
                            DateTime dateOfStudy = DateTime.Parse(reader.ReadString());
                            MonitorConstants.clinic clinic = (MonitorConstants.clinic)reader.ReadInt32();

                            // Parameter abspeichern
                            double ecgAmplitude = reader.ReadDouble();
                            double ecgFrequency = reader.ReadDouble();
                            int ecgHarmonics = reader.ReadInt32();
                            double ecgLowAlarm = reader.ReadDouble();
                            double ecgHighAlarm = reader.ReadDouble();

                            double eegAmplitude = reader.ReadDouble();
                            double eegFrequency = reader.ReadDouble();
                            double eegLowAlarm = reader.ReadDouble();
                            double eegHighAlarm = reader.ReadDouble();

                            double emgAmplitude = reader.ReadDouble();
                            double emgFrequency = reader.ReadDouble();
                            double emgLowAlarm = reader.ReadDouble();
                            double emgHighAlarm = reader.ReadDouble();

                            double respirationAmplitude = reader.ReadDouble();
                            double respirationFrequency = reader.ReadDouble();
                            double respirationLowAlarm = reader.ReadDouble();
                            double respirationHighAlarm = reader.ReadDouble();

                            if (isStationary)
                            {
                                string roomNumber = reader.ReadString();
                                StationaryPatient stationaryPatient = new StationaryPatient(
                                    patientName, dateOfStudy, age, ecgAmplitude, ecgFrequency, ecgHarmonics, clinic, roomNumber
                                );
                                stationaryPatient.ECGAmplitude = ecgAmplitude;
                                stationaryPatient.ECGFrequency = ecgFrequency;
                                stationaryPatient.ECGHarmonics = ecgHarmonics;
                                stationaryPatient.ECGLowAlarm = ecgLowAlarm;
                                stationaryPatient.ECGHighAlarm = ecgHighAlarm;

                                stationaryPatient.EEGAmplitude = eegAmplitude;
                                stationaryPatient.EEGFrequency = eegFrequency;
                                stationaryPatient.EEGLowAlarm = eegLowAlarm;
                                stationaryPatient.EEGHighAlarm = eegHighAlarm;

                                stationaryPatient.EMGAmplitude = emgAmplitude;
                                stationaryPatient.EMGFrequency = emgFrequency;
                                stationaryPatient.EMGLowAlarm = emgLowAlarm;
                                stationaryPatient.EMGHighAlarm = emgHighAlarm;

                                stationaryPatient.RespirationAmplitude = respirationAmplitude;
                                stationaryPatient.RespirationFrequency = respirationFrequency;
                                stationaryPatient.RespirationLowAlarm = respirationLowAlarm;
                                stationaryPatient.RespirationHighAlarm = respirationHighAlarm;

                                data.Add(stationaryPatient);
                            }
                            else
                            {
                                Patient patient = new Patient(
                                    patientName, dateOfStudy, age, ecgAmplitude, ecgFrequency, ecgHarmonics, clinic
                                );

                                patient.ECGAmplitude = ecgAmplitude;
                                patient.ECGFrequency = ecgFrequency;
                                patient.ECGHarmonics = ecgHarmonics;
                                patient.ECGLowAlarm = ecgLowAlarm;
                                patient.ECGHighAlarm = ecgHighAlarm;

                                patient.EEGAmplitude = eegAmplitude;
                                patient.EEGFrequency = eegFrequency;
                                patient.EEGLowAlarm = eegLowAlarm;
                                patient.EEGHighAlarm = eegHighAlarm;

                                patient.EMGAmplitude = emgAmplitude;
                                patient.EMGFrequency = emgFrequency;
                                patient.EMGLowAlarm = emgLowAlarm;
                                patient.EMGHighAlarm = emgHighAlarm;

                                patient.RespirationAmplitude = respirationAmplitude;
                                patient.RespirationFrequency = respirationFrequency;
                                patient.RespirationLowAlarm = respirationLowAlarm;
                                patient.RespirationHighAlarm = respirationHighAlarm;

                                data.Add(patient);
                            }
                        }
                    }
                }
            }
            catch (ArgumentException) { }
        }
    }
}