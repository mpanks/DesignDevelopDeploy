//using Newtonsoft.Json;
using Microsoft.Data.Sqlite;
using System.Net.WebSockets;
using System.Text.Json;

namespace Coursework
{
    internal class Program
    {
        static void Main(string[] args)
        {

            using (var sqlConnection = new SqliteConnection("Data Source = DDD_CW.db"))
            {
                sqlConnection.Open();
                var cmd = sqlConnection.CreateCommand();

                //cmd.CommandText = "Create Table studentMeeting(studentID TEXT NOT NULL, " +
                //    "meetingID INTEGER PRIMARY KEY AUTOINCREMENT, " +
                //    "PSID TEXT NOT NULL, " +
                //    "location TEXT NOT NULL, " +
                //    "time TEXT NOT NULL, " +
                //    "FOREIGN KEY(studentID) " +
                //    " REFERENCES UserLogin(LoginID) " +
                //    " ON UPDATE CASCADE " +
                //    " ON DELETE CASCADE, " +
                //    "FOREIGN KEY(PSID) " +
                //    " REFERENCES UserLogin(LoginID) " +
                //    " ON UPDATE CASCADE " +
                //    " ON DELETE CASCADE);";
                //Hashing hash = new Hashing();
                //string salt = hash.GenerateSalt();
                //string password = hash.GenerateHash("password", salt);
                cmd.CommandText = //"ALTER TABLE ProgressReports Rename COLUMN LastName TO Report;" +
                                  //"ALTER TABLE studentMeeting ADD COLUMN confirmed INTEGER; ";
                                  //$"INSERT INTO studentMeeting(studentID,PSID,location,time) Values ('717402','1','Teams','10:30'); ";
                                  "UPDATE studentMeeting SET confirmed = 0 WHERE studentID='717402';";
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
                Console.WriteLine("Created table");
            }

            FileHandler FH = new FileHandler();
            FH.Select();
        }
        public class person
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            //int ID;
            //public person(string Firstname, string Lastname)
            //{
            //    firstname = Firstname;
            //    lastname = Lastname;
            //    //this.ID = ID;
            //}
        }
    }
}