using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //public override void CreateMenu()
        //{
        //    _menuItems.Clear();
        //}
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
        public ViewProgressReports(string loginID)
        {
            _loginID = loginID;
        }
        public string MenuText()
        {
            return "View Progress Reports";
        }
        public void Select()
        {
            //TODO Implement view progress reports function
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Report, ConfidenceLevel FROM ProgressReports WHERE (LoginID = @loginID);";//Add SQL Query
                cmd.Parameters.AddWithValue("@loginID", _loginID);
                cmd.ExecuteNonQuery ();
                using (var sr = cmd.ExecuteReader())
                {
                    while(sr.Read())
                    {
                        Functions.OutputMessage($"{sr.GetName(0)}: {sr.GetString(0)}\n{sr.GetName(1)}: {sr.GetString(1)}\n");
                    }
                }
            }
        }
    }
}
