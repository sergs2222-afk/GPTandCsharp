

public class DivisionProgram
{
    // Define a delegate type for handling zero division events.
    public delegate string ZeroDivisionHandler(double dividend, double divisor);

    // Define an event based on the delegate.
    public static event ZeroDivisionHandler OnZeroDivision;

    public static void Main(string[] args)
    {
        // Attach a method (lambda expression) to the event.
        // This method will return a user-friendly error message.
        OnZeroDivision += (d, v) => $"Error: Cannot divide {d} by zero.";

        Console.WriteLine("Please enter the dividend:");
        string dividendInput = Console.ReadLine();

        Console.WriteLine("Please enter the divisor:");
        string divisorInput = Console.ReadLine();

        try
        {
            // Parse the input strings into doubles.
            double dividend = double.Parse(dividendInput);
            double divisor = double.Parse(divisorInput);

            // Perform the division inside the try block.
            double result = dividend / divisor;

            // Print the result.
            Console.WriteLine($"Result: {result}");
        }
        catch (DivideByZeroException)
        {
            // Catch the specific division-by-zero exception.
            // Invoke the event, which calls the attached handler to get the message.
            string message = OnZeroDivision?.Invoke(double.Parse(dividendInput), double.Parse(divisorInput));
            Console.WriteLine(message);
        }
        catch (FormatException)
        {
            // Handle cases where the input is not a valid number.
            Console.WriteLine("Invalid input format. Please enter valid numbers.");
        }
        catch (Exception ex)
        {
            // Catch any other unexpected exceptions.
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
