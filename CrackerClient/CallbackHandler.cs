using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;


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
        public void IO(string perf) //funkcja obslugujaca zapis do pliku
        {
            string filePath = @"C:\Users\weron\source\repos\rozproszone_proj\systemy-rozproszone\CrackerClient\TimesResults.txt"; //sciezka do pliku

            List<string> lines = new List<string>();

            lines = File.ReadAllLines(filePath).ToList();

            lines.Add(perf);
            File.WriteAllLines(filePath, lines);
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
            string performance = result + ". Average cracking speed was " + counter / stopwatch.ElapsedMilliseconds * 1000 + " H/s"; //jest ilosc powtorzen przez czas jako sredni czas a wydaje mi sie ze powinno byc na odwrot xd ze sredni czas to calkowity czas / ilosc powtorzen ;x
            Console.WriteLine(result + elapsed + performance);
            Client.AnnounceResult(result + elapsed + performance);
            IO(performance); //zapis

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
            string performance = result + ". Average cracking speed was " + counter / stopwatch.ElapsedMilliseconds * 1000 + " H/s"; //dopisalam zeby zapisywalo w pliku tez wynik crackowania i czy sie udalo
            Console.WriteLine(result + elapsed + performance);
            Client.AnnounceResult(result + elapsed + performance);
            IO(performance); //funckja zapisu
        }
    }
}
