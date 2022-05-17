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

        public List<ICrackerServiceCallback> Callbacks = new List<ICrackerServiceCallback>();
        public List<string> ClientMessages = new List<string>();

        public void AnnounceResult(string message)
        {
            ClientMessages.Add(message); // temp
        }

        public void StartCrackingBrute(string md5Password)
        {
            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                callback.Print(md5Password);
                callback.BruteCrack("0", "1000000", md5Password); // temp range
            }
            //...
        }

        public void StartCrackingDictionary(string md5Password)
        {
            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                callback.Print(md5Password);
                callback.DictionaryCrack(0, 30000, md5Password); // temp range
            }
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

            Callbacks.Add(OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>()); // do innej metody?

            return new DictionaryData()
            {
                List = list
            };
        }
    }
}
