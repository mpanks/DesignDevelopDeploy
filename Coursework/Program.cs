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

            //using (var sqlConnection = new SqliteConnection("Data Source = DDD_CW.db"))
            //{
            //    sqlConnection.Open();
            //    var cmd = sqlConnection.CreateCommand();

            //    cmd.CommandText = "Create Table ST(PSID TEXT NOT NULL, " +
            //        "meetingID INTEGER PRIMARY KEY AUTOINCREMENT, " +
            //        "STID TEXT NOT NULL, " +
            //        "location TEXT NOT NULL, " +
            //        "time TEXT NOT NULL, " +
            //        "date TEXT NOT NULL, " +
            //        "PSConfirmed INT, " +
            //        "STConfirmed, " +
            //        "FOREIGN KEY(STID) " +
            //        " REFERENCES UserLogin(LoginID) " +
            //        " ON UPDATE CASCADE " +
            //        " ON DELETE CASCADE, " +
            //        "FOREIGN KEY(PSID) " +
            //        " REFERENCES UserLogin(LoginID) " +
            //        " ON UPDATE CASCADE " +
            //        " ON DELETE CASCADE);";
            //    //Hashing hash = new Hashing();
            //    //string salt = hash.GenerateSalt();
            //    //string password = hash.GenerateHash("password", salt);
            //    //cmd.CommandText = //$"INSERT INTO PSMeeting(PSID,STID,location,time,date,PSConfirmed,STConfirmed) Values('1','3', 'RBB-209','13:45','13-12-24',0,0);";
            //    //"ALTER TABLE studentMeeting ADD COLUMN date TEXT; " +
            //    //                  //$"INSERT INTO UserInfo(loginID, title, firstname, lastname, accesslevel) VALUES ('3','Mrs','Julia', 'Loughborough', 3); ";
            //    //"UPDATE studentMeeting SET studentConfirmed = 0, PSconfirmed = 0, date = '12-12-24' WHERE PSID='1';";
            //    cmd.ExecuteNonQuery();
            //    sqlConnection.Close();
            //    Console.WriteLine("Created table");
            //}

            FileHandler FH = new FileHandler();
            FH.Select();
        }
    }
}