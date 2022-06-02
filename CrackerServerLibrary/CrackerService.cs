using Meziantou.Framework.WPF.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel;

namespace CrackerServerLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    public class CrackerService : ICrackerService
    {
        public string FilePath { get; set; }

        public ConcurrentObservableCollection<ICrackerServiceCallback> Callbacks;
        public ObservableCollection<string> ClientMessages;

        public void AnnounceResult(string message)
        {
            ClientMessages.Add(message);
        }

        public void StartCrackingBrute(string md5Password, int packageSize)
        {
            int position = 0;
            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                callback.Print(md5Password);
                callback.BruteCrack(position.ToString(), (position + packageSize).ToString(), md5Password);
                position += packageSize;
            }
        }

        public void StartCrackingDictionary(string md5Password, int packageSize, bool checkUpperCase, bool checkSuffix, String suffix)
        {
            int position = 0;
            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                callback.Print(md5Password);
                callback.DictionaryCrack(position, position + packageSize, md5Password, checkUpperCase, checkSuffix, suffix);
                position += packageSize;
            }
        }

        public DictionaryData SendDictionary()
        {
            List<string> list = new List<string>();

            if (FilePath != null)
            {
                FileStream file = File.OpenRead(FilePath);
                StreamReader reader = new StreamReader(file);

                while (!reader.EndOfStream)
                {
                    list.Add(reader.ReadLine());
                }
            }

            return new DictionaryData()
            {
                List = list
            };
        }

        public void AddClient()
        {
            Callbacks.Add(OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>());
            OperationContext.Current.Channel.Closed += new EventHandler(OnClientDisconnected);
        }

        public void OnClientDisconnected(object sender, EventArgs args)
        {
            ICrackerServiceCallback callback = sender as ICrackerServiceCallback;
            _ = Callbacks.Remove(callback);
        }
    }
}
