using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace CrackerClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CallbackHandler : ICrackerServiceCallback
    {
        public List<string> DictionaryList { get; set; }
        public CrackerServiceClient Client { get; set; }
        public string ClientID { get; set; }

        public CallbackHandler()
        {
            DictionaryList = new List<string>();
        }

        public void Print(string md5Password)
        {
            Console.Out.WriteLine($"\nReceived {md5Password} from the server.");
        }

        public void BruteCrack(string startPosition, string endPosition, string md5Password)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int counter = 0;

            Console.WriteLine("Started brute force cracking " + md5Password + " with range (" + startPosition + ", " + endPosition + ")");
            string currentPosition = startPosition;
            ResultData result = new ResultData
            {
                ClientID = ClientID,
                IsCracked = false,
                CrackingMethod = "BruteForce"
            };

            while (currentPosition != endPosition)
            {
                counter++;
                string currentHash = CrackTools.GetHash(currentPosition);

                if (currentHash.Equals(md5Password))
                {
                    result.IsCracked = true;
                    result.CrackedPassword = currentPosition;
                    break;
                }
                else
                {
                    currentPosition = CrackTools.AddHex(currentPosition, "1");
                }
            }

            stopwatch.Stop();
            result.CrackingTime = stopwatch.ElapsedMilliseconds;
            if (result.CrackingTime == 0)
            {
                result.CrackingTime = 1;
            }
            result.CrackingPerformance = counter / result.CrackingTime;
            CrackTools.PrintResult(result);
            Client.AnnounceResult(result);
        }

        public string GetDictionaryHash() {
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "dictionary.txt")))
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream stream = File.OpenRead("dictionary.txt"))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            return null;
        }

        public void DictionaryCrack(int startPosition, int endPosition, string md5Password, bool checkUpperCase, bool checkSuffix)
        {
            string dictionaryHash = GetDictionaryHash();
            if (dictionaryHash != Client.SendDictionaryHash())
            {
                Console.WriteLine("Getting dictionary...");
                DictionaryData dictionary = Client.SendDictionary();
                DictionaryList = dictionary.List;

                Console.WriteLine("Received dictionary with " + DictionaryList.Count + " words.");
                File.WriteAllLines("dictionary.txt", DictionaryList);
                Console.WriteLine("Dictionary saved\n");
            }
            else
            {
                FileStream file = File.OpenRead("dictionary.txt");
                StreamReader reader = new StreamReader(file);
                List<string> list = new List<string>();

                while (!reader.EndOfStream)
                {
                    list.Add(reader.ReadLine());
                }
                DictionaryList = list;

            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            int counter = 0;

            List<string> options = new List<string>
            {
                "normal"
            };
            List<string> suffixes = Enumerable.Range(0, 10).Select(n => n.ToString()).ToList();
            if (checkUpperCase)
            {
                options.Add("checkUpper");
            }
            if (checkSuffix)
            {
                options.Add("checkSuffix");
            }
            if (checkSuffix && checkUpperCase)
            {
                options.Add("checkUpperAndSuffix");
            }

            Console.WriteLine("Started dictionary cracking " + md5Password + " with range (" + startPosition + ", " + endPosition + ")");
            int currentPosition = startPosition;
            ResultData result = new ResultData
            {
                ClientID = ClientID,
                IsCracked = false,
                CrackingMethod = "Dictionary"
            };

            while (currentPosition != endPosition)
            {
                string currentHash = "";
                string password = "";
                foreach (string option in options)
                {
                    if (option.Equals("normal"))
                    {
                        password = DictionaryList[currentPosition];
                        currentHash = CrackTools.GetHash(password);
                        counter++;
                    }
                    else if (option.Equals("checkUpper"))
                    {
                        password = char.ToUpper(DictionaryList[currentPosition][0]) + DictionaryList[currentPosition].Substring(1);
                        currentHash = CrackTools.GetHash(password);
                        counter++;
                    }
                    else if (option.Equals("checkSuffix"))
                    {
                        foreach(string suffix in suffixes)
                        {
                            counter++;
                            password = DictionaryList[currentPosition] + suffix;
                            currentHash = CrackTools.GetHash(password);
                        }
                    }
                    else if (option.Equals("checkUpperAndSuffix"))
                    {
                        foreach (string suffix in suffixes)
                        {
                            counter++;
                            password = char.ToUpper(DictionaryList[currentPosition][0]) + DictionaryList[currentPosition].Substring(1) + suffix;
                            currentHash = CrackTools.GetHash(password);
                        }
                    }
                    if (currentHash.Equals(md5Password))
                    {
                        break;
                    }
                }
                if (currentHash.Equals(md5Password))
                {
                    result.IsCracked = true;
                    result.CrackedPassword = password; ;
                    break;
                }
                else
                {
                    currentPosition += 1;
                }
            }

            stopwatch.Stop();
            result.CrackingTime = stopwatch.ElapsedMilliseconds;
            if (result.CrackingTime == 0)
            {
                result.CrackingTime = 1;
            }
            result.CrackingPerformance = counter / result.CrackingTime;
            CrackTools.PrintResult(result);
            Client.AnnounceResult(result);
        }
    }
}
