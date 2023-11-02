using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coursework
{
    internal class FileHandling
    {
    }
    class FileHandler : ConsoleMenu
    {
        public override void CreateMenu()
        {
            _menuItems.Clear();
            _menuItems.Add(new Login());
            _menuItems.Add(new ExitMenuItem(this));
        }
    }
    class LoginInfo
    {
        string _username { get; set; }
        string _password { get; set; }
    }
    public class Login : MenuItem
    {
        public string _loginID { get; private set; }
        internal string _password { get; private set; }
        public string MenuText()
        {
            return "Login";
        }
        public void Select()
        {



            //string fileName = "LoginInfo.json";
            //Functions.OutputMessage("Please enter LoginID");
            //string _loginID = Functions.GetString();
            //List<LoginInfo> lines;
            //using (StreamReader streamReader = new StreamReader(fileName))
            //{
            //    string json = streamReader.ReadToEnd();
            //    lines = JsonConvert.DeserializeObject<List<LoginInfo>>(json);
            //    Console.WriteLine(lines);
            //}
            //if (lines[0].Contains(_loginID))
            //{
            //    int index = lines.IndexOf(_loginID);
            //    string passwordInput = Functions.GetString();
            //    _password = lines[index].ToString();
            //    if (_password == passwordInput)
            //    {
            //        Console.WriteLine("Im in");
            //    }
            //}
            //else
            //{
            //    Functions.OutputMessage($"{_loginID} is not found");
            //}
        }
    }
}
