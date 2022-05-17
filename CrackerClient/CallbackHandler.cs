using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;

namespace CrackerClient
{
    public class CallbackHandler : ICrackerServiceCallback
    {
        public List<string> DictionaryList { get; set; }
        public CrackerServiceClient Client { get; set; }

        public void Print(string md5Password)
        {
            Console.Out.WriteLine($"Received {md5Password} from the server.");
        }

        public void BruteCrack(string startPosition, string endPosition, string md5Password)
        {
            Console.WriteLine("Started cracking " + md5Password + " with range (" + startPosition + ", " + endPosition + ")");
            string currentPosition = startPosition;
            string result = "Password not found";

            while (currentPosition != endPosition)
            {
                string currentHash = CrackTools.GetHash(currentPosition);

                if (currentHash.Equals(md5Password))
                {
                    Console.WriteLine("Cracked password: " + currentPosition);
                    result = currentPosition;
                    break;
                }
                else
                {
                    currentPosition = CrackTools.AddHex(currentPosition, "1");
                }
            }

            Client.AnnounceResult(result);
        }
    }
}
