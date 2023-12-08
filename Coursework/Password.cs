using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Coursework
{
    internal class Password
    {
    }
    class Hashing
    {
        //TODO Encapsulate hashing class before final commit
        internal string _salt { get; private set; }
        internal string _hash { get; private set; }
        public Hashing() { }
        private string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[10];
            rng.GetBytes(buffer);

            return _salt = Convert.ToBase64String(buffer);
        }
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
