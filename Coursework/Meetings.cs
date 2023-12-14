using Coursework;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Coursework
{
    internal class DateTimeValidation
    {
        public static void ValidateDateTime(ref TimeOnly _time, ref string dateString)
        {
            bool check = false;
            bool exit = false;
            bool getDate = false;
            do
            {
                try
                {
                    do
                    {
                        check = false;
                        string timeString = Functions.GetString("Please choose a time for the meeting (HH:MM), or -1 to exit");

                        if (timeString == "" || timeString.Contains(" "))
                        {
                            Functions.OutputMessage("Time value cannot be left blank, or contain any spaces/white space characters");
                            check = true;
                        }
                        else if (timeString == "-1")
                        {
                            exit = true;
                            Functions.OutputMessage("Exiting");
                            break;
                        }
                        else
                        {
                            _time = TimeOnly.Parse(timeString + ":00");
                            dateString = Functions.GetString("Please choose date for the meeting (DD-MM-YY)");

                            if (dateString == "" || dateString == " ")
                            {
                                Functions.OutputMessage("Date value cannot be blank, or contain any spaces/white space characters");
                                check = true;
                            }
                            else
                            {
                                // _date = DateOnly.Parse(dateString);
                                DateTime dateTime = DateTime.Parse(dateString);
                                dateString = dateTime.ToString("yyyy-MM-dd");
                                //  Console.WriteLine(_date + " " + _time);
                                getDate = false;
                            }
                        }
                    } while (check);
                }
                catch (Exception e) { Console.WriteLine("Unrecognised format for date, please try again " + e.Message); getDate = true; }
            } while (getDate && !exit);
        }
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
                    cmd.CommandText = "";
                    //"AND userInfo.loginID = studentMeeting.StudentID " +
                    //"AND userInfo.accessLevel = @AccLvl " +
                    switch (_accLvl)
                    {
                        case 1:
                            cmd.CommandText += "SELECT title, firstname, lastname, date, time, location " +
                                "FROM userinfo, studentMeeting WHERE userInfo.loginID = PSID AND studentID = @loginID AND studentConfirmed = 0;";
                            break;
                        case 2:
                            cmd.CommandText += "SELECT title, firstname, lastname, studentMeeting.date, studentMeeting.time, studentMeeting.location " +
                                "FROM userinfo, studentmeeting, stmeeting " +
                                "WHERE (userInfo.loginID = studentID OR userinfo.loginID = STID) " +
                                "AND (studentmeeting.PSID = @loginID OR stmeeting.PSID = @loginID1) " +
                                "AND (studentmeeting.PSconfirmed = 0 OR stmeeting.psconfirmed = 0);";
                            cmd.Parameters.AddWithValue("@loginID1", _loginID);
                            break;
                        case 3:
                            cmd.CommandText += "SELECT title, firstname, lastname, date,time,location " +
                                "FROM userinfo, stMeeting " +
                                "WHERE UserInfo.loginID = PSID AND STID = @loginID and STconfirmed = 0";
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
                                $"\n{sr.GetName(3)}: {sr.GetString(3)}\n{sr.GetName(4)}: {sr.GetString(4)}\n{sr.GetName(5)}: {sr.GetString(5)}");
                        }
                    }
                }

                var update = connection.CreateCommand();
                switch (_accLvl)
                {
                    case 1:
                        update.CommandText = "UPDATE studentMeeting SET studentConfirmed = 1 WHERE studentID = @loginID;";
                        update.Parameters.AddWithValue("@loginID", _loginID);
                        break;
                    case 2:
                        update.CommandText = "UPDATE studentMeeting SET PSconfirmed = 1 WHERE PSID = @loginID; " +
                            "UPDATE stMeeting SET PSconfirmed = 1 WHERE psid = @psid;";
                        update.Parameters.AddWithValue("@psid", _loginID);
                        update.Parameters.AddWithValue("@loginID", _loginID);
                        break;
                    case 3:
                        update.CommandText = "UPDATE stMeeting set STConfirmed = 1 WHERE stid = @stid";
                        update.Parameters.AddWithValue("@stid", _loginID);
                        break;
                    default:
                        break;
                }
                update.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
    class MeetingClass
    {
        public MeetingClass()
        { }
        protected bool checkPS(SqliteCommand cmd, ref string PSID, ref string _dateString, ref TimeOnly _time, string meetingID = "-1")
        {
            cmd.CommandText = "SELECT meetingID FROM studentMeeting " +
            "WHERE PSID = @PSID " +
            "AND date = @date " +
            "AND time = @time " +
            "AND meetingID != @meetingID;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PSID", PSID);
            cmd.Parameters.AddWithValue("@date", _dateString);
            cmd.Parameters.AddWithValue("@time", _time+":00");
            cmd.Parameters.AddWithValue("@meetingID", meetingID);

            if (cmd.ExecuteScalar() != null)
            {
                return false;
            }
            else
            {
                cmd.CommandText = "SELECT * FROM stMeeting " +
                "WHERE PSID = @PSID " +
                "AND date = @date " +
                "AND time = @time " +
                "AND meetingID != @meetingID;";

                if (cmd.ExecuteScalar() != null)
                {
                    return false;
                }
                else return true;
            }
        }
        protected bool checkST(SqliteCommand cmd, ref string STID, ref string _dateString, ref TimeOnly _time, string meetingID = "-1")
        {
            cmd.CommandText = "SELECT * FROM STMeeting " +
            "WHERE PSID = @STID " +
            "AND date = @date " +
            "AND time = @time " +
            "AND meetingID != @meetingID;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@STID", STID);
            cmd.Parameters.AddWithValue("@date", _dateString);
            cmd.Parameters.AddWithValue("@time", _time+":00");
            cmd.Parameters.AddWithValue("@meetingID", meetingID);

            if (cmd.ExecuteScalar() != null)
            {
                return false;
            }
            else return true;
        }
        protected bool checkStudent(SqliteCommand cmd, ref string studentID, ref string _dateString, ref TimeOnly _time, string meetingID = "-1")
        {
            cmd.CommandText = "SELECT meetingID FROM studentMeeting " +
            "WHERE studentID = @studentID " +
            "AND date = @date " +
            "AND time = @time " +
            "AND meetingID != @meetingID;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@studentID", studentID);
            cmd.Parameters.AddWithValue("@date", _dateString);
            cmd.Parameters.AddWithValue("@time", _time + ":00");
            cmd.Parameters.AddWithValue("@meetingID", meetingID);

            if (cmd.ExecuteScalar() != null)
            {
                return false;
            }
            else return true;
        }
        protected string getOtherID(ref string fname, ref string lname, ref int accLvl, ref string _loginID)
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                switch (accLvl)
                {
                    case 1:
                        //Student doesn't need to search for PS
                        cmd.CommandText = "SELECT PSID FROM PSAllocation WHERE studentID = @loginID;";
                        break;
                    case 2:
                        cmd.CommandText = "SELECT loginID FROM UserInfo WHERE firstname = @fname AND lastname = @lname " +
                                          "AND (loginID IN (SELECT StudentID FROM PSAllocation WHERE PSID = @loginID) " +
                                          "OR loginID IN (SELECT STID FROM STAllocation WHERE PSID = @loginID));";

                        cmd.Parameters.AddWithValue("@fname", fname);
                        cmd.Parameters.AddWithValue("@lname", lname);
                        //cmd.Parameters.AddWithValue("@accLvl", 1);
                        break;
                    case 3:
                        cmd.CommandText = "SELECT loginID FROM UserInfo WHERE firstname = @fname AND lastname = @lname AND accessLevel = 2 " +
                            "AND loginID IN (SELECT PSID FROM STAllocation WHERE STID = @loginID);";
                        cmd.Parameters.AddWithValue("@fname", fname);
                        cmd.Parameters.AddWithValue("@lname", lname);
                        //cmd.Parameters.AddWithValue("@accLvl", 2);
                        break;
                    default:
                        //Should never happen
                        Console.WriteLine("Error, Please try again");
                        break;
                }
                cmd.Parameters.AddWithValue("@loginID", _loginID);
                string _otherID = "-1";
                using (var sr = cmd.ExecuteReader())
                {
                    while (sr.Read())
                    {
                        _otherID = sr.GetString(0);
                    }
                }
                if (_otherID == null)
                {
                    Functions.OutputMessage("Unrecognised user, please check the spelling of the name and try again");
                }
                connection.Close();
                return _otherID;
            }
        }
    }
    class CreateMeeting : MeetingClass, MenuItem
    {
        //TODO clean up variables
        private int _accessLevel;
        private string _loginID;
        private string _location;
        private TimeOnly _time;
        private string _otherID;
        private string dateString;
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
            GetMeetingDetails(_accessLevel);
            if (_otherID != null)
            {
                if (CheckAvailability())
                {
                    using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
                    {
                        connection.Open();
                        var cmd = connection.CreateCommand();
                        string table = string.Empty;
                        string ID = string.Empty;
                        switch (_accessLevel)
                        {
                            case 1:
                                //Student creates a meeting
                                table = "student";
                                ID = "student";
                                cmd.Parameters.AddWithValue("@firstID", _loginID);
                                cmd.Parameters.AddWithValue("@secondID", _otherID);
                                break;
                            case 2:
                                //PS creates meeting
                                int selection = Functions.GetIntegerInRange(1, 3, "Please choose to add:\n1. Student meeting\n2. Senior tutor meeting\n3. Exit");
                                switch (selection)
                                {
                                    case 1:
                                        table = "student";
                                        ID = "student";
                                        break;
                                    case 2:
                                        table = "st";
                                        ID = "st";
                                        break;
                                    case 3:
                                    default:
                                        return;
                                }

                                cmd.Parameters.AddWithValue("@firstID", _otherID);
                                cmd.Parameters.AddWithValue("@secondID", _loginID);
                                break;
                            case 3:
                                //ST Creates meeting
                                table = "ST";
                                ID = "ST";
                                cmd.CommandText = "INSERT INTO stMeeting(STID, PSID, location, date, time) VALUES(@firstID, @secondID, @location, @date, @time);";
                                cmd.Parameters.AddWithValue("@firstID", _loginID);
                                cmd.Parameters.AddWithValue("@secondID", _otherID);
                                break;
                            default:
                                cmd.Parameters.AddWithValue("@firstID", "-1");
                                cmd.Parameters.AddWithValue("@secondID", "-1");
                                break;
                        }
                        cmd.CommandText = $"INSERT INTO {table}Meeting({ID}ID, PSID, location, date, time) VALUES(@firstID, @secondID, @location, @date, @time);";
                        cmd.Parameters.AddWithValue("@location", _location);
                        //cmd.Parameters.AddWithValue("@date", _date);
                        cmd.Parameters.AddWithValue("@date", dateString);
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
                }
                else
                {
                    Functions.OutputMessage($"The time slot {_time} on the date {dateString} is already taken");
                }
            }
            else { Functions.OutputMessage("Cannot find other party in database"); }
        }

        private bool CheckAvailability()
        {
            //TODO modify to check BOTH tables for PS before inserting new meeting
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                //MeetingClass CAC = new MeetingClass(ref dateString, ref _time);
                //string otherID = string.Empty;
                //string ID = string.Empty;

                //cmd.Parameters.AddWithValue("@firstID", _loginID);
                //cmd.Parameters.AddWithValue("@secondID", _otherID);
                //cmd.Parameters.AddWithValue("@date", dateString);
                //cmd.Parameters.AddWithValue("@location", _location);
                //cmd.Parameters.AddWithValue("@time", _time);
                switch (_accessLevel)
                {
                    case 1:
                        //Student books meeting
                        //checks PS availability in student meeting
                        if (!checkStudent(cmd, ref _loginID, ref dateString, ref _time))
                        {
                            //student is unavailabile
                            return false;
                        }
                        //checks ps availability in STMeeting
                        if (!checkPS(cmd, ref _otherID, ref dateString, ref _time))
                        {
                            //PS is available
                            return false;
                        }
                        break;
                    case 2:
                        //PS book meeting
                        //TODO check room availability outside of participents
                        //checks PS availability in stMeeting
                        if (!checkPS(cmd, ref _loginID, ref dateString, ref _time))
                        {
                            //PS is unavailable
                            return false;
                        }
                        //Checks other users availability in student/stmeeting
                        if (!checkST(cmd, ref _otherID, ref dateString, ref _time))
                        {
                            //ST is unavailable
                            if (!checkStudent(cmd, ref _otherID, ref dateString, ref _time))
                            {
                                //Student is unavailable
                                return false;
                            }
                        }
                        break;
                    case 3:
                        //ST book meeting
                        //Checks PS availability in stmeeting
                        if (!checkPS(cmd, ref _otherID, ref dateString, ref _time))
                        {
                            //PS is unavailable
                            return false;
                        }
                        //Checks own availability
                        if (!checkST(cmd, ref _loginID, ref dateString, ref _time))
                        {
                            //ST is unavailable
                            return false;
                        }
                        break;
                    default:
                        //Shoudln't happen
                        return false;
                }
                return true;
            }
        }
        private bool CheckRoom()
        {
            using (var connection = new SqliteConnection("DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM studentMeeting " +
                    "WHERE date = @date " +
                    "AND time = @time " +
                    "AND location != 'Teams' " +
                    "AND location = @loc;";
                cmd.Parameters.AddWithValue("@date", dateString);
                cmd.Parameters.AddWithValue("@time", _time);
                cmd.Parameters.AddWithValue("@loc", _location);
                if (cmd.ExecuteScalar() != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void GetMeetingDetails(int accLvl)
        {
            string fname = "";
            string lname = "";
            bool check = false;
            bool exit = false;

            if (accLvl != 1)
            {
                do
                {
                    check = false;
                    fname = Functions.GetString("Please input other parties first name, -1 to exit");

                    if (fname == "" || fname == " ")
                    {
                        Functions.OutputMessage("Invalid first name");
                        check = true;
                    }
                    else if (fname == "-1")
                    {
                        Functions.OutputMessage("Exiting");
                        exit = true;
                        break;
                    }
                    else
                    {
                        lname = Functions.GetString("Please input other parties last name");
                        if (lname == "" || lname == " ")
                        {
                            Functions.OutputMessage("Invalid last name");
                            check = true;
                        }
                    }

                    _otherID = getOtherID(ref fname, ref lname, ref accLvl, ref _loginID);
                    if (_otherID == "-1")
                    {
                        check = true;
                    }
                } while (check);
            }
            else
            {
                //Get students PSID
                using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT PSID FROM PSAllocation WHERE StudentID = @loginID;";
                    cmd.Parameters.AddWithValue("@loginID", _loginID);
                    if (cmd.ExecuteScalar() != null)
                    {
                        using (var sr = cmd.ExecuteReader())
                        {
                            sr.Read();
                            _otherID = sr.GetString(0);
                        }
                    }
                    else { Functions.OutputMessage("Error encountered, please try again"); }
                }
            }
            if (!exit)
            {
                do
                {
                    check = false;
                    _location = Functions.GetString("Please choose a location for the meeting (choose \"Teams\" for \"virtual\" meetings)");
                    if (_location == "" || _location == " ")
                    {
                        Functions.OutputMessage("Location cannot be left blank, please input value");
                        check = true;
                    }
                }
                while (check);

                DateTimeValidation.ValidateDateTime(ref _time, ref dateString);
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
            else
            {
                if (_accessLevel == 2)
                {
                    return "Select Student Meetings";
                }
                else
                {
                    return "Select Personal Supervisor Meetings";
                }
            }
        }
        public void Select()
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                switch (_accessLevel)
                {
                    case 1:
                        cmd.CommandText = "SELECT firstName, lastName, Title, location, date, time " +
                        "FROM studentMeeting, UserInfo ";
                        cmd.CommandText += "WHERE studentID = @loginID " +
                            "AND PSID = UserInfo.loginID;";
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
                        using (var sr = cmd.ExecuteReader())
                        {
                            Output(sr);
                        }
                        break;
                    case 2:
                        //Get PS loginIDs under the ST
                        //Get the meetings for the PSs with their IDs
                        cmd.CommandText = "SELECT firstName, lastName, Title, location, date, time " +
                        "FROM studentMeeting, UserInfo " +
                        "WHERE PSID = @loginID AND studentID = UserInfo.loginID ";
                        if (_select)
                        {
                            cmd.CommandText += "AND studentID = (SELECT loginID FROM UserInfo WHERE firstname = @fname AND lastname = @lname " +
                                "AND loginID IN (SELECT StudentID FROM PSAllocation WHERE PSID = @loginID));";
                            Functions.OutputMessage("Please input students firstname");
                            string fname = Functions.GetString();

                            Functions.OutputMessage("Please input students last name");
                            string lname = Functions.GetString();

                            cmd.Parameters.AddWithValue("@fname", fname);
                            cmd.Parameters.AddWithValue("@lname", lname);
                            cmd.Parameters.AddWithValue("@loginID", _loginID);
                            using (var sr = cmd.ExecuteReader())
                            {
                                Output(sr);
                            }
                        }
                        else
                        {
                            cmd.CommandText += "AND studentID IN (SELECT StudentID FROM PSAllocation WHERE PSID = @loginID);";
                            cmd.Parameters.AddWithValue("@loginID", _loginID);
                            using (var sr = cmd.ExecuteReader())
                            {
                                Output(sr);
                            }

                            cmd.CommandText = "SELECT FirstName, LastName, Title, Location, Date, Time " +
                                "FROM STMeeting, UserInfo " +
                                "WHERE PSID = @loginID AND STID = UserInfo.loginID " +
                                "AND STID = (SELECT STID FROM STAllocation WHERE PSID = @loginID);";

                            using (var sr = cmd.ExecuteReader())
                            {
                                Output(sr);
                            }
                        }
                        break;
                    case 3:
                        cmd.CommandText = "SELECT firstName, lastName, Title, location, date, time " +
                        "FROM stMeeting, UserInfo " +
                        "WHERE STID = @loginID AND PSID = UserInfo.loginID ";

                        if (_select)
                        {
                            cmd.CommandText += "AND PSID = " +
                                "(SELECT LoginID " +
                                "FROM UserInfo " +
                                "WHERE firstname = @fname " +
                                "AND lastname = @lname " +
                                "AND accessLevel = 2);";
                            string fname = Functions.GetString("Please input Personal Supervisors first name");
                            string lname = Functions.GetString("Please input Personal Supervisors last name");

                            cmd.Parameters.AddWithValue("@fname", fname);
                            cmd.Parameters.AddWithValue("@lname", lname);
                        }
                        else
                        {
                            cmd.CommandText += ";";
                        }
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
                        using (var sr = cmd.ExecuteReader())
                        {
                            Output(sr);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void Output(SqliteDataReader sr)
        {
            while (sr.Read())
            {
                Functions.OutputMessage($"Meeting with {sr.GetString(2)} {sr.GetString(0)} {sr.GetString(1)}:\n{sr.GetName(3)}: {sr.GetString(3)}\n" +
                $"{sr.GetName(4)}: {sr.GetString(4)}\n{sr.GetName(5)}: {sr.GetString(5)}");
            }
        }
    }
    class ManageMeetings : MeetingClass, MenuItem
    {
        private string _loginID;
        private int _accLvl;
        private TimeOnly _time;
        private string dateString;
        private string _otherID;
        public ManageMeetings(int accLvl, string loginID)
        {
            _loginID = loginID;
            _accLvl = accLvl;
        }
        public string MenuText()
        {
            return "Manage Meetings";
        }
        public void Select()
        {
            //PS Selects meeting - search by date and time
            //Prompts PS to choose between editing/changing meeting, deleting/cancelling meeting and exiting (return to home screen)
            //TODO test PSs can manage both student and ST meetings
            int selection = 0;
            string meetingID = GetMeeting(ref selection, ref _otherID);
            if (meetingID != "")
            {
                using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    string table = string.Empty;
                    string ID = string.Empty;
                    switch (_accLvl)
                    {
                        case 1:
                            table = "student";
                            ID = "student";
                            break;
                        case 2:
                            ID = "PS";
                            //int select = Functions.GetIntegerInRange(1, 2, "Please select either:\n1. Student meeting\n2. Senior Tutor meeting");
                            if (selection == 1)
                            {
                                table = "student";
                            }
                            else if (selection == 2)
                            {
                                table = "ST";
                            }
                            else
                            {
                                //Shouldn't happen
                                Functions.OutputMessage($"Error encountered, {selection} was not a valid input");
                            }
                            break;
                        case 3:
                            table = "ST";
                            ID = "ST";
                            break;
                        default:
                            //Shouldn't happen 
                            Functions.OutputMessage("Error encountered, please try again");
                            break;
                    }

                    selection = Functions.GetIntegerInRange(1, 3, "Please choose to:\n1: Cancel meeting\n2: Update meeting\n3: Return to home screen");
                    switch (selection)
                    {
                        case 1:
                            //Canel meeting - delete row from db
                            cmd.CommandText = $"DELETE FROM {table}Meeting WHERE meetingID = @meetingID AND {ID}ID = @loginID;";
                            cmd.Parameters.AddWithValue("@meetingID", meetingID);
                            cmd.Parameters.AddWithValue("@loginID", _loginID);

                            if (cmd.ExecuteScalar != null)
                            {
                                cmd.ExecuteNonQuery();
                            }
                            Functions.OutputMessage("Meeting cancelled");
                            return;
                        case 2:
                            //Update meeting
                            bool run = true;

                            do
                            {
                                cmd.CommandText = $"UPDATE {table}Meeting " +
                                    "SET location = @location, " +
                                    "time = @time, " +
                                    "date = @date " +
                                    "WHERE meetingID = @meetingID; ";
                                cmd.Parameters.AddWithValue("@location", Functions.GetString("Please select desired location (\"Teams\" for virtual meetings)"));

                                //TimeOnly time = new TimeOnly();
                                string dateString = string.Empty;
                                DateTimeValidation.ValidateDateTime(ref _time, ref dateString);
                                if (CheckAvailability(ref dateString, ref _accLvl, ref _otherID, ref meetingID) && dateString !="")
                                {
                                    cmd.Parameters.AddWithValue("@time", _time);
                                    cmd.Parameters.AddWithValue("@date", dateString);
                                    cmd.Parameters.AddWithValue("@meetingID", meetingID);
                                    //cmd.Parameters.AddWithValue("@loginID", _loginID);
                                    if (cmd.ExecuteNonQuery() > 0)
                                    {
                                        cmd.ExecuteNonQuery();
                                        Functions.OutputMessage("Meeting updated");
                                        run = false;
                                    }
                                    else
                                    {
                                        Functions.OutputMessage("Meeting was not able to be updated, please try again");
                                    }
                                }
                                else if (dateString != "") Functions.OutputMessage("Selected time slot is unavailable");
                            } while (run);

                            return;
                        case 3:
                        //User exits, no modification
                        default:
                            //Some other input is received, should never happen
                            return;
                    }
                }
            }
        }
        private string GetMeeting(ref int selection, ref string _otherID)
        {
            bool run = true;
            string meetingID = string.Empty;

            do
            {
                //string[] time_date = new string[2];
                selection = 0;
                if (_accLvl == 2)
                {
                    selection = Functions.GetIntegerInRange(1, 3, "Please choose:\n1. Select Student meeting\n2. Select Senior Tutor meeting \n3. Exit");
                    if (selection != 3) DateTimeValidation.ValidateDateTime(ref _time, ref dateString);
                }
                else
                {
                    //time_date = SelectMeetingDetails().Split(" ");
                    DateTimeValidation.ValidateDateTime(ref _time, ref dateString);
                }

                if (dateString != "")
                {
                    using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
                    {
                        connection.Open();
                        var cmd = connection.CreateCommand();

                        switch (_accLvl)
                        {
                            case 1:
                                //Student manage meetings
                                //Can only manage PS meetings
                                cmd.CommandText = "SELECT Title, Firstname, Lastname, Location, Date, Time, MeetingID, PSID " +
                                    "FROM studentMeeting, UserInfo " +
                                    "WHERE date=@date " +
                                    "AND time = @time " +
                                    "AND PSID = UserInfo.loginID " +
                                    "AND studentID = @loginID;";
                                break;
                            case 2:
                                //PS manage meetings
                                //can manage both student and ST meetings

                                switch (selection)
                                {
                                    case 1:
                                        //PS manages student meeting
                                        cmd.CommandText = "SELECT title, firstname, lastname, location, date, time, meetingID, studentID " +
                                        "FROM studentMeeting, UserInfo " +
                                        "WHERE date=@date " +
                                        "AND time = @time " +
                                        "AND PSID = @loginID " +
                                        "AND UserInfo.loginID = studentID;";
                                        break;
                                    case 2:
                                        //PS manages ST meeting
                                        cmd.CommandText = "Select Title, Firstname, Lastname, Location, Date, Time, MeetingID, stID " +
                                            "FROM STMeeting, UserInfo " +
                                            "WHERE date=@date " +
                                            "AND time = @time " +
                                            "AND PSID = @loginID " +
                                            "AND UserInfo.loginID = STID;";
                                        break;
                                    default:
                                        //Should only happen when user exits
                                        //No output needed 
                                        return "";
                                }

                                break;
                            case 3:
                                //ST manage meetings
                                //can manage PS meetings
                                cmd.CommandText = "SELECT Title, Firstname, Lastname, Location, Date, Time, MeetingID, PSID " +
                                    "FROM STmeeting, UserInfo " +
                                    "WHERE date=@date " +
                                    "AND time = @time " +
                                    "AND PSID = UserInfo.loginID " +
                                    "AND STID = @loginID;";
                                break;
                            default:
                                //Should never happen
                                Functions.OutputMessage("Error encountered, please try again");
                                break;
                        }

                        cmd.Parameters.AddWithValue("@time", _time + ":00");
                        cmd.Parameters.AddWithValue("@date", dateString);
                        cmd.Parameters.AddWithValue("@loginID", _loginID);
                        if (cmd.ExecuteScalar() != null)
                        {
                            using (var sr = cmd.ExecuteReader())
                            {
                                while (sr.Read())
                                {
                                    Functions.OutputMessage("You have selected the meeting with: " +
                                        $"{sr.GetString(0)} {sr.GetString(1)} {sr.GetString(2)}\nlocation: {sr.GetString(3)}\nOn: {sr.GetString(4)}\nAt: {sr.GetString(5)}");
                                    meetingID = sr.GetString(6);
                                    _otherID = sr.GetString(7);
                                }
                                run = false;
                            }
                        }
                        else { Functions.OutputMessage("Cannot find meeting with the details provided"); }
                    }
                }
                else { run = false; }
            } while (run);
            return meetingID;
        }
        private bool CheckAvailability(ref string dateString, ref int _accessLevel, ref string _otherID, ref string meetingID)
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                switch (_accessLevel)
                {
                    case 1:
                        //Student books meeting
                        //checks PS availability in student meeting
                        if (!checkStudent(cmd, ref _loginID, ref dateString, ref _time, meetingID))
                        {
                            //student is unavailabile
                            return false;
                        }
                        //checks ps availability in STMeeting
                        if (!checkPS(cmd, ref _otherID, ref dateString, ref _time, meetingID))
                        {
                            //PS is available
                            return false;
                        }
                        break;
                    case 2:
                        //PS book meeting
                        //checks PS availability in stMeeting
                        if (!checkPS(cmd, ref _loginID, ref dateString, ref _time, meetingID))
                        {
                            //PS is unavailable
                            return false;
                        }
                        //Checks other users availability in student/stmeeting
                        if (!checkST(cmd, ref _otherID, ref dateString, ref _time, meetingID))
                        {
                            //ST is unavailable
                            if (!checkStudent(cmd, ref _otherID, ref dateString, ref _time, meetingID))
                            {
                                //Student is unavailable
                                return false;
                            }
                        }
                        break;
                    case 3:
                        //ST book meeting
                        //Checks PS availability in stmeeting
                        if (!checkPS(cmd, ref _otherID, ref dateString, ref _time, meetingID))
                        {
                            //PS is unavailable
                            return false;
                        }
                        //Checks own availability
                        if (!checkST(cmd, ref _loginID, ref dateString, ref _time, meetingID))
                        {
                            //ST is unavailable
                            return false;
                        }
                        break;
                    default:
                        //Shoudln't happen
                        return false;
                }
                return true;
            }
        }
    }
}

