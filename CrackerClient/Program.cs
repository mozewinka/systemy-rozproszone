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

            Console.WriteLine("Getting dictionary...");
            DictionaryData dictionary = client.SendDictionary();
            callbackHandler.DictionaryList = dictionary.List;
            //callbackHandler.DictionaryList.ForEach(Console.WriteLine); // test
            Console.WriteLine("Received dictionary with " + callbackHandler.DictionaryList.Count + " words.");

            Console.WriteLine("\nPress enter to quit...\n");
            _ = Console.ReadLine();
            client.Close();
        }
    }
}
