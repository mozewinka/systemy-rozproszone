using CrackerClient.CrackerServiceReference;
using System;
using System.ServiceModel;

namespace CrackerClient
{
    internal class Program
    {

        private static void Main()
        {
            CallbackHandler callbackHandler = new CallbackHandler();
            InstanceContext instanceContext = new InstanceContext(callbackHandler);
            CrackerServiceClient client = new CrackerServiceClient(instanceContext);
            callbackHandler.Client = client;
            callbackHandler.ClientID = CrackTools.GetCPUID();


            try
            {
                Console.WriteLine("Connecting...");
                client.AddClient();

                Console.WriteLine("Client ready");
                Console.WriteLine("Press enter to quit...");
                _ = Console.ReadLine();

                client.Close();
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("The host did not respond. (" + ex + ")");
                _ = Console.ReadLine();
            }
        }
    }
}
