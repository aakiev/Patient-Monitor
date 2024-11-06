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
using Microsoft.Win32;  //For OpenFileDialog




namespace PatientMonitor
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<KeyValuePair<int, double>> dataPoints;
        private DispatcherTimer timer;
        private int index = 0;
        Patient patient;
        MonitorConstants.Parameter parameter = MonitorConstants.Parameter.ECG;

        string patientNameTemp;
        int patientAgeTemp;
        DateTime dateTemp;
        double frequencyTemp = 0;
        int harmonicsTemp = 1;
        double amplitudeValue = 0;
        bool wasPatientCreated = false;

        public MainWindow()
        {
            InitializeComponent();
            dataPoints = new ObservableCollection<KeyValuePair<int, double>>();
            lineSeriesECG.ItemsSource = dataPoints; // Bind the series to the data points

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1); // Set timer to tick every 1 ms
            timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Real time calculation
            double currentTimeInSeconds = index / 6000.0; // 6000 perfect value for 50hz really beeing 50Hz

            // Generate datapoint
            if (patient != null)
            {
                dataPoints.Add(new KeyValuePair<int, double>(index++, patient.NextSample(currentTimeInSeconds, parameter)));
            }

            // Delete datapoints to clear the diagram
            if (dataPoints.Count > 200) // Max count of points
            {
                dataPoints.RemoveAt(0); // Delete last point
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
            if (textBox.Text == "")
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
            if (wasPatientCreated)
            {
                switch (parameter)
                {
                    case MonitorConstants.Parameter.ECG: patient.ECGFrequency = frequencyTemp; break;
                    case MonitorConstants.Parameter.EMG: patient.EMGFrequency = frequencyTemp; break;
                    case MonitorConstants.Parameter.EEG: patient.EEGFrequency = frequencyTemp; break;
                    case MonitorConstants.Parameter.Respiration: patient.RespirationFrequency = frequencyTemp; break;
                }
            }

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
            if (TextBoxFrequencyValue.Text == "")
            {
                TextBoxFrequencyValue.Text = "0";
                TextBoxFrequencyValue.Foreground = Brushes.Red;
            }

        }

        private void ComboBoxHarmonics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            harmonicsTemp = ComboBoxHarmonics.SelectedIndex;
            if (wasPatientCreated && parameter == MonitorConstants.Parameter.ECG) patient.ECGHarmonics = harmonicsTemp;
        }

        private void SliderAmplitudeValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            amplitudeValue = SliderAmplitudeValue.Value;
            if (wasPatientCreated)
            {
                switch (parameter)
                {
                    case MonitorConstants.Parameter.ECG: patient.ECGAmplitude = SliderAmplitudeValue.Value; break;
                    case MonitorConstants.Parameter.EMG: patient.EMGAmplitude = SliderAmplitudeValue.Value; break;
                    case MonitorConstants.Parameter.EEG: patient.EEGAmplitude = SliderAmplitudeValue.Value; break;
                    case MonitorConstants.Parameter.Respiration: patient.RespirationAmplitude = SliderAmplitudeValue.Value; break;
                }
            }
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
                MessageBox.Show("Fill all boxes!");
            }

        }

        private void buttonStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            SliderAmplitudeValue.IsEnabled = true;
            TextBoxFrequencyValue.IsEnabled = true;
            ComboBoxHarmonics.IsEnabled = true;
            ComboBoxParameters.IsEnabled = true;
            ButtonLoadImage.IsEnabled = true;
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


        private void ComboBoxParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            parameter = (MonitorConstants.Parameter)ComboBoxParameters.SelectedIndex;
            if (parameter == MonitorConstants.Parameter.ECG)
            {
                ComboBoxHarmonics.IsEnabled = true;
            }
            else
            {
                ComboBoxHarmonics.IsEnabled = false;
            }


            if (wasPatientCreated)
            {
                switch (parameter)
                {
                    case MonitorConstants.Parameter.ECG: SliderAmplitudeValue.Value = patient.ECGAmplitude;
                        TextBoxFrequencyValue.Text = patient.ECGFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = patient.ECGHarmonics;
                        if (patient.ECGFrequency == 0.0) { patient.ECGFrequency = frequencyTemp; }
                        break;
                    case MonitorConstants.Parameter.EMG: SliderAmplitudeValue.Value = patient.EMGAmplitude;
                        TextBoxFrequencyValue.Text = patient.EMGFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = -1;
                        if (patient.EMGFrequency == 0.0) { patient.EMGFrequency = frequencyTemp; }
                        break;
                    case MonitorConstants.Parameter.EEG: SliderAmplitudeValue.Value = patient.EEGAmplitude;
                        TextBoxFrequencyValue.Text = patient.EEGFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = -1;
                        if (patient.EEGFrequency == 0.0) { patient.EEGFrequency = frequencyTemp; }
                        break;
                    case MonitorConstants.Parameter.Respiration: SliderAmplitudeValue.Value = patient.RespirationAmplitude;
                        TextBoxFrequencyValue.Text = patient.RespirationFrequency.ToString();
                        ComboBoxHarmonics.SelectedIndex = -1;
                        if (patient.RespirationFrequency == 0.0) { patient.RespirationFrequency = frequencyTemp; }
                        break;
                }
            }
        }

        private void ComboBoxParameters_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo.IsEnabled)
            {
                combo.SelectionChanged += ComboBoxParameters_SelectionChanged;
            }
            else
            {
                combo.SelectionChanged -= ComboBoxParameters_SelectionChanged;
            }
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            //For the Project beeing exactly like Algorris, i include a timer stop here (In Case Algorri really wants it like that)
            timer.Stop();

            // Create an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.bmp)|*.bmp|All files (*.*)|*.*";

            // Show the dialog and check if the result is OK
            if (openFileDialog.ShowDialog() == true)
            {
                // Create a new BitmapImage
                BitmapImage bitmap = new BitmapImage();

                // Set the UriSource to load the image from the file
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Create an ImageBrush and set its ImageSource
                ImageBrush myImageBrush = new ImageBrush();
                myImageBrush.ImageSource = bitmap;

                // Assuming you have a Rectangle named "RectangleImage" in XAML
                RectangleImage.Fill = myImageBrush;
            }
        }

    }
}
