using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World! This is the Exercise4 Project.");

         List<string> fruits = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            Console.Write("Enter a fruit: ");
            fruits.Add(Console.ReadLine());
        }

        Console.WriteLine("You entered:");
        foreach (string fruit in fruits)
        {
            Console.WriteLine(fruit);
        }
    }
}