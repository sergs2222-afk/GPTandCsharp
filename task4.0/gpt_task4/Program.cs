using System;
using System.Globalization;

class Program
{
    // Declare delegate type
    public delegate string DivisionErrorHandler(decimal dividend, decimal divisor);

    // Event using the delegate
    public static event DivisionErrorHandler OnDivisionError;

    static void Main()
    {
        try
        {
            // Read dividend
            string? dividendInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dividendInput)) return;
            decimal dividend = decimal.Parse(dividendInput, CultureInfo.InvariantCulture);

            // Read divisor
            string? divisorInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(divisorInput)) return;
            decimal divisor = decimal.Parse(divisorInput, CultureInfo.InvariantCulture);

            decimal result;

            checked
            {
                result = dividend / divisor; // may throw DivideByZeroException
            }

            // Print result
            Console.WriteLine(result.ToString(CultureInfo.InvariantCulture));
        }
        catch (DivideByZeroException)
        {
            // Subscribe a handler
            OnDivisionError += (a, b) => "Error: Division by zero is not allowed.";

            // Call delegate and print result
            string message = OnDivisionError?.Invoke(0, 0) ?? "Unknown error";
            Console.WriteLine(message);
        }
    }
}
