//char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
char[] table = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

//int enteredNumber = Convert.ToInt32(Console.ReadLine());

//string changed = ConvertToNumericSystem(table, ref enteredNumber);

//Console.WriteLine("Decimal to new system: {0}", changed);

//Console.WriteLine("New system to decimal: {0}", ExtractToDecimal(table, changed));

Console.WriteLine(AddHex("A", "1"));

static string ConvertToNumericSystem(char[] table, ref int enteredNumber)
{
    string changed = string.Empty;

    while (enteredNumber > 0)
    {
        int reminder = enteredNumber % table.Length;
        changed = table[reminder] + changed;
        enteredNumber = enteredNumber / table.Length;
    }
    return changed;
}

static Double ExtractToDecimal(char[] table, string changed)
{
    Double length = Convert.ToDouble(changed.Length - 1);
    Double dec = 0;

    foreach (char digit in changed)
    {
        dec += Array.IndexOf(table, digit) * Math.Pow(Convert.ToDouble(table.Length), length);
        length -= 1;
    }
    return dec;
}

string AddHex(string a, string b)
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