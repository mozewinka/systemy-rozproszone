using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CrackerClient
{
    public class CallbackHandler : ICrackerServiceCallback
    {
        public List<string> DictionaryList { get; set; }

        public void Print()
        {
            Console.Out.WriteLine("print");
        }
    }

    internal class Program
    {
        private static void Main()
        {
            CallbackHandler callbackHandler = new CallbackHandler();
            InstanceContext instanceContext = new InstanceContext(callbackHandler);
            CrackerServiceClient client = new CrackerServiceClient(instanceContext);

            DictionaryData dictionary = client.SendDictionary();
            callbackHandler.DictionaryList = dictionary.List;
            callbackHandler.DictionaryList.ForEach(Console.WriteLine); // test

            _ = Console.ReadLine();
            client.Close();
        }
    }
}
