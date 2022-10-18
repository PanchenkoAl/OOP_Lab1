using System;
using System.Collections.Generic;
using System.Text;

namespace OOP_Lab_01
{
    public class Cell
    {
        public string Value;
        public string value_s;
        public List<string> dependents = new List<string>();
        public Cell()
        {
            value_s = "0";
        }

        public string getName(int c, int r)
        {
            string temp = null;
            temp += (char)(c + 65);
            temp += (char)(r + 48);
            return temp;
        }
    }
}
