using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
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
                cmd.CommandText = "INSERT INTO ProgressReports(LoginID, ConfidenceLevel, Report, PSID) VALUES(@loginID, @ConfidenceLevel, @Report, " +
                    "(SELECT PSID FROM PSAllocation WHERE StudentID = @loginID));";

                Functions.OutputMessage("Please write a quick overview for how you've managed with the previous month");
                string report = Functions.GetString();
                int confLvl = Functions.GetIntegerInRange(1, 5, "Please select your conficence level between 1 and 5");

                cmd.Parameters.AddWithValue("@loginID", _loginID);
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
        private bool _Select;
        public ViewProgressReports(string loginID, int AccessLvl, bool select = false)
        {
            _loginID = loginID;
            _AccessLvl = AccessLvl;
            _Select = select;
        }
        public string MenuText()
        {
            if (!_Select)
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
                        //Student views reports
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
                        if (_Select)
                        {
                            //Select student reports
                            cmd.CommandText = "SELECT Title, FirstName, LastName, Report, ConfidenceLevel " +
                                "FROM ProgressReports, UserInfo " +
                                "WHERE (ProgressReports.loginID = (SELECT loginID from UserInfo WHERE FirstName = @fname AND LastName = @lname AND accessLevel = 1)) " +
                                "AND (UserInfo.loginID = ProgressReports.loginID) " +
                                "AND ProgressReports.PSID = @loginID;";
                            string[] PSdetails = GetDetails().Split(' ');

                            cmd.Parameters.AddWithValue("@fname", PSdetails[0]);
                            cmd.Parameters.AddWithValue("@lname", PSdetails[1]);
                        }
                        else
                        {
                            //View all
                            cmd.CommandText = "SELECT Title, FirstName, LastName, Report, ConfidenceLevel " +
                                "FROM ProgressReports, UserInfo " +
                                "WHERE ProgressReports.PSID = @loginID " +
                                "AND (userinfo.loginID = progressreports.loginID);";
                        }
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
                        OutputDetails(cmd);
                        break;
                    case 3:
                        //PS and ST views reports
                        if (_Select)
                        {
                            //Select student reports
                            cmd.CommandText = "SELECT Title, FirstName, LastName, Report, ConfidenceLevel " +
                                "FROM ProgressReports, UserInfo " +
                                "WHERE (progressReports.loginID IN (SELECT loginID from UserInfo WHERE FirstName = @fname AND LastName = @lname AND accessLevel=1)) " +
                                "AND PSID = (SELECT PSID FROM STAllocation WHERE STID = @loginID AND PSID = (SELECT PSID FROM PSAllocation WHERE StudentID = progressReports.loginID) ) " +
                                "AND (UserInfo.loginID = ProgressReports.loginID);";
                            string[] PSdetails = GetDetails().Split(' ');

                            cmd.Parameters.AddWithValue("@fname", PSdetails[0]);
                            cmd.Parameters.AddWithValue("@lname", PSdetails[1]);
                        }
                        else
                        {
                            //View all
                            cmd.CommandText = "SELECT Title,FirstName, LastName, Report, ConfidenceLevel " +
                                "FROM ProgressReports, UserInfo " +
                                "WHERE (progressreports.PSID = (SELECT PSID FROM PSAllocation WHERE PSID = (SELECT PSID FROM STAllocation WHERE STID = @loginID))) " +
                                "AND (userinfo.loginID = progressreports.loginID);";
                        }
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
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
