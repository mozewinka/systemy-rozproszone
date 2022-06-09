using CrackerServerLibrary;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Security.Cryptography;
using System.Collections.Specialized;
using Meziantou.Framework.WPF.Collections;

namespace CrackerServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost selfHost;
        private readonly CrackerService instance;

        private ObservableCollection<string> Logs { get; }
        private ObservableCollection<string> ClientMessages { get; }
        private ConcurrentObservableCollection<Client> Clients { get; }

        public MainWindow()
        {
            InitializeComponent();
            stopButton.IsEnabled = false;
            crackButton.IsEnabled = false;
            cancelButton.IsEnabled = false;
            filePath.IsEnabled = false;
            fileButton.IsEnabled = false;
            checkUpperCase.Visibility = Visibility.Hidden;
            checkSuffix.Visibility = Visibility.Hidden;
            instance = new CrackerService();

            Logs = new ObservableCollection<string>();
            listView.ItemsSource = Logs;

            ClientMessages = new ObservableCollection<string>();
            ClientMessages.CollectionChanged += OnMessagesChanged;
            instance.ClientMessages = ClientMessages;

            Clients = new ConcurrentObservableCollection<Client>();
            Clients.AsObservable.CollectionChanged += OnCallbacksChanged;
            instance.Clients = Clients;
        }

        private void OnMessagesChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            foreach (object element in args.NewItems)
            {
                Logs.Add(element.ToString());
            }
            cancelButton.IsEnabled = instance.IsCracking;
            crackButton.IsEnabled = !instance.IsCracking;
        }

        private void OnCallbacksChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            clientsCountLabel.Content = Clients.Count.ToString();
            crackButton.IsEnabled = Clients.Count >= 1;
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
                statusLabel.Text = "STARTED";
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
                statusLabel.Text = "STARTING FAILED";
                statusLabel.Foreground = Brushes.Red;
                Logs.Add("Exception: " + ce.Message);
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Clients.Clear();
                selfHost.Close();
                statusLabel.Text = "STOPPED";
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
            checkUpperCase.Visibility = Visibility.Hidden;
            checkSuffix.Visibility = Visibility.Hidden;
        }
        
        private void DictionaryRadioChecked(object sender, RoutedEventArgs e)
        {
            if (!startButton.IsEnabled)
            {
                crackButton.IsEnabled = true;
            }
            filePath.IsEnabled = true;
            fileButton.IsEnabled = true;
            checkUpperCase.Visibility = Visibility.Visible;
            checkSuffix.Visibility = Visibility.Visible;
        }

        private static string GetHash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();
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
            int packageSize = int.Parse(packageSizeTextBox.Text);

            Logs.Add("MD5 hash of " + passwordTextBox.Text + ": " + md5Password + "\n" +
                     "Package size: " + packageSize + "\n" +
                     "Started cracking with " + method + " method");

            if (method == "Brute Force")
            {
                instance.StartCrackingBrute(md5Password, packageSize);
            }
            else
            {
                instance.StartCrackingDictionary(md5Password, packageSize, (bool)checkUpperCase.IsChecked, (bool)checkSuffix.IsChecked);
            }

            crackButton.IsEnabled = false;
            cancelButton.IsEnabled = true;
            instance.IsCracking = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            crackButton.IsEnabled = true;
            cancelButton.IsEnabled = false;
            instance.IsCracking = false;
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

        private void ClearLogsButtonClick(object sender, RoutedEventArgs e)
        {
            Logs.Clear();
        }
    }
}
