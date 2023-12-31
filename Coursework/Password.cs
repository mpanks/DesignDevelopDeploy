﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Coursework
{
    class Hashing
    {
        private string _salt;
        private string _hash;
        public Hashing() { }

        private string GenerateHash(string password, string salt)
        {
            using (SHA256 mySha256 = SHA256.Create())
            {
                UTF32Encoding encoder = new UTF32Encoding();
                byte[] saltBytes = encoder.GetBytes(salt);
                byte[] passwordBytes = encoder.GetBytes(password);
                byte[] hash = mySha256.ComputeHash(encoder.GetBytes(password + salt));
                return _hash = Convert.ToBase64String(hash);
            }
        }
        public bool checkPassword(string inputPassword, string storedHash, string salt)
        {
            string inputHash = GenerateHash(inputPassword, salt);
            if (storedHash == inputHash)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
