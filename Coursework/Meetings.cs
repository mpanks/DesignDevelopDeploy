﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class Meetings
    {
    }
    class CreateMeeting : MenuItem
    {
        private int _accessLevel;
        private string _loginID;
        public CreateMeeting(int accLvl, string loginID)
        {
            _accessLevel = accLvl;
            _loginID = loginID;
        }
        public string MenuText()
        {
            return "Book meeting";
        }
        public void Select()
        {
            //TODO Implement create meeting 
        }
    }
    class ViewMeetings : MenuItem
    {
        private int _accessLevel;
        private string _loginID;
        private bool _select;
        public ViewMeetings(int accessLevel, string loginID, bool selectStudent = false)
        {
            _accessLevel = accessLevel;
            _loginID = loginID;
            _select = selectStudent;
        }
        public string MenuText()
        {
            if (!_select)
            {
                return "View meetings";
            }
            else { return "Select Student Meetings"; }
        }
        public void Select()
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT firstName, lastName, Title, location, time " +
                            "FROM studentMeeting, UserInfo ";
                switch (_accessLevel)
                {
                    case 1: //TODO Add date to DB for meetings
                        cmd.CommandText += "WHERE studentID = @loginID " +
                            "AND PSID = UserInfo.loginID;";
                        break;
                    case 2:
                        cmd.CommandText += "WHERE PSID = @loginID AND studentID = UserInfo.loginID;";
                        if (_select)
                        {
                            cmd.CommandText += "AND studentID = (SELECT LoginID FROM UserInfo WHERE firstname = @fname AND lastname = @lname AND accessLevel = 1);";
                            Functions.OutputMessage("Please input students firstname");
                            string fname = Functions.GetString();

                            Functions.OutputMessage("Please input students last name");
                            string lname = Functions.GetString();

                            cmd.Parameters.AddWithValue("@fname", fname);
                            cmd.Parameters.AddWithValue("@lname", lname);
                        }
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
                cmd.Parameters.AddWithValue("@loginID", _loginID);
                using (var sr = cmd.ExecuteReader())
                {
                    while (sr.Read())
                    {
                        Functions.OutputMessage($"Meeting with {sr.GetString(2)} {sr.GetString(0)} {sr.GetString(1)}:\n{sr.GetName(3)}: {sr.GetString(3)}\n{sr.GetName(4)}: {sr.GetString(4)}\n");
                    }
                }
            }
        }
    }
}
