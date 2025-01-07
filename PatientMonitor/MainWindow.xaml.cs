using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization.Charting.Compatible;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging; 
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;


namespace PatientMonitor
{
    public partial class MainWindow : Window
    {
        // Sammlung von Datenpunkten zur Anzeige im Diagramm (X-Wert: Zeit, Y-Wert: Amplitude)
        private ObservableCollection<KeyValuePair<double, double>> dataPoints;

        // Timer für das regelmäßige Aktualisieren der Datenpunkte
        private DispatcherTimer timer;

        // Aktuelle Patientenobjekte (ambulant oder stationär)
        Patient patient;
        StationaryPatient stationaryPatient;

        // Aktuell ausgewählter Parameter (z. B. ECG, EMG)
        MonitorConstants.Parameter parameter = MonitorConstants.Parameter.ECG;

        // Aktuell ausgewählte Klinik
        MonitorConstants.clinic clinic = MonitorConstants.clinic.Cardiology;

        // Objekt zur Verwaltung von MR-Bildern
        private MRImaging mrImaging = new MRImaging();

        // Datenbank zur Speicherung der Patienteninformationen
        Database database;

        // ID des aktuell aktiven Patienten
        private Guid _activePatientId = Guid.Empty;

        // Laufender Index für die Datenpunkte im Diagramm
        private int index = 0;

        // Temporäre Variablen zum Speichern von Nutzereingaben für Patienten
        string patientNameTemp;
        int patientAgeTemp;
        DateTime dateTemp;
        double frequencyTemp = 0;
        int harmonicsTemp = 1;
        double amplitudeValue = 0;
        double lowAlarmTemp = 0;
        double highAlarmTemp = 0;
        string roomNumberTemp;

        // Statusvariablen zur Steuerung des Programmablaufs
        bool wasPatientCreated = false;  // Gibt an, ob bereits ein Patient erstellt wurde
        bool timerStarted = false;       // Gibt an, ob der Timer gestartet wurde
        bool isDatabaseSaved = true;     // Gibt an, ob die Datenbank gespeichert wurde
        private bool isValidationEnabled = true; // Steuert, ob die Eingabevalidierung aktiv ist


        public MainWindow()
        {
            // Initialisiert die GUI-Komponenten des Fensters
            InitializeComponent();

            // Erstellt eine neue Instanz der Datenbank zur Verwaltung von Patientendaten
            database = new Database();

            // Erstellt eine Sammlung von Datenpunkten zur Anzeige im Zeit-Diagramm
            dataPoints = new ObservableCollection<KeyValuePair<double, double>>();

            // Verknüpft die erstellte Sammlung von Datenpunkten mit der LineSeries im Diagramm
            lineSeriesTime.ItemsSource = dataPoints;

            // Erstellt einen DispatcherTimer für die regelmäßige Aktualisierung des Diagramms
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1); // Setzt das Intervall des Timers auf 1 Millisekunde
            timer.Tick += Timer_Tick; // Verknüpft das Timer-Tick-Ereignis mit der Methode Timer_Tick

        }

