using System;
using System.Collections.Generic;
using System.IO;

namespace PatientMonitor
{
    // Die Database-Klasse verwaltet eine Liste von Patienten und ermöglicht das Speichern und Laden von Patientendaten.
    internal class Database
    {
        // Maximale Anzahl an aktiven Patienten, die gespeichert werden können
        const int maxActivePatients = 100;

        // Liste, die alle gespeicherten Patienten enthält
        private List<Patient> data = new List<Patient>();

        // Öffentlicher Zugriff auf die Patientendaten
        public List<Patient> Data { get => data; }

        // Fügt einen neuen Patienten zur Liste hinzu, falls die maximale Anzahl nicht überschritten wird
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

        // Speichert die Patientendaten in einer Datei
        public void SaveData(string dataPath)
        {
            try
            {
                using (Stream ausgabe = File.Create(dataPath))
                {
                    using (BinaryWriter writer = new BinaryWriter(ausgabe))
                    {
                        // Speichert die Anzahl der Patienten
                        writer.Write(data.Count);
                        foreach (Patient patient in data)
                        {
                            // Schreibe allgemeine Patientendaten in die Datei
                            writer.Write(patient.ID.ToString()); // ID des Patienten
                            writer.Write(patient is StationaryPatient); // Stationärer oder ambulanter Patient
                            writer.Write(patient.PatientName);
                            writer.Write(patient.Age);
                            writer.Write(patient.DateOfStudy.ToString("o")); // Datum im ISO 8601-Format
                            writer.Write((int)patient.Clinic);

                            // Schreibe Parameter für alle Messarten
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

                            // Schreibe zusätzliche Daten für stationäre Patienten
                            if (patient is StationaryPatient stationaryPatient)
                            {
                                writer.Write(stationaryPatient.RoomNumber);
                            }
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                // Behandelt mögliche Fehler beim Erstellen der Datei
            }
        }

        // Lädt die Patientendaten aus einer Datei
        public void LoadData(string dataPath)
        {
            try
            {
                using (Stream eingabe = File.OpenRead(dataPath))
                {
                    using (BinaryReader reader = new BinaryReader(eingabe))
                    {
                        data.Clear(); // Leert die aktuelle Liste vor dem Laden neuer Daten
                        int patientCount = reader.ReadInt32(); // Anzahl der Patienten einlesen

                        for (int i = 0; i < patientCount; i++)
                        {
                            // Liest allgemeine Patientendaten aus der Datei
                            Guid id = Guid.Parse(reader.ReadString());
                            bool isStationary = reader.ReadBoolean();
                            string patientName = reader.ReadString();
                            int age = reader.ReadInt32();
                            DateTime dateOfStudy = DateTime.Parse(reader.ReadString());
                            MonitorConstants.clinic clinic = (MonitorConstants.clinic)reader.ReadInt32();

                            // Liest die Parameter für alle Messarten
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

                            // Erzeugt entweder einen stationären oder einen ambulanten Patienten
                            if (isStationary)
                            {
                                string roomNumber = reader.ReadString();
                                StationaryPatient stationaryPatient = new StationaryPatient(
                                    patientName, dateOfStudy, age, ecgAmplitude, ecgFrequency, ecgHarmonics, clinic, roomNumber
                                );

                                // Setzt die gelesenen Daten für den stationären Patienten
                                stationaryPatient.ID = id;
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

                                data.Add(stationaryPatient); // Fügt den stationären Patienten zur Datenbank hinzu
                            }
                            else
                            {
                                Patient patient = new Patient(
                                    patientName, dateOfStudy, age, ecgAmplitude, ecgFrequency, ecgHarmonics, clinic
                                );

                                // Setzt die gelesenen Daten für den ambulanten Patienten
                                patient.ID = id;
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

                                data.Add(patient); // Fügt den ambulanten Patienten zur Datenbank hinzu
                            }
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
                // Behandelt mögliche Fehler beim Öffnen der Datei
            }
        }
    }
}
