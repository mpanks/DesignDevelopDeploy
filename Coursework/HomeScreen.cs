﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    class userHomeScreen : ConsoleMenu, MenuItem
    {
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
            _menuItems.Add(new ViewMeetings(1, _loginID));
            _menuItems.Add(new CreateMeeting(1, _loginID));
            _menuItems.Add(new ManageMeetings(1, _loginID));
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
            _menuItems.Add(new ViewProgressReports(_loginID, 2, true));
            _menuItems.Add(new ViewMeetings(2,_loginID));
            _menuItems.Add(new ViewMeetings(2, _loginID, true));
            _menuItems.Add(new CreateMeeting(2,_loginID));
            _menuItems.Add(new ManageMeetings(2,_loginID));
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
        public STHomeScreen(string loginID) : base(loginID)
        {
        }
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new ViewProgressReports(_loginID, 3));
            _menuItems.Add(new ViewProgressReports(_loginID, 3, true));
            _menuItems.Add(new CreateMeeting(3,_loginID));
            _menuItems.Add(new ViewMeetings(3, _loginID));
            _menuItems.Add(new ViewMeetings(3,_loginID,true));
            _menuItems.Add(new ManageMeetings(3,_loginID));
            _menuItems.Add(new ExitMenuItem(this));
        }
        public override string MenuText()
        {
            return "Senior Tutor Home Screen";
        }
        public override void Select()
        {
            base.Select();
        }
    }
}
