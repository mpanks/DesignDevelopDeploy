using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Coursework
{
    internal class FileHandling
    {
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
                string password = "";
                string salt = "";
                Functions.OutputMessage("Please enter password");
                _password = Functions.GetString();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        password = reader.GetString(1);
                        salt = reader.GetString(2);
                    }
                }
                if (_password+salt == password)
                {
                    //TODO Move to users home screen
                    Console.WriteLine("Hello");
                }
                else { Functions.OutputMessage($"Password incorrect, please try again"); }
                connection.Close();
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
