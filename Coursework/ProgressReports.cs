﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class ProgressReports
    {
    }
    class AddProgressReport : MenuItem
    {
        internal string _loginID { get; private set; }
        public AddProgressReport(string loginID)
        {
            _loginID = loginID;
        }
        public string MenuText()
        {
            return "Add New Progress Report";
        }
        public void Select()
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO ProgressReports(LoginID, ConfidenceLevel, Report) VALUES(@LoginID, @ConfidenceLevel, @Report);";

                Functions.OutputMessage("Please write a quick overview for how you've managed with the previous month");
                string report = Functions.GetString();
                int confLvl = Functions.GetIntegerInRange(1, 5, "Please select your conficence level between 1 and 5");

                cmd.Parameters.AddWithValue("@LoginID", _loginID);
                cmd.Parameters.AddWithValue("@Report", report);
                cmd.Parameters.AddWithValue("@ConfidenceLevel", confLvl);

                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
    class ViewProgressReports : MenuItem
    {
        internal string _loginID { get; private set; }
        private int _AccessLvl;
        private bool _Student;
        public ViewProgressReports(string loginID, int AccessLvl, bool student = false)
        {
            _loginID = loginID;
            _AccessLvl = AccessLvl;
            _Student = student;
        }
        public string MenuText()
        {
            if (_Student == true)
            {
                return "View All Progress Reports";
            }
            else
            {
                return "Select Student's Progress Reports";
            }
        }
        public void Select()
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                switch (_AccessLvl)
                {
                    case 1:
                        cmd.CommandText = "SELECT Report, ConfidenceLevel FROM ProgressReports WHERE (LoginID = @loginID);";
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
                        using (var sr = cmd.ExecuteReader())
                        {
                            while (sr.Read())
                            {
                                Functions.OutputMessage($"{sr.GetName(0)}: {sr.GetString(0)}\n{sr.GetName(1)}: {sr.GetString(1)}\n");
                            }
                        }
                        break;
                    case 2:
                        cmd.CommandText = "SELECT Title, FirstName, LastName, Report, ConfidenceLevel " +
                            "FROM ProgressReports, UserInfo " +
                            "WHERE (PSID = @login) " +
                            "AND (UserInfo.loginID = ProgressReports.loginID);";
                        cmd.Parameters.AddWithValue("@login", _loginID);
                        OutputDetails(cmd);
                        break;
                    case 3:
                        if (_Student == true)
                        {
                            cmd.CommandText = "SELECT Title, FirstName, LastName, Report, ConfidenceLevel " +
                                "FROM ProgressReports, UserInfo " +
                                "WHERE (PSID = (SELECT loginID from UserInfo WHERE FirstName = @fname AND LastName = @lname AND accessLevel = 2)) " +
                                "AND (UserInfo.loginID = ProgressReports.loginID);";
                            string[] PSdetails = GetDetails().Split(' ');

                            cmd.Parameters.AddWithValue("@fname", PSdetails[0]);
                            cmd.Parameters.AddWithValue("@lname", PSdetails[1]);
                        }
                        else
                        {
                            cmd.CommandText = "SELECT Title,FirstName, LastName, Report, ConfidenceLevel " +
                                "FROM ProgressReports, UserInfo " +
                                "WHERE (progressreports.loginID = (SELECT loginID FROM userInfo WHERE FirstName = @fname AND LastName = @lname AND accessLevel = 1)) " +
                                "AND (userinfo.loginID = progressreports.loginID);";
                            string[] StudentDetails = GetDetails().Split(" ");

                            cmd.Parameters.AddWithValue("@fname", StudentDetails[0]);
                            cmd.Parameters.AddWithValue("@lname", StudentDetails[1]);
                        }
                        OutputDetails(cmd);
                        break;
                    default:
                        break;
                }
            }
        }
        private void OutputDetails(SqliteCommand cmd)
        {
            using (var sr = cmd.ExecuteReader())
            {
                while (sr.Read())
                {
                    Functions.OutputMessage($"{sr.GetString(0)} {sr.GetString(1)} {sr.GetString(2)}\n{sr.GetName(3)}: {sr.GetString(3)}\n{sr.GetName(4)}: {sr.GetString(4)}\n");
                }
            }
        }
        private string GetDetails()
        {
            string output = string.Empty;
            output += Functions.GetString("Please enter the persons first name");
            output += " " + Functions.GetString("Please enter the persons last name");
            return output;
        }
    }
}
