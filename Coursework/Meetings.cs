using Microsoft.Data.Sqlite;
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
    class CreateMeeting: MenuItem
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
        public ViewMeetings(int accessLevel, string loginID)
        {
            _accessLevel = accessLevel;
            _loginID = loginID;
        }
        public string MenuText()
        {
            return "View meetings";
        }
        public void Select()
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                switch (_accessLevel)
                {
                    case 1: //TODO Add date to DB for meetings
                        cmd.CommandText = "SELECT firstName, lastName, Title, location, time " +
                            "FROM studentMeeting, UserInfo " +
                            "WHERE studentID = @loginID " +
                            "AND PSID = UserInfo.loginID;";
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
                        break;
                    case 2:
                        cmd.CommandText = "SELECT firstname, lastname, title, location, time " +
                            "FROM studentMeeting, userInfo " +
                            "WHERE PSID = @loginID " +
                            "AND studentID = UserInfo.loginID;";
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
                using(var sr  = cmd.ExecuteReader())
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
