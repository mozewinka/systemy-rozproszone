using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace CrackerServerLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public class CrackerService : ICrackerService
    {
        public void Receive()
        {
            throw new NotImplementedException();
        }

        public DictionaryData SendDictionary()
        {
            FileStream file = File.OpenRead("dictionary.txt"); // TODO: add file dialog
            StreamReader reader = new StreamReader(file);
            List<string> list = new List<string>();

            while (!reader.EndOfStream)
            {
                list.Add(reader.ReadLine());
            }

            Callback.Print(); // test
            return new DictionaryData()
            {
                List = list
            };
        }

        public ICrackerServiceCallback Callback => OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>();
    }
}
