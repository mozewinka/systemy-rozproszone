using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Diagnostics;

namespace CrackerClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CallbackHandler : ICrackerServiceCallback
    {
        public List<string> DictionaryList { get; set; }
        public CrackerServiceClient Client { get; set; }

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
            string result = "Password not found in given range";

            while (currentPosition != endPosition)
            {
                counter++;
                string currentHash = CrackTools.GetHash(currentPosition);

                if (currentHash.Equals(md5Password))
                {
                    result = "Cracked password: " + currentPosition;
                    break;
                }
                else
                {
                    currentPosition = CrackTools.AddHex(currentPosition, "1");
                }
            }

            stopwatch.Stop();
            string elapsed = ". Elapsed time: " + stopwatch.ElapsedMilliseconds + " ms";
            string performance = ". Average cracking speed was " + counter / stopwatch.ElapsedMilliseconds * 1000 + " H/s";
            Console.WriteLine(result + elapsed + performance);
            Client.AnnounceResult(result + elapsed + performance);
        }

        public void DictionaryCrack(int startPosition, int endPosition, string md5Password, bool checkUpperCase, bool checkSuffix, string suffix)
        {
            if (DictionaryList.Count == 0)
            {
                Console.WriteLine("Getting dictionary...");
                DictionaryData dictionary = Client.SendDictionary();
                DictionaryList = dictionary.List;

                Console.WriteLine("Received dictionary with " + DictionaryList.Count + " words.");
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            int counter = 0;

            List<string> options = new List<string>
            {
                "normal"
            };
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
            string result = "Password not found in given range";

            while (currentPosition != endPosition)
            {
                counter++;
                string currentHash = "";
                string password = "";
                foreach (string option in options)
                {
                    if (option.Equals("normal"))
                    {
                        password = DictionaryList[currentPosition];
                        currentHash = CrackTools.GetHash(password);
                    }
                    else if (option.Equals("checkUpper"))
                    {
                        password = char.ToUpper(DictionaryList[currentPosition][0]) + DictionaryList[currentPosition].Substring(1);
                        currentHash = CrackTools.GetHash(password);
                    }
                    else if (option.Equals("checkSuffix"))
                    {
                        password = DictionaryList[currentPosition] + suffix;
                        currentHash = CrackTools.GetHash(password);
                    }
                    else if (option.Equals("checkUpperAndSuffix"))
                    {
                        password = char.ToUpper(DictionaryList[currentPosition][0]) + DictionaryList[currentPosition].Substring(1) + suffix;
                        currentHash = CrackTools.GetHash(password);
                    }
                    if (currentHash.Equals(md5Password))
                    {
                        break;
                    }
                }
                if (currentHash.Equals(md5Password))
                {
                    result = "Cracked password: " + password;
                    break;
                }
                else
                {
                    currentPosition += 1;
                }
            }

            stopwatch.Stop();
            string elapsed = ". Elapsed time: " + stopwatch.ElapsedMilliseconds + " ms";
            string performance = ". Average cracking speed was " + counter / stopwatch.ElapsedMilliseconds * 1000 + " H/s";
            Console.WriteLine(result + elapsed + performance);
            Client.AnnounceResult(result + elapsed + performance);
        }
    }
}
