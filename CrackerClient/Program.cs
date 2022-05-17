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

            Console.WriteLine("Connecting...");
            client.AddClient();

            Console.WriteLine("Getting dictionary...");
            DictionaryData dictionary = client.SendDictionary();
            callbackHandler.DictionaryList = dictionary.List;
            //callbackHandler.DictionaryList.ForEach(Console.WriteLine); // test
            Console.WriteLine("Received dictionary with " + callbackHandler.DictionaryList.Count + " words.");

            Console.WriteLine("Client ready");
            Console.WriteLine("Press enter to quit...");
            _ = Console.ReadLine();
            try
            {
                client.Close();
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("Host did not respond to Close() method. (" + ex + ")");
            }
        }
    }
}
