using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OOP_Lab_01
{
    class Report
    {
        public double Value;
        public Lexer.Errors Code;
        public Report()
        {
            Value = 0.0;
            Code = Lexer.Errors.NOERR;
        }
        public Report(double m_Value, Lexer.Errors m_Code)
        {
            Value = m_Value;
            Code = m_Code;
        }
        public bool Exception()
        {
            return (Code == Lexer.Errors.NOERR);
        }
        public string GetValue()
        {
            switch (Code)
            {
                case Lexer.Errors.NOERR:
                    return Value.ToString();
                case Lexer.Errors.DIVBYZERO:
                    { /*MassageBox.Show("Dividing by zero is impossible, result set by 0.0");*/ return ""; }
                case Lexer.Errors.NOEXP:
                    return "#ERRORNE";
                case Lexer.Errors.UNBALPARENTS:
                    return "#ERRORUP";
                case Lexer.Errors.SYNTAX:
                    return "#ERRORSY";
            }
            return "";
        }
    }
}
