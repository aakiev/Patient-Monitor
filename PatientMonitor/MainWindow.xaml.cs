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
        private MRImaging mrImaging = new MRImaging();

        string patientNameTemp;
        int patientAgeTemp;
        DateTime dateTemp;
        double frequencyTemp = 0;
        int harmonicsTemp = 1;
        double amplitudeValue = 0;
        double lowAlarmTemp = 0;
        double highAlarmTemp = 0;
        bool wasPatientCreated = false;

        public MainWindow()
        {
            InitializeComponent();
            dataPoints = new ObservableCollection<KeyValuePair<int, double>>();
            lineSeriesTime.ItemsSource = dataPoints; // Bind the series to the data points

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1); // Set timer to tick every 1 ms
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            displayTime();
        }

        private void displayTime()
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
                    case MonitorConstants.Parameter.ECG: patient.ECGFrequency = frequencyTemp;
                        patient.displayLowAlarm(parameter, patient.ECGFrequency, patient.ECGLowAlarm);
                        patient.displayHighAlarm(parameter, patient.ECGFrequency, patient.ECGHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.ECGLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.ECGHighAlarmString; break;
                    case MonitorConstants.Parameter.EMG: patient.EMGFrequency = frequencyTemp;
                        patient.displayLowAlarm(parameter, patient.EMGFrequency, patient.EMGLowAlarm);
                        patient.displayHighAlarm(parameter, patient.EMGFrequency, patient.EMGHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.EMGLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.EMGHighAlarmString; break;
                    case MonitorConstants.Parameter.EEG: patient.EEGFrequency = frequencyTemp;
                        patient.displayLowAlarm(parameter, patient.EEGFrequency, patient.EEGLowAlarm);
                        patient.displayHighAlarm(parameter, patient.EEGFrequency, patient.EEGHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.EEGLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.EEGHighAlarmString; break;
                    case MonitorConstants.Parameter.Respiration: patient.RespirationFrequency = frequencyTemp;
                        patient.displayLowAlarm(parameter, patient.RespirationFrequency, patient.RespirationLowAlarm);
                        patient.displayHighAlarm(parameter, patient.RespirationFrequency, patient.RespirationHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.RespirationLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.RespirationHighAlarmString; break;
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
            TextBoxHighAlarmValue.IsEnabled = true;
            TextBoxLowAlarmValue.IsEnabled = true;
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

                        patient.displayLowAlarm(parameter, patient.ECGFrequency, patient.ECGLowAlarm);
                        patient.displayHighAlarm(parameter, patient.ECGFrequency, patient.ECGHighAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.ECGLowAlarmString;
                        TextBlockDisplayHighAlarm.Text = patient.ECGHighAlarmString;
                        TextBoxLowAlarmValue.Text = patient.ECGLowAlarm.ToString();
                        TextBoxHighAlarmValue.Text = patient.ECGHighAlarm.ToString();
                        break;

                    case MonitorConstants.Parameter.EMG: SliderAmplitudeValue.Value = patient.EMGAmplitude;
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

                    case MonitorConstants.Parameter.EEG: SliderAmplitudeValue.Value = patient.EEGAmplitude;
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

                    case MonitorConstants.Parameter.Respiration: SliderAmplitudeValue.Value = patient.RespirationAmplitude;
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

        private void TextBoxLowAlarmValue_TextChanged(object sender, TextChangedEventArgs e)
        {

            double.TryParse(TextBoxLowAlarmValue.Text, out double parsedLowAlarm);
            lowAlarmTemp = parsedLowAlarm;

            if (wasPatientCreated)
            {
                switch (parameter)
                {
                    case MonitorConstants.Parameter.ECG: patient.ECGLowAlarm = lowAlarmTemp; 
                        patient.displayLowAlarm(parameter, patient.ECGFrequency, patient.ECGLowAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.ECGLowAlarmString; break;
                    case MonitorConstants.Parameter.EMG: patient.EMGLowAlarm = lowAlarmTemp; 
                        patient.displayLowAlarm(parameter, patient.EMGFrequency, patient.EMGLowAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.EMGLowAlarmString; break;
                    case MonitorConstants.Parameter.EEG: patient.EEGLowAlarm = lowAlarmTemp; 
                        patient.displayLowAlarm(parameter, patient.EEGFrequency, patient.EEGLowAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.EEGLowAlarmString; break;
                    case MonitorConstants.Parameter.Respiration: patient.RespirationLowAlarm = lowAlarmTemp; 
                        patient.displayLowAlarm(parameter, patient.RespirationFrequency, patient.RespirationLowAlarm);
                        TextBlockDisplayLowAlarm.Text = patient.RespirationLowAlarmString; break;
                }
            }
        }

        private void TextBoxHighAlarmValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(TextBoxHighAlarmValue.Text, out double parsedHighAlarm);
            highAlarmTemp = parsedHighAlarm;

            if (wasPatientCreated)
            {
                switch (parameter)
                {
                    case MonitorConstants.Parameter.ECG:
                        patient.ECGHighAlarm = highAlarmTemp;
                        patient.displayHighAlarm(parameter, patient.ECGFrequency, patient.ECGHighAlarm);
                        TextBlockDisplayHighAlarm.Text = patient.ECGHighAlarmString; break;
                    case MonitorConstants.Parameter.EMG:
                        patient.EMGHighAlarm = highAlarmTemp;
                        patient.displayHighAlarm(parameter, patient.EMGFrequency, patient.EMGHighAlarm);
                        TextBlockDisplayHighAlarm.Text = patient.EMGHighAlarmString; break;
                    case MonitorConstants.Parameter.EEG:
                        patient.EEGHighAlarm = highAlarmTemp;
                        patient.displayHighAlarm(parameter, patient.EEGFrequency, patient.EEGHighAlarm);
                        TextBlockDisplayHighAlarm.Text = patient.EEGHighAlarmString; break;
                    case MonitorConstants.Parameter.Respiration:
                        patient.RespirationHighAlarm = highAlarmTemp;
                        patient.displayHighAlarm(parameter, patient.RespirationFrequency, patient.RespirationHighAlarm);
                        TextBlockDisplayHighAlarm.Text = patient.RespirationHighAlarmString; break;
                }
            }
        }

        private void TextBoxLowAlarmValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxLowAlarmValue.Text == "")
            {
                TextBoxLowAlarmValue.Text = "0";
                TextBoxLowAlarmValue.Foreground = Brushes.Red;
            }
        }

        private void TextBoxLowAlarmValue_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxLowAlarmValue.Text = "";
            TextBoxLowAlarmValue.Foreground = Brushes.Black;
        }

        private void TextBoxHighAlarmValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxHighAlarmValue.Text == "")
            {
                TextBoxHighAlarmValue.Text = "0";
                TextBoxHighAlarmValue.Foreground = Brushes.Red;
            }
        }

        private void TextBoxHighAlarmValue_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxHighAlarmValue.Text = "";
            TextBoxHighAlarmValue.Foreground = Brushes.Black;
        }

        private void ButtonFourierTransformation_Click(object sender, RoutedEventArgs e)
        {
            lineSeriesFFT.ItemsSource = null;

            if (patient != null && patient.SampleList.Count >= 512)
            {
                // Letzte 512 Punkte mit Skip
                double[] sampleArray = patient.SampleList.Skip(patient.SampleList.Count - 512).ToArray();

                // Erstellung Spektrum-Objekt und Fourier-Transformation
                Spektrum spektrum = new Spektrum(sampleArray.Length);
                double[] frequencySpectrum = spektrum.FFT(sampleArray, sampleArray.Length);

                // Frequenzdaten binden an neue lineSeries
                ObservableCollection<KeyValuePair<int, double>> frequencyDataPoints = new ObservableCollection<KeyValuePair<int, double>>();
                double samplingRate = 6000;
                for (int i = 0; i < frequencySpectrum.Length; i++)
                {
                    double frequency = i * (samplingRate / sampleArray.Length); // Frequenz berechnen
                    frequencyDataPoints.Add(new KeyValuePair<int, double>((int)frequency, frequencySpectrum[i]));
                }

                //LineSeries für Frequenz aktualisieren
                lineSeriesFFT.ItemsSource = frequencyDataPoints;

            }
            else if (patient != null && patient.SampleList.Count < 512)
            {
                MessageBox.Show("Not enough data points available for Fourier transform. At least 512 points are required.");
            }
            else
            {
                MessageBox.Show("No patient data available for frequency display.");
            }
        }
        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            // Stop the time
            timer.Stop();

            // Create an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

            // Show the dialog and check if the result is OK
            if (openFileDialog.ShowDialog() == true)
            {

                // Load the image into the MRImaging class
                mrImaging.LoadImage(openFileDialog.FileName);

                // Display the currently loaded image
                BitmapImage currentImage = mrImaging.GetCurrentImage();
                if (currentImage != null)
                {
                    ImageBrush myImageBrush = new ImageBrush();
                    myImageBrush.ImageSource = currentImage;
                    RectangleImage.Fill = myImageBrush;
                }
            }

            ButtonNextImage.IsEnabled = true;
            ButtonPreviousImage.IsEnabled = true;
            TextBoxMaxImages.IsEnabled = true;
        }

        private void ButtonPreviousImage_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage previousImage = mrImaging.BackImage();
            if (previousImage != null)
            {
                ImageBrush myImageBrush = new ImageBrush();
                myImageBrush.ImageSource = previousImage;
                RectangleImage.Fill = myImageBrush;
            }
            else
            {
                MessageBox.Show("No previous images available!");
            }
        }

        private void ButtonNextImage_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage nextImage = mrImaging.ForwardImage();
            if (nextImage != null)
            {
                ImageBrush myImageBrush = new ImageBrush();
                myImageBrush.ImageSource = nextImage;
                RectangleImage.Fill = myImageBrush;
            }
            else
            {
                MessageBox.Show("No next images available!");
            }
        }

        private void TextBoxMaxImages_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(TextBoxMaxImages.Text, out int maxImagesTemp);
            mrImaging.MaxImages = maxImagesTemp;
        }

        private void TextBoxMaxImages_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxMaxImages.Text = "";
            TextBoxMaxImages.Foreground = Brushes.Black;
        }

        private void TextBoxMaxImages_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxMaxImages.Text == "")
            {
                TextBoxMaxImages.Text = "0";
                TextBoxMaxImages.Foreground = Brushes.Red;
            }
        }

        private void TextBoxMaxImages_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
