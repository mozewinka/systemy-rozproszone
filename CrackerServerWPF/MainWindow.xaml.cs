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
            Uri baseAddress = new Uri("http://localhost:8001/CrackerService/");
            selfHost = new ServiceHost(instance, baseAddress);

            try
            {
                _ = selfHost.AddServiceEndpoint(typeof(ICrackerService), new WSDualHttpBinding(), "CrackerService");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true
                };
                selfHost.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
                selfHost.Description.Behaviors.Add(smb);

                selfHost.Open();
                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
                Logs.Add("Service started");

            }
            catch (CommunicationException ce)
            {
                Logs.Add("Exception: " + ce.Message);
                selfHost.Abort();
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                selfHost.Close();
                Logs.Add("Service stopped");
                startButton.IsEnabled = true;
                stopButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Logs.Add("Exception: " + ex.Message);
            }
        }

        private void BruteForceRadioChecked(object sender, RoutedEventArgs e)
        {
            crackButton.IsEnabled = true;
            filePath.IsEnabled = false;
            fileButton.IsEnabled = false;
        }

        private void DictionaryRadioChecked(object sender, RoutedEventArgs e)
        {
            crackButton.IsEnabled = true;
            filePath.IsEnabled = true;
            fileButton.IsEnabled = true;
        }

        private void CrackButtonClick(object sender, RoutedEventArgs e)
        {
            string method = (bool)bruteForceRadio.IsChecked ? "Brute Force" : "Dictionary";
            Logs.Add("Started cracking with " + method + " method");
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

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
