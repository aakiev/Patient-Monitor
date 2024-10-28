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


namespace PatientMonitor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<KeyValuePair<int, double>> dataPoints;
        private DispatcherTimer timer;
        private int index = 0;
        Patient patient;

        string patientNameTemp;
        int patientAgeTemp;
        DateTime dateTemp;
        double frequencyTemp = 0;
        int harmonicsTemp = 1;
        double amplitudeValue = 5;
        bool wasPatientCreated = false;

        public MainWindow()
        {
            InitializeComponent();
            dataPoints = new ObservableCollection<KeyValuePair<int, double>>();
            lineSeriesECG.ItemsSource = dataPoints; // Bind the series to the data points

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1); // Set timer to tick every ms
            timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Berechne die aktuelle Zeit in Sekunden
            double currentTimeInSeconds = index / 6000.0; // 6000 der perfekte Wert damit die Ausgabe wirklich echte ms sind

            // Erzeuge einen neuen Datenpunkt
            if (patient != null)
            {
                dataPoints.Add(new KeyValuePair<int, double>(index++, patient.NextSample(currentTimeInSeconds)));
            }

            // Entferne alte Punkte, um das Diagramm sauber zu halten
            if (dataPoints.Count > 200) // Maximale Anzahl der Punkte
            {
                dataPoints.RemoveAt(0); // Entferne den ältesten Punkt
            }

        }


        private void PatientNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Text = "";
            textBox.Foreground = Brushes.Black;
        }

        private void PatientNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(textBox.Text == "")
            {
                textBox.Text = "Enter name here";
                textBox.Foreground = Brushes.Red;
            }
            
        }

        private void PatientAgeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "")
            {
                textBox.Text = "Enter age here";
                textBox.Foreground = Brushes.Red;
            }

        }

        private void PatientAgeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
            
        }

        private void PatientAgeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(PatientAgeTextBox.Text, out int parsedage);
            patientAgeTemp = parsedage;

        }

        private void PatientNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            patientNameTemp = PatientNameTextBox.Text;
        }

        private void DatePickerDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dateTemp = DatePickerDate.SelectedDate.Value;
        }

        private void TextBoxFrequencyValue_TextChanged(object sender, TextChangedEventArgs e)
        {

            double.TryParse(TextBoxFrequencyValue.Text, out double parsedFrequency);
            frequencyTemp = parsedFrequency;
            if (wasPatientCreated) { patient.ECGFrequency = frequencyTemp; }

        }

        private void TextBoxFrequencyValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {   
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBoxFrequencyValue_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxFrequencyValue.Text = "";
            TextBoxFrequencyValue.Foreground = Brushes.Black;

        }

        private void TextBoxFrequencyValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if(TextBoxFrequencyValue.Text == "")
            {
                TextBoxFrequencyValue.Text = "0";
                TextBoxFrequencyValue.Foreground = Brushes.Red;
            }

        }

        private void ComboBoxHarmonics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            harmonicsTemp = ComboBoxHarmonics.SelectedIndex;
            if (wasPatientCreated) patient.ECGHarmonics = harmonicsTemp;
        }

        private void SliderAmplitudeValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            amplitudeValue = SliderAmplitudeValue.Value;
            if(wasPatientCreated) patient.ECGAmplitude = amplitudeValue;
        }

        private void buttonCreatePatient_Click(object sender, RoutedEventArgs e)
        {
            bool ageValid = int.TryParse(PatientAgeTextBox.Text, out int _);

            if (PatientNameTextBox.Text != "Enter name here" && !string.IsNullOrWhiteSpace(PatientNameTextBox.Text) && ageValid && DatePickerDate.SelectedDate.HasValue)
            {
                patient = new Patient(patientNameTemp, dateTemp, patientAgeTemp, amplitudeValue, frequencyTemp, harmonicsTemp);
                MessageBox.Show("Patient " + patientNameTemp + " was created!");
                wasPatientCreated = true;
                buttonUpdatePatient.IsEnabled = true;
                buttonStartSimulation.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Füllen Sie alle Felder aus!");
            }
            
        }

        private void buttonStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            SliderAmplitudeValue.IsEnabled = true;
            TextBoxFrequencyValue.IsEnabled = true;
            ComboBoxHarmonics.IsEnabled = true;
        }

        private void buttonQuit_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void buttonUpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Patient " + patient.PatientName + " was updatet!");
            patient.PatientName = patientNameTemp;
            patient.Age = patientAgeTemp;
            patient.DateOfStudy = dateTemp;
            patient.ECGAmplitude = amplitudeValue;
            patient.ECGFrequency = frequencyTemp;
            patient.ECGHarmonics = harmonicsTemp;
            
        }
    }
}
