﻿using Microsoft.Data.Sqlite;
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
        public userHomeScreen( string pLoginID)
        {
            _loginID = pLoginID;
        }
        public override void CreateMenu()
        {
            this.IsActive = false;
            this.SelectMenu();
        }
        public override string MenuText()
        {
            return "Home Screen";
        }
        public void SelectMenu()
        {
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
                MeetingNotification mn = new MeetingNotification(_loginID, accLvl);
                mn.Select();

                switch (accLvl)
                {
                    case 1:
                        StudentHomeScreen shs = new StudentHomeScreen(_loginID);
                        shs.Select();
                        break;
                    case 2:
                        PSHomeScreen pshs = new PSHomeScreen(_loginID);
                        pshs.Select();
                        break;
                    case 3:
                        STHomeScreen sTHomeScreen = new STHomeScreen(_loginID);
                        sTHomeScreen.Select();
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
        public StudentHomeScreen(string loginID) : base(loginID) { }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new AddProgressReport(_loginID));
            _menuItems.Add(new ViewProgressReports(_loginID, 1));
            _menuItems.Add(new ViewStudentMeetings(1, _loginID));
            _menuItems.Add(new CreateMeeting(1, _loginID));
            _menuItems.Add(new ExitMenuItem(this)); 
        }
        public override string MenuText()
        {
            return "Student Home Page";
        }
        public override void Select()
        {
            base.Select();
        }
    }
    class PSHomeScreen : userHomeScreen, MenuItem
    {
        public PSHomeScreen (string loginID): base(loginID)
        {
        }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new ViewProgressReports(_loginID, 2));
            _menuItems.Add(new ViewStudentMeetings(2,_loginID));
            _menuItems.Add(new ViewStudentMeetings(2, _loginID, true));
            _menuItems.Add(new CreateMeeting(2,_loginID));
            _menuItems.Add(new ManageMeetings(_loginID));
            _menuItems.Add(new ExitMenuItem(this));
        }
        public override string MenuText()
        {
            return "Personal Supervisors Home Screen";
        }
        public override void Select()
        {
            base.Select();
        }
    }
    class STHomeScreen : userHomeScreen
    {
        private string _loginID;
        public STHomeScreen(string loginID) : base(loginID)
        {
            _loginID=loginID;
        }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            //TODO Allow ST to review the reports of a specific student from one of their PSs
            //TODO Allow ST to review all reports from every student under their PSs
            //TODO Allow ST to book meetings with their PSs
            _menuItems.Add(new ExitMenuItem(this));
        }
        public string MenuTEext()
        {
            return "Senior Tutor Home Screen";
        }
        public override void Select()
        {
            base.Select();
        }
    }
}
