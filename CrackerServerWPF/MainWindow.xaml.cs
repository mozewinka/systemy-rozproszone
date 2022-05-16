using CrackerServerLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace CrackerServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost selfHost;
        private readonly CrackerService instance;

        private ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            stopButton.IsEnabled = false;
            crackButton.IsEnabled = false;
            listView.ItemsSource = Logs;
            filePath.IsEnabled = false;
            fileButton.IsEnabled = false;
            instance = new CrackerService();
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            Uri baseAddress = new Uri($"http://{ipTextBox.Text}:8001/CrackerService/");
            selfHost = new ServiceHost(instance, baseAddress);

            try
            {
                WSDualHttpBinding binding = new WSDualHttpBinding
                {
                    Name = "DuplexBinding",
                    Security = { Mode = WSDualHttpSecurityMode.None },
                    MessageEncoding = WSMessageEncoding.Mtom
                };
                _ = selfHost.AddServiceEndpoint(typeof(ICrackerService), binding, "CrackerService");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true
                };
                selfHost.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
                selfHost.Description.Behaviors.Add(smb);

                selfHost.Open();
                statusLabel.Content = "STARTED";
                statusLabel.Foreground = Brushes.Green;
                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
                if ((bool)bruteForceRadio.IsChecked || (bool)dictionaryRadio.IsChecked)
                {
                    crackButton.IsEnabled = true;
                }
                Logs.Add($"Service started at http://{ipTextBox.Text}:8001/CrackerService/");

            }
            catch (CommunicationException ce)
            {
                selfHost.Abort();
                statusLabel.Content = "STARTING FAILED";
                statusLabel.Foreground = Brushes.Red;
                Logs.Add("Exception: " + ce.Message);
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                selfHost.Close();
                statusLabel.Content = "STOPPED";
                statusLabel.Foreground = Brushes.Red;
                startButton.IsEnabled = true;
                stopButton.IsEnabled = false;
                if ((bool)bruteForceRadio.IsChecked || (bool)dictionaryRadio.IsChecked)
                {
                    crackButton.IsEnabled = false;
                }
                Logs.Add("Service stopped");
            }
            catch (Exception ex)
            {
                Logs.Add("Exception: " + ex.Message);
            }
        }

        private void BruteForceRadioChecked(object sender, RoutedEventArgs e)
        {
            if (!startButton.IsEnabled)
            {
                crackButton.IsEnabled = true;
            }
            filePath.IsEnabled = false;
            fileButton.IsEnabled = false;
        }

        private void DictionaryRadioChecked(object sender, RoutedEventArgs e)
        {
            if (!startButton.IsEnabled)
            {
                crackButton.IsEnabled = true;
            }
            filePath.IsEnabled = true;
            fileButton.IsEnabled = true;
        }

        private static string GetHash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    _ = stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        private void CrackButtonClick(object sender, RoutedEventArgs e)
        {
            string method = (bool)bruteForceRadio.IsChecked ? "Brute Force" : "Dictionary";
            string md5Password = GetHash(passwordTextBox.Text);
            
            Logs.Add("The MD5 hash of " + passwordTextBox.Text + " is " + md5Password);
            Logs.Add("Started cracking with " + method + " method");

            instance.StartCracking(md5Password);
            // ...
        }

        private void FileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                filePath.Text = fileDialog.FileName;
                instance.FilePath = fileDialog.FileName;
            }
        }
    }
}
