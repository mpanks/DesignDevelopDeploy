using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

namespace Coursework
{
    internal class FileHandling
    {
    }
    class Hashing
    {
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
        public bool checkPassword(string inputPassword, string storedHash,string salt)
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
    class FileHandler : ConsoleMenu
    {
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new Login());
            _menuItems.Add(new ExitMenuItem(this));
        }
        public override string MenuText()
        {
            return "Main Menu";
        }
    }

    class Login : MenuItem
    {
        public string _loginID { get; private set; }
        internal string _password { get; private set; }

        public string MenuText()
        {
            return "Login";
        }
        public void Select()
        {
            Functions.OutputMessage("Please enter LoginID");
            string _loginID = Functions.GetString();
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT * FROM UserLogin WHERE loginID=$ID;";
                cmd.Parameters.AddWithValue("$ID", _loginID);
                string hash = "";
                string salt = "";
                Functions.OutputMessage("Please enter password");
                _password = Functions.GetString();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hash = reader.GetString(1);
                        salt = reader.GetString(2);
                    }
                }
                Hashing hashing = new Hashing();
                //salt = hashing.GenerateSalt();
                //_password = hashing.GenerateHash(_password, salt);
                //cmd.CommandText = $"UPDATE UserLogin " +
                //    $"SET password = '{_password}', salt = '{salt}' " +
                //    $"WHERE (loginID = 717402);";
                //Console.WriteLine("\n" + cmd.CommandText);
                //cmd.ExecuteNonQuery();
                //connection.Close();

                if (hashing.checkPassword(_password,hash,salt))
                {
                    //TODO Move to users home screen
                    userHomeScreen uhs = new userHomeScreen(_loginID);
                    uhs.Select();
                }
                else
                {
                    Functions.OutputMessage($"Password incorrect, please try again"); connection.Close();
                }
            }
        }
    }
}
