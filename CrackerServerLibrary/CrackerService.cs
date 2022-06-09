using Meziantou.Framework.WPF.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public ConcurrentObservableCollection<Client> Clients;
        public ObservableCollection<string> ClientMessages;

        public void AnnounceResult(ResultData result)
        {
            IsCracking = !result.IsCracked;

            ClientMessages.Add("Client ID: " + result.ClientID + "\n" +
                               "Result: " + (result.IsCracked ? "Cracked password: " + result.CrackedPassword : "Password not found in given range") + "\n" +
                               "Elapsed time: " + result.CrackingTime + " ms" + "\n" +
                               "Average cracking speed: " + result.CrackingPerformance + " kH/s");
            LogResultToFile(result);
            
            if (IsCracking)
            {
                Client client = Clients.First(e => e.ClientID == result.ClientID);
                if (result.CrackingMethod == "Dictionary")
                {
                    client.Callback.DictionaryCrack(currentPosition, currentPosition + packageSize, md5Password, checkUpperCase, checkSuffix);
                }
                else
                {
                    string endPosition = NumericTools.AddInNumericSystem(NumericTools.ConvertToNumericSystem(currentPosition), NumericTools.ConvertToNumericSystem(packageSize));
                    client.Callback.BruteCrack(NumericTools.ConvertToNumericSystem(currentPosition), endPosition, md5Password);
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
                //ClientMessages.Add(((FileStream)streamWriter.BaseStream).Name); //temp
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

            foreach (Client client in Clients)
            {
                client.Callback.Print(md5Password);
                string endPosition = NumericTools.AddInNumericSystem(NumericTools.ConvertToNumericSystem(currentPosition), NumericTools.ConvertToNumericSystem(packageSize));
                client.Callback.BruteCrack(NumericTools.ConvertToNumericSystem(currentPosition), endPosition, md5Password);
                currentPosition += packageSize;
            }
        }

        public void StartCrackingDictionary(string md5Password, int packageSize, bool checkUpperCase, bool checkSuffix)
        {
            currentPosition = 0;
            this.md5Password = md5Password;
            this.packageSize = packageSize;
            this.checkUpperCase = checkUpperCase;
            this.checkSuffix = checkSuffix;

            foreach (Client client in Clients)
            {
                client.Callback.Print(md5Password);
                client.Callback.DictionaryCrack(currentPosition, currentPosition + packageSize, md5Password, checkUpperCase, checkSuffix);
                currentPosition += packageSize;
            }
        }

        public DictionaryData SendDictionary()
        {
            List<string> list = new List<string>();

            if (FilePath != null)
            {
                Console.WriteLine(FilePath);
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

        public string SendDictionaryHash()
        {
            if (FilePath != null)
            {
                Console.WriteLine(FilePath);
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream stream = File.OpenRead(FilePath))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            return null;
        }

        public void AddClient(string clientID)
        {
            Clients.Add(new Client()
            {
                ClientID = clientID,
                Callback = OperationContext.Current.GetCallbackChannel<ICrackerServiceCallback>()
            });
            OperationContext.Current.Channel.Closed += new EventHandler(OnClientDisconnected);
        }

        public void OnClientDisconnected(object sender, EventArgs args)
        {
            ICrackerServiceCallback callback = sender as ICrackerServiceCallback;
            _ = Clients.Remove(Clients.First(e => e.Callback == callback));
        }
    }
}
