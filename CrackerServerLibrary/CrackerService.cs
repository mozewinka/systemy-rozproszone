using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace CrackerServerLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public class CrackerService : ICrackerService
    {
        public string FilePath { get; set; }

        public ICrackerServiceCallback ClientTestCallback; // tymczasowo

        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void StartCracking(string md5Password)
        {
            //Callback.Print(md5Password);
            ClientTestCallback.Print(md5Password);
            //...
        }

        public DictionaryData SendDictionary()
        {
            FileStream file = File.OpenRead(FilePath);
            StreamReader reader = new StreamReader(file);
            List<string> list = new List<string>();

            while (!reader.EndOfStream)
            {
                list.Add(reader.ReadLine());
            }

            ClientTestCallback = OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>(); // do innej metody?

            return new DictionaryData()
            {
                List = list
            };
        }

        public ICrackerServiceCallback Callback => OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>(); // dodac klientow do listy callbackow zamiast w taki sposob
    }
}
