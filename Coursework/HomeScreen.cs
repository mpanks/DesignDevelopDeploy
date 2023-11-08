using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class HomeScreen
    {
    }
    class userHomeScreen : ConsoleMenu
    {
        protected  ConsoleMenu menu { get { return this; } private set { } }
        protected int accLvl {get; private set;}
        protected string _loginID { get; private set;}
        public userHomeScreen( string pLoginID = "")
        {
            _loginID = pLoginID;
            menu = this;
        }
        public override void CreateMenu()
        {
            this.SelectMenu();
        }
        public override string MenuText()
        {
            return "Home Screen";
        }
        public void SelectMenu()
        {
            //base.Select();
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT AccessLevel FROM UserInfo WHERE loginID = @loginID";
                cmd.Parameters.AddWithValue("@loginID", _loginID);

                cmd.ExecuteNonQuery();
                using (var sr = cmd.ExecuteReader())
                {
                    string userData = string.Empty;
                    while (sr.Read())
                    {
                        accLvl = sr.GetInt32(0);
                        Console.WriteLine(accLvl);
                    }
                }
                switch (accLvl)
                {
                    case 1:
                        StudentHomeScreen shs = new StudentHomeScreen(_loginID);
                        shs.Select();
                        break;
                    case 2:
                        PSHomeScreen pshs = new PSHomeScreen();
                        pshs.Select();
                        break;
                    case 3:
                        //TODO Divert to ST homescreen
                        break;
                    case 4:
                        //TODO Divert to admin homescreen?
                        break;
                    default:
                        Functions.OutputMessage("Error, please try again");
                        break;

                }
            }
        }
    }
    class StudentHomeScreen : userHomeScreen, MenuItem
    { 
        private string pLoginID;
        public StudentHomeScreen(string loginID)
        {
            pLoginID = loginID;
        }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            //TODO Add menu options for Student home screen
            _menuItems.Add(new ExitMenuItem(menu));
        }
        public override string MenuText()
        {
            return "Student Home Page";
        }
        public override void Select()
        {
            //CreateMenu();
            base.Select();
            Console.WriteLine("Student Home Page");
        }
    }
    class PSHomeScreen : userHomeScreen, MenuItem
    {
        public override void CreateMenu()
        {
            _menuItems.Clear();
            //TODO Add menu items for PS home screen
            _menuItems.Add(new ExitMenuItem(this));
        }
        public override string MenuText()
        {
            return "Personal Supervisors Home Screen";
        }
        public override void Select()
        {
            base.Select();
            Console.WriteLine("Welcome to PS homescreen");
        }
    }
}
