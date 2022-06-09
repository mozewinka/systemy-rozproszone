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
            ClientMessages.Add("Client ID: " + result.ClientID + "\n" +
                               "Result: " + (result.IsCracked ? "Cracked password: " + result.CrackedPassword : "Password not found in given range") + "\n" +
                               "Elapsed time: " + result.CrackingTime + " ms" + "\n" +
                               "Average cracking speed: " + result.CrackingPerformance + " kH/s");
            LogResultToFile(result);

            IsCracking = !result.IsCracked;
            if (IsCracking)
            {
                Client client = Clients.First(e => e.ClientID == result.ClientID);
                if (result.CrackingMethod == "Dictionary")
                {
                    
                    client.Callback.DictionaryCrack(currentPosition, currentPosition + packageSize, md5Password, checkUpperCase, checkSuffix); //temp
                }
                else
                {
                    client.Callback.BruteCrack(currentPosition.ToString(), (currentPosition + packageSize).ToString(), md5Password); //temp
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

            foreach (Client client in Clients)
            {
                callback.Print(md5Password);
                string endPosition = AddInNumericSystem(ConvertToNumericSystem(currentPosition), ConvertToNumericSystem(packageSize));
                callback.BruteCrack(ConvertToNumericSystem(currentPosition),endPosition, md5Password);
                currentPosition += packageSize;
            }
        }

        string AddInNumericSystem(string a, string b)
        {
            char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            if (a.Length < b.Length)
            {
                string tmp = a;
                a = b; b = tmp;
            }
            int l1 = a.Length, l2 = b.Length;
            string ans = "";
            int carry = 0, i, j;

            for (i = l1 - 1, j = l2 - 1;
                 j >= 0; i--, j--)
            {
                int sum = Array.IndexOf(table, a[i]) + Array.IndexOf(table, b[j]) + carry;

                int addition_bit = table[sum % table.Length];
                ans += (char)addition_bit;

                carry = sum / table.Length;
            }

            while (i >= 0)
            {
                int sum = Array.IndexOf(table, a[i]) + carry;
                int addition_bit = table[sum % table.Length];

                ans += (char)addition_bit;
                carry = sum / table.Length;
                i--;
            }
            if (carry > 0)
            {
                ans += table[carry];
            }
            char[] charArray = ans.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);

        }

        static string ConvertToNumericSystem( int enteredNumber)
        {
            string changed = string.Empty;
            char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            while (enteredNumber > 0)
            {
                int reminder = enteredNumber % table.Length;
                changed = table[reminder] + changed;
                enteredNumber = enteredNumber / table.Length;
            }
            return changed;
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
