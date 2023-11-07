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
        public string _salt { get; private set; }
        public string _hash { get; private set; }
        public Hashing() { }
        public string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[10];
            rng.GetBytes(buffer);

            return _salt = Convert.ToBase64String(buffer);
        }
        public string GenerateHash(string password, string salt)
        {
            using (SHA256 mySha256 = SHA256.Create())
            {
                UTF32Encoding encoder = new UTF32Encoding();
                byte[] saltBytes = encoder.GetBytes(salt);
                byte[] passwordBytes = encoder.GetBytes(password);
                byte[] hash = mySha256.ComputeHash(encoder.GetBytes(password + salt));
                Console.WriteLine(password + salt);
                //string hashString = string.Empty;
                //for (int i = 0; i < hash.Length; i++)
                //{
                //    hashString += hash[i];
                //}
                //Console.WriteLine(hashString.ToString());
                return _hash = Convert.ToBase64String(hash);
            }
        }
        public bool checkPassword(string inputPassword, string storedHash,string salt)
        {
            string inputHash = GenerateHash(inputPassword, salt);
            Console.WriteLine("InputHash: " + inputHash);
            Console.WriteLine("StoredHash: " + storedHash);
            if (storedHash == inputHash)
            {
                //TODO Remove outputs

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
    }

    public class Login : MenuItem
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
                Console.WriteLine("InputtedPassword: " + _password);
                Console.WriteLine("Stored Salt: " + salt);
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
                    Console.WriteLine("Hello");
                }
                else
                {
                    Functions.OutputMessage($"Password incorrect, please try again"); connection.Close();
                }
            }
            Console.WriteLine("Really hope that worked");

            //string fileName = "LoginInfo.json";

            //List<LoginInfo> lines;
            //using (StreamReader streamReader = new StreamReader(fileName))
            //{
            //    string json = streamReader.ReadToEnd();
            //    lines = JsonConvert.DeserializeObject<List<LoginInfo>>(json);
            //    Console.WriteLine(lines);
            //}
            //if (lines[0].Contains(_loginID))
            //{
            //    int index = lines.IndexOf(_loginID);
            //    string passwordInput = Functions.GetString();
            //    _password = lines[index].ToString();
            //    if (_password == passwordInput)
            //    {
            //        Console.WriteLine("Im in");
            //    }
            //}
            //else
            //{
            //    Functions.OutputMessage($"{_loginID} is not found");
            //}
        }
    }
}
