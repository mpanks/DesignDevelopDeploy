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
                cmd.CommandText = //"ALTER TABLE studentMeeting Rename COLUMN confirmed TO studentConfirmed;";
                                  //"ALTER TABLE studentMeeting ADD COLUMN date TEXT; " +
                //$"INSERT INTO studentMeeting(studentID,PSID,location,time) Values ('717402','1','Teams','10:30'); ";
                "UPDATE studentMeeting SET studentConfirmed = 0, PSconfirmed = 0, date = '12-12-24' WHERE studentID='717402';";
                cmd.ExecuteNonQuery();
                sqlConnection.Close();
                Console.WriteLine("Created table");
            }

            FileHandler FH = new FileHandler();
            FH.Select();
        }
    }
}