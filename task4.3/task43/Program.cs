using System;
using System.Globalization;
using System.IO;
using System.Text;

public delegate string ZeroDivisionHandler(decimal dividend, decimal divisor);

class Program
{
    public static event ZeroDivisionHandler OnZeroDivision;
    static string originalInput = "";

    static void Main()
    {
        // Підписка двох обробників на подію
        OnZeroDivision += (dividend, divisor) => "Ділення на нуль неможливе";
        OnZeroDivision += (dividend, divisor) =>
        {
            File.AppendAllText("divide.log", originalInput + "\r\n", Encoding.GetEncoding(1251));
            return "";
        };

        // Зчитування одного рядка
        originalInput = Console.ReadLine();

        // Нормалізація та розділення
        string normalized = originalInput
            .Replace('−', '-')                 // Юнікод мінус → звичайний мінус
            .Replace("\u00A0", "")             // Нерозривний пробіл
            .Replace("\u202F", "")             // Вузький нерозривний пробіл
            .Replace(" ", "")                  // Пробіл
            .Replace("'", "");                 // апостроф

        string[] parts = normalized.Split(new[] { ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            return; // некоректний ввід, програма завершить роботу без виводу

        if (!decimal.TryParse(parts[0], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out decimal numA))
            return;
        if (!decimal.TryParse(parts[1], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out decimal numB))
            return;

        try
        {
            decimal result;
            checked
            {
                result = numA / numB; // може кинути DivideByZeroException
            }

            // Вивід з форматуванням
            Console.WriteLine(result.ToString("F4", new CultureInfo("uk-UA")));
        }
        catch (DivideByZeroException)
        {
            string message = OnZeroDivision?.Invoke(numA, numB);
            if (!string.IsNullOrEmpty(message))
                Console.WriteLine(message);

            Environment.ExitCode = 42;
        }
    }
}

