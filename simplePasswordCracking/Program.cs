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

            Boolean found = false;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hashBytes = sha256Hash.ComputeHash(userPassBytes);
                string hashedUserPassData = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                Console.WriteLine("Hashed password using SHA-256: " + hashedUserPassData);
                
                String[] readText = File.ReadAllLines("../../../dictionary/2of12.txt");
                foreach (String s in readText)
                {
                    byte[] dictionaryItemBytes = Encoding.UTF8.GetBytes(s);
                    byte[] hashDictBytes = sha256Hash.ComputeHash(dictionaryItemBytes);
                    string hashedDictData = BitConverter.ToString(hashDictBytes).Replace("-", String.Empty);
                    
                    if (hashedUserPassData == hashedDictData)
                    {
                        Console.WriteLine("Your password is too easy. Try again.");
                        found = true;
                    }
                }
            }
            
            if (!found)
            {
                Console.WriteLine("Your password was not found. Well done.");
            }
        }
    }
}