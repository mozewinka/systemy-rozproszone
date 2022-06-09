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
        public bool IsCracking { get; set; }

        private int currentPosition;
        private string md5Password;
        private int packageSize;
        private bool checkUpperCase;
        private bool checkSuffix;
        private string suffix;

        public ConcurrentObservableCollection<ICrackerServiceCallback> Callbacks;
        public ObservableCollection<string> ClientMessages;

        public void AnnounceResult(ResultData result)
        {
            ClientMessages.Add("Client ID: " + result.ClientID + "\n" +
                               "Result: " + (result.IsCracked ? "Cracked password: " + result.CrackedPassword : "Password not found in given range") + "\n" +
                               "Elapsed time: " + result.CrackingTime + " ms" + "\n" +
                               "Average cracking speed: " + result.CrackingPerformance + " kH/s");
            LogResultToFile(result);
            IsCracking = !result.IsCracked;
            if (IsCracking)
            {
                if (result.CrackingMethod == "Dictionary")
                {
                    Callbacks[0].DictionaryCrack(currentPosition, currentPosition + packageSize, md5Password, checkUpperCase, checkSuffix, suffix); //temp
                }
                else
                {
                    Callbacks[0].BruteCrack(currentPosition.ToString(), (currentPosition + packageSize).ToString(), md5Password); //temp
                }
                currentPosition += packageSize;
            }
        }

        private void LogResultToFile(ResultData result)
        {
            string path = "results.csv";
            if (!File.Exists(path))
            {
                using (StreamWriter streamWriter = File.CreateText(path))
                {
                    streamWriter.WriteLine("ClientID;IsCracked;CrackedPassword;CrackingTime(ms);CrackingPerformance(kH/s);Method");
                }
            }

            using (StreamWriter streamWriter = File.AppendText(path))
            {
                ClientMessages.Add(((FileStream)streamWriter.BaseStream).Name); //temp
                streamWriter.WriteLine(result.ClientID + ";" +
                                       result.IsCracked + ";" +
                                       result.CrackedPassword + ";" +
                                       result.CrackingTime + ";" +
                                       result.CrackingPerformance + ";" +
                                       result.CrackingMethod);
            }
        }

        public void StartCrackingBrute(string md5Password, int packageSize)
        {
            currentPosition = 0;
            this.md5Password = md5Password;
            this.packageSize = packageSize;

            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                callback.Print(md5Password);
                callback.BruteCrack(currentPosition.ToString(), (currentPosition + packageSize).ToString(), md5Password);
                currentPosition += packageSize;
            }
        }

        public void StartCrackingDictionary(string md5Password, int packageSize, bool checkUpperCase, bool checkSuffix, string suffix)
        {
            currentPosition = 0;
            this.md5Password = md5Password;
            this.packageSize = packageSize;
            this.checkUpperCase = checkUpperCase;
            this.checkSuffix = checkSuffix;
            this.suffix = suffix;

            foreach (ICrackerServiceCallback callback in Callbacks)
            {
                callback.Print(md5Password);
                callback.DictionaryCrack(currentPosition, currentPosition + packageSize, md5Password, checkUpperCase, checkSuffix, suffix);
                currentPosition += packageSize;
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
