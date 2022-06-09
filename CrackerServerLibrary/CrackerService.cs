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

        public void AnnounceResult(ResultData result)
        {
            ClientMessages.Add("Client ID: " + result.ClientID + "\n" +
                               "Result: " + (result.IsCracked ? "Cracked password: " + result.CrackedPassword : "Password not found in given range") + "\n" +
                               "Elapsed time: " + result.CrackingTime + " ms" + "\n" +
                               "Average cracking speed: " + result.CrackingPerformance + " kH/s");
            LogResultToFile(result);
        }

        private void LogResultToFile(ResultData result)
        {
            string path = "results.csv";
            if (!File.Exists(path))
            {
                using (StreamWriter streamWriter = File.CreateText(path))
                {
                    streamWriter.WriteLine("ClientID;IsCracked;CrackedPassword;CrackingTime(ms);CrackingPerformance(kH/s)");
                }
            }

            using (StreamWriter streamWriter = File.AppendText(path))
            {
                ClientMessages.Add(((FileStream)streamWriter.BaseStream).Name); //temp
                streamWriter.WriteLine(result.ClientID + ";" + result.IsCracked + ";" + result.CrackedPassword + ";" + result.CrackingTime + ";" + result.CrackingPerformance);
            }
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

        public void StartCrackingDictionary(string md5Password, int packageSize, bool checkUpperCase, bool checkSuffix, string suffix)
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
