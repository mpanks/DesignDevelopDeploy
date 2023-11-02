using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class menu
    {
    }
    public static class Functions
    {

        public static void OutputMessage(string pMessage)
        {
            Console.WriteLine(pMessage);
        }
        public static int GetIntegerInRange(float pMin, float pMax, string pMessage)
        {
            int result = -1;
            try
            {
                float temp = GetFloatInRange(pMin, pMax, pMessage);
                result = Convert.ToInt16(temp);
            }
            catch { OutputMessage($"input is not an integer value, please try again"); }
            return result;
        }
        public static float GetFloatInRange(float pMin, float pMax, string pMessage)
        {
            if (pMin > pMax)
            {
                throw new Exception($"Minimum value of {pMin} cannot be greater than the maximum value {pMax}");
            }

            float result = -1;
            do
            {
                OutputMessage(pMessage);
                OutputMessage($"Please enter a number between {pMin} and {pMax} inclusive");

                string input = Console.ReadLine();
                try
                {
                    result = float.Parse(input);
                }
                catch
                {
                    OutputMessage($"{input} is not a number, please try again");
                }

                if (result >= pMin && result <= pMax)
                {
                    return result;
                }
                OutputMessage($"{result} is not between {pMin} and {pMax}");
            }
            while (true);
        }
        public static string GetString()
        {
            return Console.ReadLine();
        }

    }
    interface MenuItem
    {
        public abstract string MenuText();
        public abstract void Select();
    }

    abstract class ConsoleMenu : MenuItem
    {
        protected List<MenuItem> _menuItems = new List<MenuItem>();
        public bool IsActive { get; set; }
        public abstract void CreateMenu();
        public virtual void Select()
        {
            IsActive = true;
            do
            {
                CreateMenu();
                if (_menuItems.Count > 0)
                {
                    string output = $"{MenuText()}{Environment.NewLine}";
                    int selection = Functions.GetIntegerInRange(1, _menuItems.Count, this.ToString()) - 1;
                    _menuItems[selection].Select();
                }
            } while (IsActive);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(MenuText());
            for (int i = 0; i < _menuItems.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {_menuItems[i].MenuText()}");
            }
            return sb.ToString();
        }
        public virtual string MenuText()
        {
            return "Console Menu";
        }
    }

    class ExitMenuItem : MenuItem
    {
        protected ConsoleMenu _menu;
        public ExitMenuItem(ConsoleMenu parentItem)
        {
            _menu = parentItem;
        }
        public virtual string MenuText()
        {
            return "Exit";
        }
        public virtual void Select()
        {
            _menu.IsActive = false;
        }
    }
}
