using Coursework;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;


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

    class CreateMeeting : MenuItem
    {
        //TODO clean up variables
        private int _accessLevel;
        private string _loginID;
        private string _location;
        private TimeOnly _time;
        private string _otherID;
        //private DateOnly _date;
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
                        cmd.CommandText = "INSERT INTO studentMeeting(studentID, PSID, location, date, time) VALUES(@firstID, @secondID, @location, @date, @time);";
                        switch (_accessLevel)
                        {
                            case 1:
                                //Student creates a meeting
                                cmd.Parameters.AddWithValue("@firstID", _loginID);
                                cmd.Parameters.AddWithValue("@secondID", _otherID);
                                break;
                            case 2:
                                //PS creates meeting
                                cmd.Parameters.AddWithValue("@firstID", _otherID);
                                cmd.Parameters.AddWithValue("@secondID", _loginID);
                                break;
                            case 3:
                                //ST Creates meeting
                                cmd.CommandText = "INSERT INTO stMeeting(STID, PSID, location, date, time) VALUES(@firstID, @secondID, @location, @date, @time);";
                                cmd.Parameters.AddWithValue("@firstID", _loginID);
                                cmd.Parameters.AddWithValue("@secondID", _otherID);
                                break;
                            default:
                                cmd.Parameters.AddWithValue("@firstID", "-1");
                                cmd.Parameters.AddWithValue("@secondID", "-1");
                                break;
                        }
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
        }
        private bool CheckAvailability()
        {
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM studentMeeting " +
                    "WHERE studentID=@studentID " +
                    "AND PSID = @PSID " +
                    "AND location=@location " +
                    "AND date = @date " +
                    "AND time=time ";
                switch (_accessLevel)
                {
                    case 1:
                        cmd.Parameters.AddWithValue("@studentID", _loginID);
                        cmd.Parameters.AddWithValue("@PSID", _otherID);
                        break;
                    case 2:
                        cmd.Parameters.AddWithValue("@studentID", _otherID);
                        cmd.Parameters.AddWithValue("@PSID", _loginID);
                        break;
                    default:
                        cmd.Parameters.AddWithValue("@studentID", "-1");
                        cmd.Parameters.AddWithValue("@PSID", "-1");
                        break;
                }
                cmd.Parameters.AddWithValue("@date", dateString);
                cmd.Parameters.AddWithValue("@location", _location);
                cmd.Parameters.AddWithValue("@time", _time);
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
                                cmd.CommandText = "SELECT loginID FROM UserInfo WHERE firstname = @fname AND lastname = @lname AND accessLevel = @accLvl " +
                                                  "AND loginID IN (SELECT StudentID FROM PSAllocation WHERE PSID = @loginID);";

                                cmd.Parameters.AddWithValue("@fname", fname);
                                cmd.Parameters.AddWithValue("@lname", lname);
                                cmd.Parameters.AddWithValue("@accLvl", 1);
                                break;
                            case 3:
                                cmd.CommandText = "SELECT loginID FROM UserInfo WHERE firstname = @fname AND lastname = @lname AND accessLevel = @accLvl " +
                                    "AND loginID IN (SELECT PSID FROM STAllocation WHERE STID = @loginID);";
                                cmd.Parameters.AddWithValue("@fname", fname);
                                cmd.Parameters.AddWithValue("@lname", lname);
                                cmd.Parameters.AddWithValue("@accLvl", 2);
                                break;
                            default:
                                //Should never happen
                                Console.WriteLine("Error, Please try again");
                                break;
                        }
                        cmd.Parameters.AddWithValue("@loginID", _loginID);

                        using (var sr = cmd.ExecuteReader())
                        {
                            while (sr.Read())
                            {
                                _otherID = sr.GetString(0);
                            }
                        }
                        if (_otherID == "")
                        {
                            check = true;
                            Functions.OutputMessage("Unrecognised user, please check the spelling of the name and try again");
                        }
                        connection.Close();
                    }

                } while (check);
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
                if (_accessLevel == 1)
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
                            cmd.CommandText += "AND studentID = (SELECT loginID FROM UserIndo WHERE firstname = @fname AND lastname = @lname " +
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
                //cmd.Parameters.AddWithValue("@loginID", _loginID);
                //using (var sr = cmd.ExecuteReader())
                //{
                //    while (sr.Read())
                //    {
                //        Functions.OutputMessage($"Meeting with {sr.GetString(2)} {sr.GetString(0)} {sr.GetString(1)}:\n{sr.GetName(3)}: {sr.GetString(3)}\n" +
                //            $"{sr.GetName(4)}: {sr.GetString(4)}\n{sr.GetName(5)}: {sr.GetString(5)}");
                //    }
                //}
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
    class ManageMeetings : MenuItem
    {
        private string _loginID;
        private int _accLvl;
        private TimeOnly _time;
        private DateOnly _date;
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
            string meetingID = GetMeeting();
            using (var connection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                int selection = Functions.GetIntegerInRange(1, 3, "Please choose to:\n1: Cancel meeting\n2: Update meeting\n3: Return to home screen");
                switch (selection)
                {
                    case 1:
                        //Canel meeting - delete row from db
                        cmd.CommandText = "DELETE FROM studentMeeting WHERE meetingID = @meetingID AND PSID = @loginID;";
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
                            cmd.CommandText = "UPDATE studentMeeting " +
                                "SET location = @location, " +
                                "time = @time, " +
                                "date = @date " +
                                "WHERE meetingID = @meetingID " +
                                "AND PSID = @loginID;";
                            cmd.Parameters.AddWithValue("@location", Functions.GetString("Please select desired location (\"Teams\" for virtual meetings)"));
                            cmd.Parameters.AddWithValue("@time", Functions.GetString("Please choose a time for the meeting (HH:MM)"));
                            cmd.Parameters.AddWithValue("@date", Functions.GetString("Please select a date for the meeting (DD-MM-YY)"));
                            cmd.Parameters.AddWithValue("@meetingID", meetingID);
                            cmd.Parameters.AddWithValue("@loginID", _loginID);
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
                        } while (run);
                        return;
                    case 3:
                        //User exits, no modification
                        return;
                    default:
                        //Some other input is received, should never happen
                        return;
                }
            }
        }
        private string GetMeeting()
        {
            bool run = true;
            string meetingID = string.Empty;
            //TODO modify to allow students and STs to manage their meetings
            //Add switch case
            //Cry

            do
            {
                string[] time_date = SelectMeetingDetails().Split(" ");
                if (time_date[1] != "01/01/0001")
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
                                cmd.CommandText = "SELECT Title, Firstname, Lastname, Location, Date, Time, MeetingID " +
                                    "FROM studentMeeting, UserInfo " +
                                    "WHERE date=@date " +
                                    "AND time = @time " +
                                    "AND PSID = UserInfo.loginID " +
                                    "AND studentID = @loginID;";
                                break;
                            case 2:
                                //PS manage meetings
                                //can manage both student and ST meetings
                                int selection = Functions.GetIntegerInRange(1, 3, "Please choose:\n1. Select Student meeting\n2. Select Senior Tutor meeting \n3. Exit");
                                switch(selection)
                                {
                                    case 1:
                                        cmd.CommandText = "SELECT title, firstname, lastname, location, date, time, meetingID " +
                                        "FROM studentMeeting, UserInfo " +
                                        "WHERE date=@date " +
                                        "AND time = @time " +
                                        "AND PSID = @loginID " +
                                        "AND UserInfo.loginID = studentID;";
                                        break;
                                        case 2:
                                        cmd.CommandText = "Select Title, Firstname, Lastname, Location, Date, Time, MeetingID " +
                                            "FROM STMeeting, UserInfo " +
                                            "WHERE date=@date " +
                                            "AND time = @time " +
                                            "AND PSID = @loginID " +
                                            "AND UserInfo.loginID = STID;";
                                        break;
                                    default:
                                        //Should only happen when user exits
                                        break;
                                }

                                break;
                            case 3:
                                //ST manage meetings
                                //can manage PS meetings
                                cmd.CommandText = "SELECT Title, Firstname, Lastname, Location, Date, Time, MeetingID " +
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

                        cmd.Parameters.AddWithValue("@time", time_date[0] +":00");
                        cmd.Parameters.AddWithValue("@date", time_date[1]);
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
                                }
                                run = false;
                            }
                        }
                        else { Functions.OutputMessage("Cannot find meeting with the details provided"); }
                    }
                }
            } while (run);
            return meetingID;
        }
        private string SelectMeetingDetails()
        {
            string date = string.Empty;
            DateTimeValidation.ValidateDateTime(ref _time, ref date);
            //string date = _date.ToString("yyyy-mm-dd h:mm");
            string output = $"{_time} {date}";
            return output;
        }
    }
}

