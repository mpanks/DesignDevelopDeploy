//using Newtonsoft.Json;
using System.Text.Json;

namespace Coursework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"C:\\Users\\matth\\OneDrive\\Documents\\Comp Sci\\DDD\\Coursework\\Coursework\\Test_File.json";
            List<person> people = new List<person>();
            person miranda = new person{
                firstname="miranda",
                lastname="evergreen"};
            people.Add(miranda);
            //JsonSerializer serializer = new JsonSerializer();
            using(StreamWriter sw = new StreamWriter(fileName))
            {
                //string json = JsonConvert.SerializeObject(people,Formatting.Indented);
                string json = JsonSerializer.Serialize(people);
                sw.WriteLine(json);
                Console.WriteLine(json);
                //sw.WriteLine(jsonString);
                //Console.WriteLine(jsonString);
            }
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