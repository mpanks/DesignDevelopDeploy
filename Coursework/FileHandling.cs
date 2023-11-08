﻿using System;
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
