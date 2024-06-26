using System.Globalization;

namespace Rutils;

public static class StringHelpers
{
    public static string CurrencyString(int currencyAmount, string? prefix = "$", string? postfix = null, string specificCulture = "en-US")
    {
        return CurrencyString((double)currencyAmount, prefix, postfix, specificCulture);
    }

    public static string CurrencyString(float currencyAmount, string? prefix = "$", string? postfix = null, string specificCulture = "en-US")
    {
        return CurrencyString((double)currencyAmount, prefix, postfix, specificCulture);
    }

    public static string CurrencyString(double currencyAmount, string? prefix = "$", string? postfix = null, string specificCulture = "en-US")
    {
        bool hasDecimals = Math.Abs(currencyAmount % 1) > (Double.Epsilon * 100);

        var culture = CultureInfo.GetCultureInfo("en-US");

        string currencyAmountString = hasDecimals ? Math.Abs(currencyAmount).ToString("#,0.00", culture) : int.Abs((int)currencyAmount).ToString("#,0", culture);
        return $"{(currencyAmount < 0f ? "-" : "")}{(prefix != null ? prefix : "")}{currencyAmountString}{(postfix != null ? postfix : "")}";
    }


    /// <summary>
    /// Splits a string into chunks that do not exceed the max length provided
    /// </summary>
    public static string SplitStringInChunks(string src, int maxLength, string splitDelimiter = " ", string joinDelimiter = "\n")
    {
        List<string> chunks = new List<string>();

        string chunk = "";

        //split the string
        string[] splitString = src.Split(splitDelimiter);


        for (int i = 0; i < splitString.Length; ++i)
        {
            ref string currentString = ref splitString[i]; 

            if (chunk.Length == 0)
            {
                chunk += currentString;
            }
            else if (chunk.Length + currentString.Length <= maxLength)
            {
                chunk += splitDelimiter + currentString;
            }
            else
            {
                //chunk will exceed the maxLength
                chunks.Add(chunk);
                chunk = currentString;
            }
        }
        
        //add last chunk if we have to
        if (chunk.Length != 0)
        {
            chunks.Add(chunk);
        }

        //return joined result
        return string.Join(joinDelimiter, chunks);
    }

    public static string CreateTable<T>
    (
        IEnumerable<T> entries,
        IEnumerable<(string columnTitle, Func<T,string> selectorFunction)> columns,
        char verticalSeperator = '|',
        char horizontalSeperator = '-',
        char spacingChar = ' '
    )
    {          
        //initialize column lists  (2d array x = column, y = data entry)
        string[,] tableData = new string[ columns.Count(), entries.Count() ];
        
        //populate column list with data
        for (int y = 0; y < entries.Count(); ++y)
        {
            T entry = entries.ElementAt(y);

            //fetch all column data for entry
            for (int x = 0; x < columns.Count(); ++x)
            {
                //populate 2d array
                tableData[x, y] = columns.ElementAt(x).selectorFunction.Invoke(entry);
            }
        }

        //construct table as text

        //for each column, find the longest string
        int[] columnSizes = new int[tableData.GetLength(0)];

        for (int x = 0; x < tableData.GetLength(0); ++x)
        {
            int highestLength = columns.ElementAt(x).columnTitle.Length;
            
            for (int y = 0; y < tableData.GetLength(1); ++y)
            {
                if (tableData[x, y].Length > highestLength)
                {
                    highestLength = tableData[x, y].Length;
                }
            }

            columnSizes[x] = highestLength;
        }

        //we now have the entry size for every column, create string table now.
        string horizontalLine = $"{verticalSeperator}{new string(horizontalSeperator, columnSizes.Sum() + tableData.GetLength(0) * 3 - 1)}{verticalSeperator}\n";
        string tableString = "";

        //create table header
        tableString += $"{verticalSeperator}{spacingChar}{string.Join($"{spacingChar}{verticalSeperator}{spacingChar}", columns.Select((column, index) => column.columnTitle.PadRight(columnSizes[index], spacingChar)))}{spacingChar}{verticalSeperator}\n";
        tableString += horizontalLine;

        //create rows
        for (int y = 0; y < tableData.GetLength(1); ++y)
        {
            string[] rowStrings = new string[tableData.GetLength(0)];

            for (int x = 0; x < tableData.GetLength(0); ++x)
            {
                rowStrings[x] = tableData[x,y].PadRight(columnSizes[x], spacingChar);
            }

            //append row data
            tableString += $"{verticalSeperator}{spacingChar}{string.Join($"{spacingChar}{verticalSeperator}{spacingChar}", rowStrings)}{spacingChar}{verticalSeperator}\n";
            tableString += horizontalLine;
        }
        
        //we have a finished string table
        return tableString;
    }

    public static string FirstCharToUpper(this string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException("There is no first letter");

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
}