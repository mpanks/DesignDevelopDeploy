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
    class MeetingNotification : MenuItem
    {
        private string _loginID;
        private int _accLvl;
        public MeetingNotification(string loginID, int AccessLevel)
        {
            _loginID = loginID;
            _accLvl = AccessLevel;
        }
        public string MenuText()
        {
            return "Meeting Notifications";
        }
        public void Select()
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT title, firstname, lastname, time, location " +
                        "FROM userinfo, studentMeeting ";
                        //"AND userInfo.loginID = studentMeeting.StudentID " +
                        //"AND userInfo.accessLevel = @AccLvl " +
                    switch (_accLvl)
                    {
                        case 1:
                            cmd.CommandText += "WHERE userInfo.loginID = PSID AND studentID = @loginID AND studentConfirmed = 0;";
                            break;
                        case 2:
                            cmd.CommandText += "WHERE userInfo.loginID = studentID AND PSID = @loginID AND PSconfirmed = 0;";
                            break;
                        default:
                            break;
                    }
                    cmd.Parameters.AddWithValue("@loginID", _loginID);

                    using (var sr = cmd.ExecuteReader())
                    {
                        while (sr.Read())
                        {
                            Functions.OutputMessage($"You have an unviewed meeting with {sr.GetString(0)} {sr.GetString(1)} {sr.GetString(2)}:" +
                                $"\n{sr.GetName(3)}: {sr.GetString(3)}\n{sr.GetName(4)}: {sr.GetString(4)}");
                        }
                    }
                }

                var update = connection.CreateCommand();
                switch (_accLvl)
                {
                    case 1:
                        update.CommandText ="UPDATE studentMeeting SET studentConfirmed = 1 WHERE studentID = @loginID;";
                        break;
                    case 2:
                        update.CommandText = "UPDATE studentMeeting SET PSconfirmed = 1 WHERE PSID = @loginID;";
                        break;
                    default:
                        break;
                }
                update.Parameters.AddWithValue("@loginID", _loginID);
                update.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
    class CreateMeeting : MenuItem
    {
        private int _accessLevel;
        private string _loginID;
        private string _location;
        private string _time;
        private string _otherID;
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
            //TODO check if room is available at that time
            //"Teams" isnt a "room" and can have overlapping times IF both parties are free at that time
            //Create list of times that are taken, search list for the selected time
            List<string> times = new List<string>();
            GetMeetingDetails(_accessLevel);
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO studentMeeting(studentID, PSID, location, time) VALUES(@studentID, @PSID, @location, @time);";
                switch (_accessLevel)
                {
                    case 1:
                        //Student creates meeting
                        GetCurrentMeetings(times, _loginID, _otherID);
                        if (!times.Contains(_time))
                        {
                            cmd.Parameters.AddWithValue("@studentID", _loginID);
                            cmd.Parameters.AddWithValue("@PSID", _otherID);
                        }
                        else
                        {
                            Functions.OutputMessage($"Cannot create a meeting at {_time} as there is already a meeting booked then");
                        }
                        break;
                    case 2:
                        //PS creates meeting
                        GetCurrentMeetings(times, _otherID, _loginID);
                        if (!times.Contains(_time))
                        {
                            cmd.Parameters.AddWithValue("@studentID", _otherID);
                            cmd.Parameters.AddWithValue("@PSID", _loginID);
                        }
                        else
                        {
                            Functions.OutputMessage($"Cannot create a meeting at {_time} as there is already a meeting booked then");
                        }
                        break;
                    default:
                        break;
                }
                if (cmd.Parameters.Count > 3)
                {
                    cmd.Parameters.AddWithValue("@location", _location);
                    cmd.Parameters.AddWithValue("@time", _time);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        Functions.OutputMessage($"Meeting created for {_time} in {_location}");
                    }
                    else
                    {
                        Functions.OutputMessage($"Unable to create a meeting for {_time} in {_location}");
                    }
                }
                else
                {
                    Functions.OutputMessage($"Unable to create a meeting for {_time} in {_location}");
                }
            }
        }
        private void GetCurrentMeetings(List<string> times, string studentID, string PSID)
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT time FROM studentMeeting WHERE studentID = @studentID AND PSID = @PSID;";
                cmd.Parameters.AddWithValue("@studentID", studentID);
                cmd.Parameters.AddWithValue("@PSID", PSID);
                using (var sr = cmd.ExecuteReader())
                {
                    while (sr.Read())
                    {
                        times.Add(sr.GetString(0));
                    }
                }
            }
        }
        private void GetMeetingDetails(int accLvl)
        {
            Functions.OutputMessage("Please input other parties first name");
            string fname = Functions.GetString();

            Functions.OutputMessage("Please input other parties last name");
            string lname = Functions.GetString();

            Functions.OutputMessage("Please choose a location for the meeting (choose \"Teams\" for \"virtual\" meetings)");
            _location = Functions.GetString();

            Functions.OutputMessage("Please choose a time for the meeting (HH:MM)");
            _time = Functions.GetString();

            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT loginID FROM UserInfo WHERE firstname = @fname AND lastname = @lname AND accessLevel = @accLvl;";
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@lname", lname);
                int accessLvl = 0;
                switch (accLvl)
                {
                    case 1:
                        accessLvl = 2;
                        break;
                    case 2:
                        accessLvl = 1;
                        break;
                    default:
                        break;
                }
                cmd.Parameters.AddWithValue("@accLvl", accessLvl);

                using (var sr = cmd.ExecuteReader())
                {
                    while (sr.Read())
                    {
                        _otherID = sr.GetString(0);
                    }
                }
                connection.Close();
            }
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
