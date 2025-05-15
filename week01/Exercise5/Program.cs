using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World! This is the Exercise5 Project.");

         Console.Write("Enter a number: ");
        int number = int.Parse(Console.ReadLine());

        int square = Square(number);
        Console.WriteLine($"The square of {number} is {square}");
    }

    static int Square(int n)
    {
        return n * n;
    }
}