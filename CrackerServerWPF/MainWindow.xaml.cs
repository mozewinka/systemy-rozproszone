using CrackerServerLibrary;
using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
            StopButton.IsEnabled = false;
            CrackButton.IsEnabled = false;
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            Uri baseAddress = new Uri("http://localhost:8001/CrackerService/");
            selfHost = new ServiceHost(typeof(CrackerService), baseAddress);

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
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                Console.WriteLine("service started"); // tymczasowo do konsoli

            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("exception: {0}", ce.Message); // tymczasowo do konsoli
                selfHost.Abort();
            }
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                selfHost.Close();
                Console.WriteLine("service stopped"); // tymczasowo do konsoli
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception: {0}", ex.Message); // tymczasowo do konsoli
            }
        }
    }
}