        // Methode, die bei jedem Tick des Timers ausgeführt wird
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Überprüft, ob der RadioButton für Parameter aktiviert ist
            if (RadioButtonParameter.IsChecked == true)
            {
                // Blendet die Patientendaten-Anzeige aus und zeigt das Zeit-Diagramm an
                PatientData.Visibility = Visibility.Hidden;
                displayTime(); // Zeigt die Echtzeitdaten im Diagramm an
            }
            // Überprüft, ob der RadioButton für die Datenbank aktiviert ist
            else if (RadioButtonDataBase.IsChecked == true)
            {
                // Zeigt die Patientendaten-Anzeige an
                PatientData.Visibility = Visibility.Visible;
                displayDatabase(); // Zeigt die gespeicherten Patientendaten an
            }
        }

        // Methode zur Anzeige der Echtzeitdaten im Diagramm
        private void displayTime()
        {
            // Berechnung der aktuellen Zeit in Sekunden
            double currentTimeInSeconds = index / 6000.0; // 6000 ergibt eine perfekte Simulation von 50 Hz

            // Generiert einen neuen Datenpunkt, wenn ein Patient vorhanden ist
            if (patient != null)
            {
                // Fügt einen neuen Datenpunkt hinzu, indem die Methode NextSample des Patienten aufgerufen wird
                dataPoints.Add(new KeyValuePair<double, double>(index / 100.0, patient.NextSample(currentTimeInSeconds, parameter)));
                index++; // Erhöht den Index für den nächsten Datenpunkt
            }

            // Löscht alte Datenpunkte, um das Diagramm übersichtlich zu halten
            if (dataPoints.Count > 200) // Maximal 200 Datenpunkte anzeigen
            {
                dataPoints.RemoveAt(0); // Entfernt den ältesten Datenpunkt
            }
        }



        // Löscht den Text und setzt die Schriftfarbe auf Schwarz, wenn das Textfeld für den Patientennamen den Fokus erhält
        private void PatientNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Text = ""; // Textfeld leeren
            textBox.Foreground = Brushes.Black; // Schriftfarbe auf Schwarz setzen
        }

        // Setzt den Platzhaltertext und die Schriftfarbe auf Rot, wenn das Textfeld für den Patientennamen den Fokus verliert und leer ist
        private void PatientNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "") // Überprüft, ob das Textfeld leer ist
            {
                textBox.Text = "Enter name here"; // Platzhaltertext setzen
                textBox.Foreground = Brushes.Red; // Schriftfarbe auf Rot setzen
            }
        }

        // Setzt den Platzhaltertext und die Schriftfarbe auf Rot, wenn das Textfeld für das Alter den Fokus verliert und leer ist
        private void PatientAgeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "") // Überprüft, ob das Textfeld leer ist
            {
                textBox.Text = "Enter age here"; // Platzhaltertext setzen
                textBox.Foreground = Brushes.Red; // Schriftfarbe auf Rot setzen
            }
        }

        // Überprüft die Eingabe im Alter-Textfeld und erlaubt nur numerische Eingaben
        private void PatientAgeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _); // Verhindert nicht-numerische Eingaben
        }

        // Speichert den aktuellen Wert des Alter-Textfelds in der temporären Variablen, wenn sich der Text ändert
        private void PatientAgeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(PatientAgeTextBox.Text, out int parsedage); // Versucht, die Eingabe in eine Zahl zu konvertieren
            patientAgeTemp = parsedage; // Speichert das Ergebnis in der temporären Variablen
        }


        // Aktualisiert die temporäre Variable für den Patientennamen bei jeder Änderung des Textfelds
        private void PatientNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            patientNameTemp = PatientNameTextBox.Text; // Speichert den aktuellen Text des Namensfeldes in einer temporären Variable
        }

        // Speichert das ausgewählte Datum aus dem DatePicker in der temporären Variable, wenn sich das Datum ändert
        private void DatePickerDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePickerDate.SelectedDate.HasValue) // Überprüft, ob ein gültiges Datum ausgewählt wurde
                dateTemp = DatePickerDate.SelectedDate.Value; // Speichert das ausgewählte Datum in der temporären Variablen
        }

        // Überprüft die Eingabe der Frequenz und aktualisiert die entsprechende Frequenz des Patienten
        private void TextBoxFrequencyValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Versucht, die Eingabe im Textfeld in eine Gleitkommazahl zu konvertieren
            if (double.TryParse(TextBoxFrequencyValue.Text, out double parsedFrequency))
            {
                // Überprüft, ob die eingegebene Frequenz im gültigen Bereich liegt
                if (parsedFrequency >= 0 && parsedFrequency <= 150)
                {
                    frequencyTemp = parsedFrequency; // Speichert die gültige Frequenz in der temporären Variablen

                    if (wasPatientCreated) // Überprüft, ob bereits ein Patient erstellt wurde
                    {
                        // Schaltet basierend auf dem aktuellen Parameter (ECG, EMG, EEG oder Respiration)
                        switch (parameter)
                        {
                            case MonitorConstants.Parameter.ECG:
                                patient.ECGFrequency = frequencyTemp; // Setzt die neue Frequenz
                                patient.displayLowAlarm(parameter, patient.ECGFrequency, patient.ECGLowAlarm); // Aktualisiert die Low-Alarm-Anzeige
                                patient.displayHighAlarm(parameter, patient.ECGFrequency, patient.ECGHighAlarm); // Aktualisiert die High-Alarm-Anzeige
                                TextBlockDisplayLowAlarm.Text = patient.ECGLowAlarmString; // Zeigt den Low-Alarm-Wert an
                                TextBlockDisplayHighAlarm.Text = patient.ECGHighAlarmString; // Zeigt den High-Alarm-Wert an
                                break;
                            case MonitorConstants.Parameter.EMG:    //Selbes für alle anderen Parameter
                                patient.EMGFrequency = frequencyTemp;
                                patient.displayLowAlarm(parameter, patient.EMGFrequency, patient.EMGLowAlarm);
                                patient.displayHighAlarm(parameter, patient.EMGFrequency, patient.EMGHighAlarm);
                                TextBlockDisplayLowAlarm.Text = patient.EMGLowAlarmString;
                                TextBlockDisplayHighAlarm.Text = patient.EMGHighAlarmString;
                                break;
                            case MonitorConstants.Parameter.EEG:
                                patient.EEGFrequency = frequencyTemp;
                                patient.displayLowAlarm(parameter, patient.EEGFrequency, patient.EEGLowAlarm);
                                patient.displayHighAlarm(parameter, patient.EEGFrequency, patient.EEGHighAlarm);
                                TextBlockDisplayLowAlarm.Text = patient.EEGLowAlarmString;
                                TextBlockDisplayHighAlarm.Text = patient.EEGHighAlarmString;
                                break;
                            case MonitorConstants.Parameter.Respiration:
                                patient.RespirationFrequency = frequencyTemp;
                                patient.displayLowAlarm(parameter, patient.RespirationFrequency, patient.RespirationLowAlarm);
                                patient.displayHighAlarm(parameter, patient.RespirationFrequency, patient.RespirationHighAlarm);
                                TextBlockDisplayLowAlarm.Text = patient.RespirationLowAlarmString;
                                TextBlockDisplayHighAlarm.Text = patient.RespirationHighAlarmString;
                                break;
                        }
                    }
                }
                else
                {
                    // Zeigt eine Warnmeldung an, wenn die Frequenz außerhalb des gültigen Bereichs liegt
                    MessageBox.Show("Frequency must be between 1 and 150 Hz.", "Invalid Frequency", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TextBoxFrequencyValue.Text = "1"; // Setzt das Textfeld auf einen gültigen Standardwert zurück
                }
            }
        }



        // Verhindert die Eingabe ungültiger Zeichen im Textfeld für die Frequenz
        private void TextBoxFrequencyValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _); // Erlaubt nur Zahlen als Eingabe
        }

        // Löscht den Text und setzt die Schriftfarbe auf Schwarz, wenn das Textfeld den Fokus erhält
        private void TextBoxFrequencyValue_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxFrequencyValue.Text = ""; // Setzt das Textfeld auf leer
            TextBoxFrequencyValue.Foreground = Brushes.Black; // Setzt die Schriftfarbe auf Schwarz
        }

        // Überprüft beim Verlassen des Textfelds, ob es leer ist, und setzt einen Standardwert
        private void TextBoxFrequencyValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxFrequencyValue.Text == "") // Prüft, ob das Textfeld leer ist
            {
                TextBoxFrequencyValue.Text = "0"; // Setzt den Standardwert auf 0
                TextBoxFrequencyValue.Foreground = Brushes.Red; // Markiert das Feld mit roter Schrift
            }
        }

        // Speichert den ausgewählten Harmonischen-Index in einer temporären Variablen und aktualisiert den Patienten, falls bereits erstellt
        private void ComboBoxHarmonics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            harmonicsTemp = ComboBoxHarmonics.SelectedIndex; // Speichert den Index der ausgewählten Harmonischen
            if (wasPatientCreated && parameter == MonitorConstants.Parameter.ECG)
                patient.ECGHarmonics = harmonicsTemp; // Aktualisiert die Harmonischen, wenn ECG als Parameter aktiv ist
        }

        // Speichert den aktuellen Amplitudenwert und aktualisiert den Patienten je nach Parameter
        private void SliderAmplitudeValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            amplitudeValue = SliderAmplitudeValue.Value; // Speichert die Amplitude in der temporären Variablen
            if (wasPatientCreated) // Prüft, ob bereits ein Patient erstellt wurde
            {
                switch (parameter) // Aktualisiert die Amplitude basierend auf dem aktuellen Parameter
                {
                    case MonitorConstants.Parameter.ECG:
                        patient.ECGAmplitude = SliderAmplitudeValue.Value;
                        break;
                    case MonitorConstants.Parameter.EMG:
                        patient.EMGAmplitude = SliderAmplitudeValue.Value;
                        break;
                    case MonitorConstants.Parameter.EEG:
                        patient.EEGAmplitude = SliderAmplitudeValue.Value;
                        break;
                    case MonitorConstants.Parameter.Respiration:
                        patient.RespirationAmplitude = SliderAmplitudeValue.Value;
                        break;
                }
            }
        }


        private void buttonCreatePatient_Click(object sender, RoutedEventArgs e)
        {
            List<string> missingFields = new List<string>(); // Liste zur Speicherung fehlender Felder

            // Überprüfung der Felder und Hinzufügen der fehlenden Informationen zur Liste
            if (string.IsNullOrWhiteSpace(PatientNameTextBox.Text) || PatientNameTextBox.Text == "Enter name here")
                missingFields.Add("Patient Name"); // Patientennamen überprüfen

            if (!int.TryParse(PatientAgeTextBox.Text, out _))
                missingFields.Add("Patient Age"); // Patientenalter überprüfen

            if (!DatePickerDate.SelectedDate.HasValue)
                missingFields.Add("Date of Study"); // Studien-Datum überprüfen

            if (ComboBoxClinic.SelectedIndex == -1)
                missingFields.Add("Clinic"); // Klinik überprüfen

            if (RadioButtonAmbulatory.IsChecked != true && RadioButtonStationary.IsChecked != true)
                missingFields.Add("Patient Type (Ambulatory or Stationary)"); // Patienten-Typ überprüfen

            if (RadioButtonStationary.IsChecked == true &&
                (string.IsNullOrWhiteSpace(TextBoxRoomNumber.Text) || TextBoxRoomNumber.Text == "Enter room number"))
                missingFields.Add("Room Number (for Stationary Patients)"); // Zimmernummer für stationäre Patienten prüfen

            // Falls Felder fehlen, zeige eine Nachricht und beende die Methode
            if (missingFields.Count > 0)
            {
                string message = "Please fill in the following required fields:\n" + string.Join("\n", missingFields);
                MessageBox.Show(message, "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Methode verlassen, wenn Pflichtfelder fehlen
            }

            // Überprüfung, ob ein Patient mit denselben Informationen bereits existiert
            bool patientExists = database.Data.Any(p =>
                p.PatientName == patientNameTemp &&
                p.Age == patientAgeTemp &&
                p.DateOfStudy == dateTemp &&
                p.Clinic == clinic &&
                ((p is StationaryPatient sp && sp.RoomNumber == roomNumberTemp && RadioButtonStationary.IsChecked == true) ||
                 (p is Patient && RadioButtonAmbulatory.IsChecked == true)));

            if (patientExists)
            {
                MessageBox.Show("A patient with the same information already exists. Please check the details and try again.",
                    "Duplicate Patient", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Abbruch, wenn bereits ein identischer Patient existiert
            }

            // Neuen Patienten erstellen
            if (RadioButtonAmbulatory.IsChecked == true) // Erstellen eines ambulanten Patienten
            {
                clinic = (MonitorConstants.clinic)ComboBoxClinic.SelectedIndex;
                patient = new Patient(patientNameTemp, dateTemp, patientAgeTemp, amplitudeValue, frequencyTemp, harmonicsTemp, clinic);
                database.AddPatient(patient); // Patient der Datenbank hinzufügen
                wasPatientCreated = true;
                isDatabaseSaved = false;
                buttonStartSimulation.IsEnabled = true;
                ComboBoxClinicSort.SelectedIndex = -1;
                RadioButtonDataBase.IsChecked = true;

                if (wasPatientCreated)
                {
                    // Zurücksetzen der Eingabefelder und UI-Elemente
                    SliderAmplitudeValue.Value = 0;
                    TextBoxFrequencyValue.Text = "0";
                    TextBoxLowAlarmValue.Text = "0";
                    TextBoxHighAlarmValue.Text = "0";
                    ComboBoxParameters.SelectedIndex = 0;
                    ComboBoxHarmonics.SelectedIndex = 0;
                }

                HighlightActivePatient(patient); // Aktiven Patienten hervorheben
                displayDatabase(); // Datenbankanzeige aktualisieren
                MessageBox.Show("Patient " + patientNameTemp + " was created!"); // Bestätigung anzeigen
            }
            else if (RadioButtonStationary.IsChecked == true) // Erstellen eines stationären Patienten
            {
                clinic = (MonitorConstants.clinic)ComboBoxClinic.SelectedIndex;
                stationaryPatient = new StationaryPatient(patientNameTemp, dateTemp, patientAgeTemp, amplitudeValue, frequencyTemp, harmonicsTemp, clinic, roomNumberTemp);
                database.AddPatient(stationaryPatient); // Stationären Patienten der Datenbank hinzufügen
                wasPatientCreated = true;
                isDatabaseSaved = false;
                buttonStartSimulation.IsEnabled = true;
                ComboBoxClinicSort.SelectedIndex = -1;
                RadioButtonDataBase.IsChecked = true;
                patient = stationaryPatient;

                if (wasPatientCreated)
                {
                    // Zurücksetzen der Eingabefelder und UI-Elemente
                    SliderAmplitudeValue.Value = 0;
                    TextBoxFrequencyValue.Text = "0";
                    TextBoxLowAlarmValue.Text = "0";
                    TextBoxHighAlarmValue.Text = "0";
                    ComboBoxParameters.SelectedIndex = 0;
                    ComboBoxHarmonics.SelectedIndex = 0;
                }

                HighlightActivePatient(patient); // Aktiven Patienten hervorheben
                displayDatabase(); // Datenbankanzeige aktualisieren
                MessageBox.Show("Stationary Patient " + patientNameTemp + " was created!"); // Bestätigung anzeigen
            }
        }

        private void buttonStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            HighlightActivePatient(patient); // Aktiven Patienten in der Datenbank hervorheben
            displayDatabase(); // Datenbankanzeige aktualisieren

            timerStarted = true; // Flag setzen, dass der Timer gestartet wurde
                                 // Aktivierung der relevanten Steuerelemente
            SliderAmplitudeValue.IsEnabled = true;
            TextBoxFrequencyValue.IsEnabled = true;
            ComboBoxHarmonics.IsEnabled = true;
            ComboBoxParameters.IsEnabled = true;
            ButtonLoadImage.IsEnabled = true;
            TextBoxHighAlarmValue.IsEnabled = true;
            TextBoxLowAlarmValue.IsEnabled = true;
            RadioButtonParameter.IsEnabled = true;

            if (RadioButtonDataBase.IsChecked == true) return; // Falls Datenbankmodus aktiv, Timer nicht starten
            timer.Start(); // Timer starten
        }

        private void buttonQuit_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop(); // Timer stoppen, um alle Hintergrundprozesse zu beenden
        }

        private void ComboBoxParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isValidationEnabled = false; // Validierung vorübergehend deaktivieren

            // Ausgewählten Parameter aktualisieren
            parameter = (MonitorConstants.Parameter)ComboBoxParameters.SelectedIndex;
            if (parameter == MonitorConstants.Parameter.ECG)
            {
                ComboBoxHarmonics.IsEnabled = true; // Harmonics nur für ECG aktivieren
            }
            else
            {
                ComboBoxHarmonics.IsEnabled = false; // Für andere Parameter deaktivieren
            }

            if (wasPatientCreated) // Falls ein Patient bereits erstellt wurde
            {
                switch (parameter) // Je nach ausgewähltem Parameter unterschiedliche Werte setzen
                {
                    case MonitorConstants.Parameter.ECG:
                        SliderAmplitudeValue.Value = patient.ECGAmplitude;
                        TextBoxFrequencyValue.Text = patient.ECGFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = patient.ECGHarmonics;
                        if (patient.ECGFrequency == 0.0) { patient.ECGFrequency = frequencyTemp; }

                        patient.displayLowAlarm(parameter, patient.ECGFrequency, patient.ECGLowAlarm);
                        patient.displayHighAlarm(parameter, patient.ECGFrequency, patient.ECGHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.ECGLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.ECGHighAlarmString;
                        TextBoxLowAlarmValue.Text = patient.ECGLowAlarm.ToString();
                        TextBoxHighAlarmValue.Text = patient.ECGHighAlarm.ToString();
                        break;

                    case MonitorConstants.Parameter.EMG:
                        SliderAmplitudeValue.Value = patient.EMGAmplitude;
                        TextBoxFrequencyValue.Text = patient.EMGFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = -1;
                        if (patient.EMGFrequency == 0.0) { patient.EMGFrequency = frequencyTemp; }

                        patient.displayLowAlarm(parameter, patient.EMGFrequency, patient.EMGLowAlarm);
                        patient.displayHighAlarm(parameter, patient.EMGFrequency, patient.EMGHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.EMGLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.EMGHighAlarmString;
                        TextBoxLowAlarmValue.Text = patient.EMGLowAlarm.ToString();
                        TextBoxHighAlarmValue.Text = patient.EMGHighAlarm.ToString();
                        break;

                    case MonitorConstants.Parameter.EEG:
                        SliderAmplitudeValue.Value = patient.EEGAmplitude;
                        TextBoxFrequencyValue.Text = patient.EEGFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = -1;
                        if (patient.EEGFrequency == 0.0) { patient.EEGFrequency = frequencyTemp; }

                        patient.displayLowAlarm(parameter, patient.EEGFrequency, patient.EEGLowAlarm);
                        patient.displayHighAlarm(parameter, patient.EEGFrequency, patient.EEGHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.EEGLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.EEGHighAlarmString;
                        TextBoxLowAlarmValue.Text = patient.EEGLowAlarm.ToString();
                        TextBoxHighAlarmValue.Text = patient.EEGHighAlarm.ToString();
                        break;

                    case MonitorConstants.Parameter.Respiration:
                        SliderAmplitudeValue.Value = patient.RespirationAmplitude;
                        TextBoxFrequencyValue.Text = patient.RespirationFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = -1;
                        if (patient.RespirationFrequency == 0.0) { patient.RespirationFrequency = frequencyTemp; }

                        patient.displayLowAlarm(parameter, patient.RespirationFrequency, patient.RespirationLowAlarm);
                        patient.displayHighAlarm(parameter, patient.RespirationFrequency, patient.RespirationHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.RespirationLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.RespirationHighAlarmString;
                        TextBoxLowAlarmValue.Text = patient.RespirationLowAlarm.ToString();
                        TextBoxHighAlarmValue.Text = patient.RespirationHighAlarm.ToString();
                        break;
                }

                // GUI-Werte in temporäre Variablen kopieren
                patientNameTemp = PatientNameTextBox.Text;
                int.TryParse(PatientAgeTextBox.Text, out patientAgeTemp);
                dateTemp = DatePickerDate.SelectedDate ?? DateTime.Now;
                double.TryParse(TextBoxLowAlarmValue.Text, out lowAlarmTemp);
                double.TryParse(TextBoxHighAlarmValue.Text, out highAlarmTemp);
                roomNumberTemp = TextBoxRoomNumber.Text;
                clinic = (MonitorConstants.clinic)ComboBoxClinic.SelectedIndex;
            }

            isValidationEnabled = true; // Validierung wieder aktivieren
        }


        private void ComboBoxParameters_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Cast des Senders zu ComboBox
            ComboBox combo = sender as ComboBox;

            // Überprüfen, ob die ComboBox aktiviert wurde
            if (combo.IsEnabled)
            {
                // Eventhandler hinzufügen, wenn aktiviert
                combo.SelectionChanged += ComboBoxParameters_SelectionChanged;
            }
            else
            {
                // Eventhandler entfernen, wenn deaktiviert
                combo.SelectionChanged -= ComboBoxParameters_SelectionChanged;
            }
        }

        private void TextBoxLowAlarmValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Validierung deaktiviert? Falls ja, Methode verlassen
            if (!isValidationEnabled) return;

            // Versuch, den eingegebenen Wert zu parsen
            if (double.TryParse(TextBoxLowAlarmValue.Text, out double parsedLowAlarm))
            {
                // Prüfen, ob der Wert außerhalb des zulässigen Bereichs liegt
                if (parsedLowAlarm < 0 || parsedLowAlarm > 150)
                {
                    // Fehlermeldung anzeigen und Wert zurücksetzen
                    MessageBox.Show("Low Alarm must be between 0 and 150 Hz.", "Invalid Low Alarm", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TextBoxLowAlarmValue.Text = lowAlarmTemp.ToString();
                }
                // Prüfen, ob der Low Alarm größer als der High Alarm ist
                else if (parsedLowAlarm > highAlarmTemp)
                {
                    // Fehlermeldung anzeigen und Wert zurücksetzen
                    MessageBox.Show("Low Alarm must be lower than High Alarm.", "Invalid Low Alarm", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TextBoxLowAlarmValue.Text = lowAlarmTemp.ToString();
                }
                else
                {
                    // Gültigen Wert speichern
                    lowAlarmTemp = parsedLowAlarm;

                    // Falls bereits ein Patient erstellt wurde, den Wert aktualisieren
                    if (wasPatientCreated)
                    {
                        switch (parameter)
                        {
                            case MonitorConstants.Parameter.ECG:
                                patient.ECGLowAlarm = lowAlarmTemp;
                                patient.displayLowAlarm(parameter, patient.ECGFrequency, patient.ECGLowAlarm);
                                TextBlockDisplayLowAlarm.Text = patient.ECGLowAlarmString;
                                break;
                            case MonitorConstants.Parameter.EMG:
                                patient.EMGLowAlarm = lowAlarmTemp;
                                patient.displayLowAlarm(parameter, patient.EMGFrequency, patient.EMGLowAlarm);
                                TextBlockDisplayLowAlarm.Text = patient.EMGLowAlarmString;
                                break;
                            case MonitorConstants.Parameter.EEG:
                                patient.EEGLowAlarm = lowAlarmTemp;
                                patient.displayLowAlarm(parameter, patient.EEGFrequency, patient.EEGLowAlarm);
                                TextBlockDisplayLowAlarm.Text = patient.EEGLowAlarmString;
                                break;
                            case MonitorConstants.Parameter.Respiration:
                                patient.RespirationLowAlarm = lowAlarmTemp;
                                patient.displayLowAlarm(parameter, patient.RespirationFrequency, patient.RespirationLowAlarm);
                                TextBlockDisplayLowAlarm.Text = patient.RespirationLowAlarmString;
                                break;
                        }
                    }
                }
            }
        }

        private void TextBoxHighAlarmValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Validierung deaktiviert? Falls ja, Methode verlassen
            if (!isValidationEnabled) return;

            // Versuch, den eingegebenen Wert zu parsen
            if (double.TryParse(TextBoxHighAlarmValue.Text, out double parsedHighAlarm))
            {
                // Prüfen, ob der Wert außerhalb des zulässigen Bereichs liegt
                if (parsedHighAlarm < 0 || parsedHighAlarm > 150)
                {
                    // Fehlermeldung anzeigen und Wert zurücksetzen
                    MessageBox.Show("High Alarm must be between 0 and 150 Hz.", "Invalid High Alarm", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TextBoxHighAlarmValue.Text = highAlarmTemp.ToString();
                }
                // Prüfen, ob der High Alarm kleiner als der Low Alarm ist
                else if (parsedHighAlarm < lowAlarmTemp)
                {
                    // Fehlermeldung anzeigen und Wert zurücksetzen
                    MessageBox.Show("High Alarm must be greater than Low Alarm.", "Invalid High Alarm", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TextBoxHighAlarmValue.Text = highAlarmTemp.ToString();
                }
                else
                {
                    // Gültigen Wert speichern
                    highAlarmTemp = parsedHighAlarm;

                    // Falls bereits ein Patient erstellt wurde, den Wert aktualisieren
                    if (wasPatientCreated)
                    {
                        switch (parameter)
                        {
                            case MonitorConstants.Parameter.ECG:
                                patient.ECGHighAlarm = highAlarmTemp;
                                patient.displayHighAlarm(parameter, patient.ECGFrequency, patient.ECGHighAlarm);
                                TextBlockDisplayHighAlarm.Text = patient.ECGHighAlarmString;
                                break;
                            case MonitorConstants.Parameter.EMG:
                                patient.EMGHighAlarm = highAlarmTemp;
                                patient.displayHighAlarm(parameter, patient.EMGFrequency, patient.EMGHighAlarm);
                                TextBlockDisplayHighAlarm.Text = patient.EMGHighAlarmString;
                                break;
                            case MonitorConstants.Parameter.EEG:
                                patient.EEGHighAlarm = highAlarmTemp;
                                patient.displayHighAlarm(parameter, patient.EEGFrequency, patient.EEGHighAlarm);
                                TextBlockDisplayHighAlarm.Text = patient.EEGHighAlarmString;
                                break;
                            case MonitorConstants.Parameter.Respiration:
                                patient.RespirationHighAlarm = highAlarmTemp;
                                patient.displayHighAlarm(parameter, patient.RespirationFrequency, patient.RespirationHighAlarm);
                                TextBlockDisplayHighAlarm.Text = patient.RespirationHighAlarmString;
                                break;
                        }
                    }
                }
            }
        }

        private void TextBoxLowAlarmValue_LostFocus(object sender, RoutedEventArgs e)
        {
            // Wenn das Textfeld leer ist, wird der Standardwert "0" gesetzt und die Schrift rot gefärbt
            if (TextBoxLowAlarmValue.Text == "")
            {
                TextBoxLowAlarmValue.Text = "0";
                TextBoxLowAlarmValue.Foreground = Brushes.Red;
            }
        }

        private void TextBoxLowAlarmValue_GotFocus(object sender, RoutedEventArgs e)
        {
            // Beim Fokussieren des Textfeldes wird der Text gelöscht und die Schrift schwarz gefärbt
            TextBoxLowAlarmValue.Text = "";
            TextBoxLowAlarmValue.Foreground = Brushes.Black;
        }

        private void TextBoxHighAlarmValue_LostFocus(object sender, RoutedEventArgs e)
        {
            // Wenn das Textfeld leer ist, wird der Standardwert "0" gesetzt und die Schrift rot gefärbt
            if (TextBoxHighAlarmValue.Text == "")
            {
                TextBoxHighAlarmValue.Text = "0";
                TextBoxHighAlarmValue.Foreground = Brushes.Red;
            }
        }

        private void TextBoxHighAlarmValue_GotFocus(object sender, RoutedEventArgs e)
        {
            // Beim Fokussieren des Textfeldes wird der Text gelöscht und die Schrift schwarz gefärbt
            TextBoxHighAlarmValue.Text = "";
            TextBoxHighAlarmValue.Foreground = Brushes.Black;
        }

        private void ButtonFourierTransformation_Click(object sender, RoutedEventArgs e)
        {
            // Setzt die Datenquelle der LineSeries auf null, um sie zurückzusetzen
            lineSeriesFFT.ItemsSource = null;

            // Überprüft, ob ein Patient existiert und ob es mindestens einen Messpunkt gibt
            if (patient != null && patient.SampleList.Count > 0)
            {
                // Größe des Arrays für die Fourier-Transformation (512 Punkte)
                int fftSize = 512;
                double[] sampleArray = new double[fftSize];

                // Berechnet die Anzahl der zu kopierenden Punkte (maximal 512)
                int pointsToCopy = Math.Min(patient.SampleList.Count, fftSize);

                // Kopiert die letzten vorhandenen Messpunkte in das Array, der Rest bleibt 0 (Zero-Padding)
                Array.Copy(patient.SampleList.Skip(patient.SampleList.Count - pointsToCopy).ToArray(), sampleArray, pointsToCopy);

                // Erstellt ein Spektrum-Objekt und führt die Fourier-Transformation durch
                Spektrum spektrum = new Spektrum(fftSize);
                double[] frequencySpectrum = spektrum.FFT(sampleArray, fftSize);

                // Erstellt eine Liste von Key-Value-Paaren für die Frequenzdaten
                ObservableCollection<KeyValuePair<int, double>> frequencyDataPoints = new ObservableCollection<KeyValuePair<int, double>>();
                double samplingRate = 6000; // Sampling-Rate in Hz
                for (int i = 0; i < frequencySpectrum.Length; i++)
                {
                    double frequency = i * (samplingRate / fftSize); // Berechnet die Frequenz für jeden Punkt
                    frequencyDataPoints.Add(new KeyValuePair<int, double>((int)frequency, frequencySpectrum[i]));
                }

                // Setzt die Datenquelle der LineSeries auf die berechneten Frequenzdaten
                lineSeriesFFT.ItemsSource = frequencyDataPoints;
            }
        }


        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            // Timer stoppen
            timer.Stop();

            _ = MessageBox.Show("Valid name-format for image files: BASE**.ext\n- BASE is a arbitary string\n- ** are two digits\n- .ext is the image format");

            // Öffne eine Datei, um den Ordnerpfad zu ermitteln
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Hole den Ordnerpfad der ausgewählten Datei
                string folderPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                // Lade alle Bilddateien im Ordner und sortiere sie nach Zahlen im Dateinamen
                List<string> imagePaths = System.IO.Directory.GetFiles(folderPath, "*.*")
                    .Where(file => file.EndsWith(".bmp") || file.EndsWith(".jpg") || file.EndsWith(".png"))
                    .OrderBy(file =>
                    {
                // Extrahiere die Nummer aus dem Dateinamen
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                        int number = ExtractNumber(fileName);
                        return number; // Sortiere nach der extrahierten Nummer
            })
                    .ToList();

                // Lade die Bilder in die MRImaging-Klasse
                if (imagePaths.Count > 0)
                {
                    mrImaging.ClearImages(); // Alte Bilder löschen (falls vorhanden)

                    foreach (string path in imagePaths)
                    {
                        mrImaging.LoadImage(path);
                    }

                    // Zeige das erste Bild
                    BitmapImage currentImage = mrImaging.ImageList[0];
                    if (currentImage != null)
                    {
                        ImageBrush myImageBrush = new ImageBrush();
                        myImageBrush.ImageSource = currentImage;
                        RectangleImage.Fill = myImageBrush;
                    }

                    // Buttons aktivieren
                    ButtonNextImage.IsEnabled = imagePaths.Count > 1;
                    ButtonPreviousImage.IsEnabled = imagePaths.Count > 1;
                    TextBoxMaxImages.IsEnabled = true;
                    TextBoxMaxImages.Text = imagePaths.Count.ToString();
                }
                else
                {
                    MessageBox.Show("No valid images found in the selected folder.");
                }
            }
        }

        // Hilfsmethode zum Extrahieren einer Zahl aus einem Dateinamen
        private int ExtractNumber(string fileName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(fileName, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0; // Falls keine Zahl gefunden wird, Rückgabewert 0
        }

        private void ButtonPreviousImage_Click(object sender, RoutedEventArgs e)
        {
            // Ruft das vorherige Bild aus der Liste ab
            BitmapImage previousImage = mrImaging.BackImage();

            // Prüft, ob ein vorheriges Bild vorhanden ist
            if (previousImage != null)
            {
                // Erstellt einen neuen ImageBrush und setzt das Bild als Quelle
                ImageBrush myImageBrush = new ImageBrush();
                myImageBrush.ImageSource = previousImage;
                RectangleImage.Fill = myImageBrush;
            }
            else
            {
                // Zeigt eine Nachricht an, wenn kein vorheriges Bild verfügbar ist
                MessageBox.Show("No previous images available!");
            }
        }

        private void ButtonNextImage_Click(object sender, RoutedEventArgs e)
        {
            // Ruft das nächste Bild aus der Liste ab
            BitmapImage nextImage = mrImaging.ForwardImage();

            // Prüft, ob ein nächstes Bild vorhanden ist
            if (nextImage != null)
            {
                // Erstellt einen neuen ImageBrush und setzt das Bild als Quelle
                ImageBrush myImageBrush = new ImageBrush();
                myImageBrush.ImageSource = nextImage;
                RectangleImage.Fill = myImageBrush;
            }
            else
            {
                // Zeigt eine Nachricht an, wenn kein nächstes Bild verfügbar ist
                MessageBox.Show("No next images available!");
            }
        }

        private void TextBoxMaxImages_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Versucht, den eingegebenen Wert in einen Integer zu konvertieren
            int.TryParse(TextBoxMaxImages.Text, out int maxImagesTemp);

            // Prüft, ob der eingegebene Wert kleiner oder gleich der Anzahl der geladenen Bilder ist
            if (maxImagesTemp <= mrImaging.ImageList.Count)
            {
                // Setzt die maximale Anzahl der anzuzeigenden Bilder
                mrImaging.MaxImages = maxImagesTemp;

                // Falls Bilder vorhanden sind, zeigt das erste Bild an
                if (mrImaging.ImageList.Count > 0)
                {
                    ImageBrush myImageBrush = new ImageBrush();
                    myImageBrush.ImageSource = mrImaging.ImageList[0];
                    RectangleImage.Fill = myImageBrush;
                }
            }
            else
            {
                // Zeigt eine Warnung an, wenn der eingegebene Wert die Anzahl der hochgeladenen Bilder überschreitet
                MessageBox.Show("The maximum amount of images can not be higher than the total images you uploaded!");
                TextBoxMaxImages.Text = "0"; // Setzt den Wert zurück
            }
        }

        private void TextBoxMaxImages_GotFocus(object sender, RoutedEventArgs e)
        {
            // Löscht den Textinhalt beim Fokussieren und setzt die Schriftfarbe auf Schwarz
            TextBoxMaxImages.Text = "";
            TextBoxMaxImages.Foreground = Brushes.Black;
        }

        private void TextBoxMaxImages_LostFocus(object sender, RoutedEventArgs e)
        {
            // Setzt den Standardwert und die Schriftfarbe auf Rot, falls das Feld leer bleibt
            if (TextBoxMaxImages.Text == "")
            {
                TextBoxMaxImages.Text = "0";
                TextBoxMaxImages.Foreground = Brushes.Red;
            }
        }

        private void TextBoxMaxImages_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verhindert die Eingabe von Zeichen, die keine Zahlen sind
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBoxRoomNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            // Löscht den Textinhalt beim Fokussieren und setzt die Schriftfarbe auf Schwarz
            TextBoxRoomNumber.Text = "";
            TextBoxRoomNumber.Foreground = Brushes.Black;
        }

        private void TextBoxRoomNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            // Setzt den Standardtext und die Schriftfarbe auf Rot, falls das Feld leer bleibt
            if (TextBoxRoomNumber.Text == "")
            {
                TextBoxRoomNumber.Text = "Enter room number";
                TextBoxRoomNumber.Foreground = Brushes.Red;
            }
        }

        private void TextBoxRoomNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verhindert die Eingabe von nicht-numerischen Zeichen
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBoxRoomNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Aktualisiert die temporäre Variable für die Zimmernummer
            int.TryParse(TextBoxRoomNumber.Text, out int roomNumberTempTemp);
            roomNumberTemp = roomNumberTempTemp.ToString();
        }

        private void ComboBoxClinic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Speichert die ausgewählte Klinik als temporäre Variable
            clinic = (MonitorConstants.clinic)ComboBoxClinic.SelectedIndex;
        }

        private void RadioButtonAmbulatory_Checked(object sender, RoutedEventArgs e)
        {
            // Deaktiviert das Feld für die Zimmernummer bei ambulanten Patienten
            TextBoxRoomNumber.IsEnabled = false;
            TextBoxRoomNumber.Text = " /";
        }

        private void RadioButtonStationary_Checked(object sender, RoutedEventArgs e)
        {
            // Aktiviert das Feld für die Zimmernummer bei stationären Patienten
            TextBoxRoomNumber.IsEnabled = true;
            TextBoxRoomNumber.Text = "Enter room number";
        }

        private void displayDatabase()
        {
            // Löscht die angezeigten Daten aus der DataGrid-Anzeige
            PatientData.Items.Clear();

            // Fügt alle Patienten aus der Datenbank zur Anzeige hinzu
            foreach (var patient in database.Data)
            {
                if (patient is StationaryPatient stationaryPatient)
                {
                    // Fügt stationäre Patienten mit entsprechenden Spalten hinzu
                    PatientData.Items.Add(new
                    {
                        ColumnID = stationaryPatient.ID.ToString(),
                        ColumnName = stationaryPatient.PatientName,
                        ColumnAge = stationaryPatient.Age,
                        ColumnClinic = stationaryPatient.Clinictype,
                        ColumnType = "Stationary",
                        ColumnRoom = stationaryPatient.RoomNumber,
                        ColumnDate = stationaryPatient.DateOfStudy.ToString("dd.MM.yyyy")
                    });
                }
                else if (patient is Patient patient1)
                {
                    // Fügt ambulante Patienten mit entsprechenden Spalten hinzu
                    PatientData.Items.Add(new
                    {
                        ColumnID = patient1.ID.ToString(),
                        ColumnName = patient1.PatientName,
                        ColumnAge = patient1.Age,
                        ColumnClinic = patient1.Clinictype,
                        ColumnType = "Ambulatory",
                        ColumnRoom = " / ",
                        ColumnDate = patient1.DateOfStudy.ToString("dd.MM.yyyy")
                    });
                }
            }
        }

        private void RadioButtonParameter_Checked(object sender, RoutedEventArgs e)
        {
            // Blendet die Datenbankanzeige aus und startet den Timer, wenn die Simulation aktiv ist
            PatientData.Visibility = Visibility.Hidden;
            if (timerStarted) timer.Start();
        }

        private void RadioButtonDataBase_Checked(object sender, RoutedEventArgs e)
        {
            // Stoppt den Timer und zeigt die Datenbank an, wenn der Radiobutton ausgewählt ist
            if (timerStarted)
            {
                timer.Stop();
                PatientData.Visibility = Visibility.Visible;
            }

            HighlightActivePatient(patient);
        }

        private void ButtonLoadDB_Click(object sender, RoutedEventArgs e)
        {
            if (!isDatabaseSaved)
            {
                // Zeigt eine Warnung an, falls die Datenbank nicht gespeichert ist
                MessageBoxResult result = MessageBox.Show(
                    "Sie haben die aktuelle Datenbank nicht gespeichert, wollen Sie wirklich fortfahren?",
                    "Alarm!",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                // Abbruch, wenn der Benutzer nicht fortfahren möchte
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            string file = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.text|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Speichert den Dateipfad der ausgewählten Datei
                file = openFileDialog.FileName;
            }

            // Lädt die Datenbank aus der Datei
            database.LoadData(file);

            if (database.Data.Count > 0)
            {
                wasPatientCreated = true;
                buttonStartSimulation.IsEnabled = true;

                // Wählt den zuletzt hinzugefügten Patienten aus
                patient = database.Data.Last();

                // Setzt den Parameter auf ECG und aktualisiert die GUI
                parameter = MonitorConstants.Parameter.ECG;
                ComboBoxParameters.SelectedIndex = (int)MonitorConstants.Parameter.ECG;
                UpdateGUIWithPatientData(patient);

                // Hebt den aktiven Patienten hervor
                HighlightActivePatient(patient);
            }

            isDatabaseSaved = true;
            displayDatabase();
        }

        private void UpdateGUIWithPatientData(Patient selectedPatient)
        {
            if (selectedPatient != null)
            {
                isValidationEnabled = false;

                // Aktualisiert die GUI mit den Daten des ausgewählten Patienten
                SliderAmplitudeValue.Value = selectedPatient.ECGAmplitude;
                TextBoxFrequencyValue.Text = selectedPatient.ECGFrequency.ToString();

                if (parameter == MonitorConstants.Parameter.ECG)
                {
                    ComboBoxHarmonics.SelectedIndex = selectedPatient.ECGHarmonics;
                }
                else
                {
                    ComboBoxHarmonics.SelectedIndex = -1;
                }

                TextBoxLowAlarmValue.Text = selectedPatient.ECGLowAlarm.ToString();
                TextBoxHighAlarmValue.Text = selectedPatient.ECGHighAlarm.ToString();
                PatientNameTextBox.Text = selectedPatient.PatientName;
                PatientAgeTextBox.Text = selectedPatient.Age.ToString();
                DatePickerDate.Text = selectedPatient.DateOfStudy.ToString();

                // Unterscheidet zwischen stationärem und ambulantem Patienten
                if (selectedPatient is StationaryPatient stationaryPatient)
                {
                    RadioButtonStationary.IsChecked = true;
                    TextBoxRoomNumber.Text = stationaryPatient.RoomNumber;
                }
                else
                {
                    RadioButtonAmbulatory.IsChecked = true;
                }

                // Setzt den Kliniktyp im Dropdown
                ComboBoxClinic.SelectedIndex = (int)selectedPatient.Clinic;

                // Kopiert die Werte in temporäre Variablen
                patientNameTemp = PatientNameTextBox.Text;
                int.TryParse(PatientAgeTextBox.Text, out patientAgeTemp);
                dateTemp = DatePickerDate.SelectedDate ?? DateTime.Now;
                double.TryParse(TextBoxLowAlarmValue.Text, out lowAlarmTemp);
                double.TryParse(TextBoxHighAlarmValue.Text, out highAlarmTemp);
                roomNumberTemp = TextBoxRoomNumber.Text;
                clinic = (MonitorConstants.clinic)ComboBoxClinic.SelectedIndex;

                isValidationEnabled = true;
            }
        }

        private void ButtonSaveDB_Click(object sender, RoutedEventArgs e)
        {
            // Öffnet einen Dialog zum Speichern der Datenbank
            string file = string.Empty;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.text|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                file = saveFileDialog.FileName;
            }

            isDatabaseSaved = true;
            database.SaveData(file);
        }

        private void ComboBoxClinicSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Erstellt einen PatientComparer und sortiert die Datenbank
            PatientComparer pc = new PatientComparer();
            pc.CA = (MonitorConstants.compareAfter)ComboBoxClinicSort.SelectedIndex;

            if (database != null)
            {
                database.Data.Sort(pc);
                displayDatabase();
            }
        }

        private void HighlightActivePatient(Patient activePatient)
        {
            if (activePatient == null) return;

            // Speichert die ID des aktiven Patienten
            Guid activePatientId = activePatient.ID;
            _activePatientId = activePatientId;

            // Durchläuft alle Einträge in der Datenbank und hebt den aktiven Patienten hervor
            foreach (var item in PatientData.Items)
            {
                dynamic data = item;
                if (Guid.TryParse(data.ColumnID.ToString(), out Guid parsedID))
                {
                    DataGridRow row = PatientData.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (row != null)
                    {
                        row.Background = parsedID == activePatientId ? Brushes.LightGreen : Brushes.White;
                    }
                }
            }
        }

        private void PatientData_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Hebt die Zeile des aktiven Patienten hervor, wenn die Zeile geladen wird
            dynamic data = e.Row.Item;
            if (Guid.TryParse(data.ColumnID.ToString(), out Guid parsedID))
            {
                e.Row.Background = parsedID == _activePatientId ? Brushes.LightGreen : Brushes.White;
            }
        }

        private void ButtonQuit_Click_1(object sender, RoutedEventArgs e)
        {
            // Zeigt eine Bestätigungsnachricht an, bevor die Anwendung beendet wird
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to quit?",
                "Confirm Quit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                if (timer != null && timer.IsEnabled) timer.Stop();
                Application.Current.Shutdown();
            }
        }

        private void PatientData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Prüft, ob ein Patient ausgewählt wurde
            if (PatientData.SelectedItem != null)
            {
                dynamic selectedPatientData = PatientData.SelectedItem;
                Guid selectedPatientID = Guid.Parse(selectedPatientData.ColumnID);
                Patient selectedPatient = database.Data.FirstOrDefault(p => p.ID == selectedPatientID);

                if (selectedPatient != null)
                {
                    // Setzt den ausgewählten Patienten als aktiv und aktualisiert die GUI
                    patient = selectedPatient;
                    UpdateGUIWithPatientData(selectedPatient);
                    PatientData.ScrollIntoView(PatientData.SelectedItem);
                    PatientData.UpdateLayout();
                    HighlightActivePatient(selectedPatient);
                    displayDatabase();
                }
                else
                {
                    MessageBox.Show("Selected patient not found in the database.");
                }
            }
        }
    }
}
