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

<<<<<<< HEAD
        public List<ICrackerServiceCallback> Callbacks = new List<ICrackerServiceCallback>();

        public void Receive()
=======
        public ConcurrentObservableCollection<ICrackerServiceCallback> Callbacks;
        public ObservableCollection<string> ClientMessages;

        public void AnnounceResult(string message)
>>>>>>> 345f6d9ed391109fd4f376f238f5617d6a038bbe
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



        public void StartCracking(string md5Password)
        {
            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                //callback.PrintBrute(md5Password);

               callback.Print(md5Password);
                // tu?
               
            }
            //...czy tu?


            //czy gdzie xD
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

<<<<<<< HEAD
            Callbacks.Add(OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>()); // do innej metody?

=======
>>>>>>> 345f6d9ed391109fd4f376f238f5617d6a038bbe
            return new DictionaryData()
            {
                List = list
            };
        }

<<<<<<< HEAD
       
=======
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
>>>>>>> 345f6d9ed391109fd4f376f238f5617d6a038bbe
    }
}
