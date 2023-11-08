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
            //string fileName = "Test_File.json";
            //List<person> people = new List<person>();
            //person miranda = new person{
            //    firstname="miranda",
            //    lastname="evergreen"};
            //people.Add(miranda);
            ////JsonSerializer serializer = new JsonSerializer();
            //using(StreamWriter sw = new StreamWriter(fileName))
            //{
            //    //string json = JsonConvert.SerializeObject(people,Formatting.Indented);
            //    string json = JsonSerializer.Serialize(people);
            //    sw.WriteLine(json);
            //    Console.WriteLine(json);
            //    //sw.WriteLine(jsonString);
            //    //Console.WriteLine(jsonString);
            //}

            //using (var sqlConnection = new SqliteConnection("Data Source = DDD_CW.db"))
            //{
            //    sqlConnection.Open();
            //    var cmd = sqlConnection.CreateCommand();

            //    //cmd.CommandText = "Create Table ProgressReports(LoginID TEXT NOT NULL, " +
            //    //    "ReportID INTEGER PRIMARY KEY AUTOINCREMENT, " +
            //    //    "LastName TEXT, " +
            //    //    "" +
            //    //    "FOREIGN KEY(LoginID)" +
            //    //    " REFERENCES UserLogin(LoginID)" +
            //    //    " ON UPDATE CASCADE" +
            //    //    " ON DELETE CASCADE);";

            //    //cmd.CommandText = //"ALTER TABLE ProgressReports Rename COLUMN LastName TO Report;" +
            //    //                  //"ALTER TABLE ProgressReports ADD COLUMN ConfidenceLevel INTEGER; " +
            //    //                  //"INSERT INTO ProgressReports(ConfidenceLevel) VALUES(5), WHERE (LoginID = '717402');";
            //    //    //"UPDATE ProgressReports SET ConfidenceLevel = 5 WHERE LoginID='717402';";
            //    //cmd.ExecuteNonQuery();
            //    //sqlConnection.Close();
            //    //Console.WriteLine("Created table");
            //}

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