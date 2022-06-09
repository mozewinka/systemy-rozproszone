using CrackerClient.CrackerServiceReference;
using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace CrackerClient
{
    internal static class CrackTools
    {
        static readonly char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 
            'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public static string AddHex(string a, string b)
        {
            if (a.Length < b.Length)
            {
                string tmp = a;
                a = b; b = tmp;
            }
            int l1 = a.Length, l2 = b.Length;
            string ans = "";
            int carry = 0, i, j;

            for (i = l1 - 1, j = l2 - 1; j >= 0; i--, j--)
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

        public static string GetHash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    _ = stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static void PrintResult(ResultData result)
        {
            Console.WriteLine("Client ID: " + result.ClientID);
            Console.WriteLine("Result: " + (result.IsCracked ? "Cracked password: " + result.CrackedPassword : "Password not found in given range"));
            Console.WriteLine("Elapsed time: " + result.CrackingTime + " ms");
            Console.WriteLine("Average cracking speed: " + result.CrackingPerformance + " kH/s\n");
        }

        public static string GetCPUID()
        {
            string cpuID = "";

            ManagementClass management = new ManagementClass("win32_processor");
            ManagementObjectCollection objects = management.GetInstances();

            foreach (ManagementObject @object in objects)
            {
                if (cpuID == "")
                {
                    cpuID = @object.Properties["processorID"].Value.ToString();
                    break;
                }
            }

            return cpuID.ToLower();
        }
    }
}
