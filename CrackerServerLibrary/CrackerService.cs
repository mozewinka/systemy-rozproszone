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

        public void Receive()
        {
            throw new NotImplementedException();
        }

        public void StartCracking(string md5Password)
        {
            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                callback.Print(md5Password);
            }
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

            Callbacks.Add(OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>()); // do innej metody?

            return new DictionaryData()
            {
                List = list
            };
        }
    }
}
