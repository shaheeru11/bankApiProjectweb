using System;
using System.Collections.Generic;
using System.Text;

namespace bankApiProject.Controllers
{
    public class TokenizationUtility
    {
        private readonly Random random = new Random();
        public int key = 3;
        public int oddKey = 62;

        public TokenizationUtility()
        {
            key = 3;
            oddKey = 62;
        }

        public string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] randomString = new char[20];
            for (int i = 0; i < 20; i++)
            {
                randomString[i] = chars[random.Next(chars.Length)];
            }
            return new string(randomString);
        }

        public string EncryptString(string input)
        {
            string randomString = GenerateRandomString();
            input = randomString + input; // Concatenating random string with input
            char[] arr = input.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    arr[i] = (char)(arr[i] + key);
                }
                else
                {
                    arr[i] = (char)(arr[i] - oddKey);
                }
            }
            return new string(arr);
        }

        public string DecryptString(string input)
        {
            char[] arr = input.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (i % 2 == 0)
                {
                    arr[i] = (char)(arr[i] - key);
                }
                else
                {
                    arr[i] = (char)(arr[i] + oddKey);
                }
            }
            // Removing the first 20 characters which represent the random string
            return new string(arr, 20, arr.Length - 20);
        }
    }
}
