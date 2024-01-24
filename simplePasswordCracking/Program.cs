using System;
using System.IO;

namespace passwordCracking
{
    class Program
    {
        static void Main(string[] args)
        {
            string userPass = Console.ReadLine();
            Console.WriteLine(userPass);
            string[] readText = File.ReadAllLines("../../../dictionary/2of12.txt");
            foreach (string s in readText)
            {
                if (userPass == s)
                {
                    Console.WriteLine("Your password is too easy. Try again.");
                }
            }
        }
    }
}