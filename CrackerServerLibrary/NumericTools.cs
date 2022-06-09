using System;

namespace CrackerServerLibrary
{
    public static class NumericTools
    {
        public static string AddInNumericSystem(string a, string b)
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

        public static string ConvertToNumericSystem(int enteredNumber)
        {
            if (enteredNumber == 0)
            {
                return "0";
            }

            string changed = string.Empty;
            char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            while (enteredNumber > 0)
            {
                int reminder = enteredNumber % table.Length;
                changed = table[reminder] + changed;
                enteredNumber /= table.Length;
            }
            return changed;
        }
    }
}
