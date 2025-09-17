using System;

// Ask user to enter the value of x
Console.Write("Enter value of x: ");
double x = double.Parse(Console.ReadLine());

// Step size: e / 20
double step = Math.E / 20.0;

// Print table header
Console.WriteLine("+-----------+----------------+----------------+");
Console.WriteLine("|     a     |      x^a       |      a^x       |");
Console.WriteLine("+-----------+----------------+----------------+");

// Loop from 0 to e with step = e/20
for (double a = 0; a <= Math.E + 1e-10; a += step) // small epsilon for floating-point
{
    string valueXpowA;
    string valueApowX;

    // Calculate x^a (always valid)
    valueXpowA = Math.Pow(x, a).ToString("F6");

    // Calculate a^x (but only if a > 0, since log(0) is undefined for Math.Pow)
    if (a <= 0 && x != 0)   /// (!!!) 0^0 == 1
    {
        valueApowX = "undefined";
    }
    else
    {
        valueApowX = Math.Pow(a, x).ToString("F6");
    }

    // Print table row
    Console.WriteLine($"|{a,11:F6}|{valueXpowA,16}|{valueApowX,16}|");
    Console.WriteLine("+-----------+----------------+----------------+");
 
}
Console.ReadLine();
