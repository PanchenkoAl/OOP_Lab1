using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;

namespace OOP_Lab_01
{
    class Lexer
    {
        const int Alphabet = 26;
        enum Types { NONE, DELIMITER, VARIABLE, NUMBER };
        public enum Errors { NOERR, SYNTAX, UNBALPARENTS, NOEXP, DIVBYZERO };
        public Errors tokenErrors;
        private string expression = "123";
        private int expIndex;
        private string token = "1";

        private Types tokenType;
        private double[] variables = new double[Alphabet];

        public Lexer()
        {
            for (int i = 0; i < variables.Length; i++)
                variables[i] = 0.0;
        }

        public string GetExpression()
        {
            return expression;
        }

        public void SetExpression(string m_expression)
        {
            expression = m_expression;
        }

        bool IsDelim(char c)
        {
            if ("+-/*%^=()|&".IndexOf(c) != -1)
                return true;
            else
                return false;
        }

        bool IsNumber(char c)
        {
            if ("1234567890".IndexOf(c) != -1)
                return true;
            else
                return false;
        }
        bool IsLetter(char c)
        {
            if ("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz".IndexOf(c) != -1)
                return true;
            else
                return false;
        }


        void GetToken()
        {
            tokenType = Types.NONE;
            token = "";

            
            if (expIndex == expression.Length)
                return;
            while (expIndex <= expression.Length && char.IsWhiteSpace(expression[expIndex]))
                expIndex++;
            
            if (expIndex == expression.Length)
                return;

            if (IsDelim(expression[expIndex]))
            {
                token += expression[expIndex];
                expIndex++;
                tokenType = Types.DELIMITER;
            }
            else if (IsLetter(expression[expIndex]))
            {
                while (expIndex < expression.Length && !IsDelim(expression[expIndex]))
                {
                    token += expression[expIndex];
                    expIndex++;
                    if (expIndex >= expression.Length) break;
                }
                tokenType = Types.VARIABLE;
            }
            else if (IsNumber(expression[expIndex]))
            {
                    
                while (expIndex < expression.Length && !IsDelim(expression[expIndex]))
                {
                    token += expression[expIndex];
                    expIndex++;
                    if (expIndex >= expression.Length) break;
                }
                tokenType = Types.NUMBER;
            }
        }

        void Atom(out double result)
        {
            switch (tokenType)
            {
                case Types.NUMBER:
                    try
                    {
                        result = Double.Parse(token);
                    }
                    catch (FormatException)
                    {
                        result = 0.0;
                        tokenErrors = Errors.SYNTAX;
                    }
                    GetToken();
 
                    return;
                case Types.VARIABLE:
                    result = FindVariable(token);
                    GetToken();
                    return;
                default:
                    result = 0.0;
                    tokenErrors = Errors.SYNTAX;
                    break;
            }
        }


        double FindVariable(string vname)
        {
            if (vname.Contains("max(") || vname.Contains("min("))
            {
                string temp1 = null;
                double x, y;
                for (int i = 4; i < vname.Length; i++)
                {
                    temp1 += vname[i];
                }
                string[] vnameSplit = temp1.Split(',');
                x = Convert.ToDouble(vnameSplit[0]);
                y = Convert.ToDouble(vnameSplit[1]);
                if (x < y)
                {
                    if (vname.Contains("min("))
                        return x;
                    else
                        return y;
                }
                else
                    if (vname.Contains("min("))
                    return y;
                else
                    return x;
            }
            return 0.0;
        }

        void EvaluateExpression1(out double result)
        {
            int varIdx;
            Types ttokenType;
            string tempToken;
            if (tokenType == Types.VARIABLE)
            {
                tempToken = String.Copy(token);
                ttokenType = tokenType;
                varIdx = Char.ToUpper(token[0]) - 'A';
                GetToken();
                if (token != "=")
                {
                    expIndex -= token.Length;
                    token = String.Copy(tempToken);
                    tokenType = ttokenType;
                }
                else
                {
                    GetToken();
                    EvaluateExpression2(out result);
                    variables[varIdx] = result;
                    return;
                }
            }
            EvaluateExpression2(out result);   
        }

        void EvaluateExpression2(out double result)
        {
            string op;
            double partialResult;

            EvaluateExpression3(out result);
            while ((op = token) == "+" || op == "-")
            {
                GetToken();
                EvaluateExpression3(out partialResult);
                switch(op)
                {
                    case "+":
                        result += partialResult;
                        break;
                    case "-":
                        result -= partialResult;
                        break;
                }
            }
        }

        void EvaluateExpression3(out double result)
        {
            string op = token;
            double partialResult = 0.0;

            EvaluateExpression4(out result);
            while((op = token) == "*" || op == "/" )
            {
                GetToken();
                EvaluateExpression4(out partialResult);
                switch (op)
                {
                    case "*":
                        result *= partialResult;
                        break;
                    case "/":
                        if(partialResult == 0.0)
                        {
                            result = 0.0;
                        }
                        result /= partialResult;
                        break;
                }
            }
        }

        void EvaluateExpression4(out double result)
        {
            double partialResult, ex;
            int t;
            EvaluateExpression5(out result);
            if(token == "^")
            {
                GetToken();
                EvaluateExpression4(out partialResult);
                ex = result;
                if(partialResult == 0.0)
                {
                    result = 1.0;
                    return;
                }
                for(t = (int)partialResult - 1; t > 0; t --)
                {
                    result = result * (double)ex;
                }
            }
        }

        void EvaluateExpression5(out double result)
        {
            string op;
            
            op = "";
            if(tokenType == Types.DELIMITER && (token == "+" || token == "-"))
            {
                op = token;
                GetToken();
            }
            EvaluateExpression6(out result);
            if (op == "-")
                result = -result;
        }

        void EvaluateExpression6(out double result)
        {
            if (token == "(")
            {
                GetToken();
                EvaluateExpression2(out result);
                if (token != ")")
                {
                    tokenErrors = Errors.UNBALPARENTS;
                }
                GetToken();
            }
            else
                Atom(out result);
        }

        public Report Evaluate(string exprstr)
        {
            double result;
            expression = exprstr;
            expIndex = 0;
            tokenErrors = Errors.NOERR;

            GetToken();
            if (token == "")
            {
                tokenErrors = Errors.NOEXP;
                return new Report(0.0, tokenErrors);
            }
            EvaluateExpression1(out result);
            return new Report(result, tokenErrors);
        }
    }
}
