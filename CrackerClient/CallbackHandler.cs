using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;

namespace CrackerClient
{
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
            Console.WriteLine("Started brute force cracking " + md5Password + " with range (" + startPosition + ", " + endPosition + ")");
            string currentPosition = startPosition;
            string result = "Password not found in given range";

            while (currentPosition != endPosition)
            {
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

            Console.WriteLine(result);
            Client.AnnounceResult(result);
        }

        public void DictionaryCrack(int startPosition, int endPosition, string md5Password)
        {
            //if (DictionaryList.Count == 0) // add check if local dict is same as server one
            //{
            //    Console.WriteLine("Getting dictionary...");
            //    DictionaryData dictionary = Client.SendDictionary(); // nie dziala
            //    DictionaryList = dictionary.List;
            //    //callbackHandler.DictionaryList.ForEach(Console.WriteLine); // test
            //    Console.WriteLine("Received dictionary with " + DictionaryList.Count + " words.");
            //}

            Console.WriteLine("Started dictionary cracking " + md5Password + " with range (" + startPosition + ", " + endPosition + ")");
            int currentPosition = startPosition;
            string result = "Password not found in given range";

            while (currentPosition != endPosition)
            {
                string currentHash = CrackTools.GetHash(DictionaryList[currentPosition]);

                if (currentHash.Equals(md5Password))
                {
                    result = "Cracked password: " + DictionaryList[currentPosition];
                    break;
                }
                else
                {
                    currentPosition += 1;
                }
            }

            Console.WriteLine(result);
            Client.AnnounceResult(result);
        }
    }
}
