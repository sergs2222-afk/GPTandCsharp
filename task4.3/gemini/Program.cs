using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class DivisionProgram
{
    public delegate string ZeroDivisionHandler(decimal dividend, decimal divisor);
    public static event ZeroDivisionHandler OnZeroDivision;

    public static void Main()
    {
        string input = Console.ReadLine();
        string originalInput = input;

        // 3. Нормалізація вхідних даних
        input = input.Replace('−', '-');
        input = Regex.Replace(input, @"\s+", " ").Trim();
        input = Regex.Replace(input, @"'|(?<=\d)[ \u00A0\u202F]+(?=\d)", "");
        string[] numbers = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (numbers.Length != 2)
        {
            return;
        }

        decimal dividend = 0;
        decimal divisor = 0;

        // 3.5. Парсинг
        if (!decimal.TryParse(numbers[0], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dividend) ||
            !decimal.TryParse(numbers[1], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out divisor))
        {
            return;
        }

        // 5.1. Підписка на подію
        OnZeroDivision += (d, v) => "Ділення на нуль неможливе";
        OnZeroDivision += (d, v) =>
        {
            File.AppendAllText("divide.log", originalInput + "\r\n", System.Text.Encoding.GetEncoding(1251));
            return string.Empty;
        };

        try
        {
            checked
            {
                // 1.1, 1.2. Ділення
                decimal result = dividend / divisor;

                // 4. Форматування та вивід
                result = decimal.Round(result, 4, MidpointRounding.ToZero);
                string formattedResult = result.ToString("F4", new CultureInfo("uk-UA"));
                Console.WriteLine(formattedResult);
            }
        }
        catch (DivideByZeroException)
        {
            // 2.3. Виклик події
            string message = OnZeroDivision?.Invoke(dividend, divisor);
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine(message);
            }

            // 5.3. Код завершення
            Environment.ExitCode = 42;
        }
    }
}