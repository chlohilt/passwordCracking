using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace passwordCracking
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input your password in plain text (NEVER do this lol)");
            String userPass = Console.ReadLine();
            byte[] userPassBytes = Encoding.UTF8.GetBytes(userPass);

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hashBytes = sha256Hash.ComputeHash(userPassBytes);
                string hashedData = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                Console.WriteLine("Hashed password using SHA-256: " + hashedData);
            }
            
            String[] readText = File.ReadAllLines("../../../dictionary/2of12.txt");
            Boolean found = false;
            foreach (String s in readText)
            {
                if (userPass == s)
                {
                    Console.WriteLine("Your password is too easy. Try again.");
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("Your password was not found. Well done.");
            }
        }
    }
}