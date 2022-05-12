using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Security.Cryptography;
using System.Text;

namespace CrackerClient
{
    public class CallbackHandler : ICrackerServiceCallback
    {
        char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        //char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public List<string> DictionaryList { get; set; }

        public void Print(string md5Password)
        {
            Console.Out.WriteLine($"Received {md5Password} from the server.");
            Console.Out.WriteLine($"Algorytm Brute Force zwrocil : {BruteCrack("0", md5Password)} ");
        }
       

        public string BruteCrack(string Sa, string SHb)
        {
            //string SHb = GetHash(Sb);//hashowanie hasła
            string wynik = "brak";
            for (int i = 0; i < 100; i++)
            {
                string SHa = GetHash(Sa);//hashowanie kolejnych iteracji "AAAAAA.."
                                         // Console.WriteLine("Sa: " + Sa + " SHa: " + SHa + " Sb:  " + Sb + " SHb: " + SHb);
                if (SHa.Equals(SHb)) //porównanie hashy
                {
                    Console.WriteLine(i+ "-ta iteracja bedaca wynikiem, shashowana: " + SHa);
                    wynik = SHa;
                    return wynik; //zgadza się
                }             // else if (i==100000-1)//w ostatniej iteracji
                              //  return false;//
                else
                {
                    Sa = AddHex(Sa, "1");
                   // Console.WriteLine("Sa: " + Sa);

                }
            }

            return wynik;
        }

        public string AddHex(string a, string b)
        {
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

        private static string GetHash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    _ = stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
    }

    internal class Program
    {
        private static void Main()
        {
            CallbackHandler callbackHandler = new CallbackHandler();
            InstanceContext instanceContext = new InstanceContext(callbackHandler);
            CrackerServiceClient client = new CrackerServiceClient(instanceContext);

            Console.WriteLine("Getting dictionary...");
            DictionaryData dictionary = client.SendDictionary();
            callbackHandler.DictionaryList = dictionary.List;
            //callbackHandler.DictionaryList.ForEach(Console.WriteLine); // test
            Console.WriteLine("Received dictionary with " + callbackHandler.DictionaryList.Count + " words.");

            Console.WriteLine("\nPress enter to quit...\n");
            _ = Console.ReadLine();
            client.Close();
        }
    }
}
