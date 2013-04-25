using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class RegularExpression
    {
        public readonly static char EMPTY_WORD = '$';
        public readonly static string EMPTY_SET = "EMPTY_SET";

        public string Value { get; private set; }

        public RegularExpression(string str)
        {
            removeObsoleteParentheses(ref str);
            Value = str;
        }



        public RegularExpression() : this(EMPTY_WORD) { }

        public RegularExpression(char c) : this(c.ToString()) { }

        public bool IsEmptyWord
        {
            get
            {
                if (Value.Equals(EMPTY_WORD))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public bool IsEmptySet
        {
            get
            {
                if (Value.Equals(EMPTY_SET))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public bool IsUnion
        {
            get
            {
                if (Value.Length == 1) return false;
                int parentheses = 0;
                for (int i = 0; i < Value.Length; i++)
                {
                    switch (Value[i])
                    {
                        case '(':
                            parentheses++;
                            break;
                        case ')':
                            parentheses--;
                            break;
                        case '+':
                            if (parentheses == 0 && Value[i - 1] != '^')
                                return true;
                            break;
                    }
                }
                return false;
            }
        }

        public bool IsConcatenation
        {
            get
            {
                if (IsUnion) return false;
                if (Value.Length == 3)
                {
                    if (Value[1] == '^') return false;
                    else return true;
                }
                else if (Value.Length < 2)
                    return false;

                int parentheses = 0;
                int numberOfParentheses = Value.Count(c => c == '(' || c == ')');
                for (int i = 0; i < Value.Length; i++)
                {
                    switch (Value[i])
                    {
                        case '(':
                            parentheses++;
                            break;
                        case ')':
                            parentheses--;
                            break;
                        case '*':
                            if (parentheses == 0 && i == Value.Length - 1 && Value[i-1] == ')' && numberOfParentheses == 2)
                                return false;
                            break;
                        case '^':
                            if (parentheses == 0 && Value[i + 1] == '+' && i == Value.Length - 2 && Value[i - 1] == ')' && numberOfParentheses == 2)
                                return false;
                            break;
                    }
                }
                return true;
            }
        }

        public bool IsKleene
        {
            get
            {
                if (Value.Length < 2 || IsUnion) return false;
                if (Value.Length == 2 && Value[1] == '*') return true;
                int parenthesesCounter = 0;
                int numberOfParentheses = Value.Count(c => c == '(' || c == ')');
                for (int i = 0; i < Value.Length; i++)
                {
                    switch (Value[i])
                    {
                        case '(':
                            parenthesesCounter++;
                            break;
                        case ')':
                            parenthesesCounter--;
                            break;
                        case '*':
                            if (parenthesesCounter == 0 && i == Value.Length - 1 && numberOfParentheses == 2 )
                                return true;
                            break;
                    }
                }
                return false;
            }
        }

        public bool IsPlus
        {
            get
            {
                if (Value.Length < 3 || IsUnion || IsConcatenation) return false;
                if (Value.Length == 3 && Value[1] == '^' && Value[2] == '+') return true;
                int parenthesesCounter = 0;
                int numberOfParentheses = Value.Count(c => c == '(' || c == ')');
                for (int i = 0; i < Value.Length; i++)
                {
                    switch (Value[i])
                    {
                        case '(':
                            parenthesesCounter++;
                            break;
                        case ')':
                            parenthesesCounter--;
                            break;
                        case '^':
                            if (parenthesesCounter == 0 && Value[i + 1] == '+' && i == Value.Length - 2 && numberOfParentheses == 2)
                                return true;
                            break;
                    }
                }
                return false;
            }
        }

        public void GetConcatSubExpressions(out RegularExpression r, out RegularExpression s)
        {
            //if (!IsConcatenation)
            //    throw new Exception();
            r = null;
            s = null;

            switch (Value[0])
            {
                case '(':
                    //Beginning with a '('
                    Stack stack = new Stack();
                    int parenthesesCounter = 0;
                    for (int i = 0; i < Value.Length; i++)
                    {
                        if (Value[i] == '(') parenthesesCounter++;
                        else if (Value[i] == ')') parenthesesCounter--;
                        stack.Push(Value[i]);
                        if (parenthesesCounter == 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            while (stack.Count != 0)
                                sb.Append(stack.Pop());
                            char[] rValue = sb.ToString().ToCharArray();
                            Array.Reverse(rValue);

                            switch(Value[i+1])
                            {
                                case '*':
                                    r = new RegularExpression(new String(rValue) + Value[i+1]);
                                    s = new RegularExpression(Value.Substring(i + 2));
                                    break;
                                case '^':
                                    r = new RegularExpression(new String(rValue) + Value[i + 1] + Value[i+2]);
                                    s = new RegularExpression(Value.Substring(i + 3));
                                    break;
                                default:
                                    r = new RegularExpression(new String(rValue));
                                    s = new RegularExpression(Value.Substring(i + 1));
                                    break;
                            }
                            return;
                        }
                    }
                    break;
                default:
                    //Beginning with a character
                    switch (Value[1])
                    {
                        case '*':
                            r = new RegularExpression(Value.Substring(0, 2));
                            s = new RegularExpression(Value.Substring(2));
                            break;
                        case '^':
                            r = new RegularExpression(Value.Substring(0, 3));
                            s = new RegularExpression(Value.Substring(3));
                            break;
                        default:
                            r = new RegularExpression(Value[0]);
                            s = new RegularExpression(Value.Substring(1));
                            break;
                    }
                    return;
            }
        }

        public void GetUnionSubExpressions(out RegularExpression r, out RegularExpression s)
        {
            if (!IsUnion)
                throw new Exception();
            r = s = null;
            Stack stack = new Stack();
            int parentheses = 0;
            
            for (int i = 0; i < Value.Length; i++)
            {
                switch (Value[i])
                {
                    case '(':
                        parentheses++;
                        break;
                    case ')':
                        parentheses--;
                        break;
                    case '+':
                        if (parentheses == 0 && Value[i - 1] != '^')
                        {
                            r = new RegularExpression(Value.Substring(0,i));
                            s = new RegularExpression(Value.Substring(i+1));
                        }
                        break;
                }
            }
        }

        public RegularExpression Concatenate(RegularExpression re)
        {
            Value += re.Value;
            return this;
        }

        public RegularExpression Union(RegularExpression re)
        {
            Value += "+" + re.Value;
            return this;
        }

        #region PRIVATE FUNCTIONS

        private void removeObsoleteParentheses(ref string str)
        {
            while (containsObsoloeteParentheses(str))
            {
                str = str.Substring(1, str.Length - 2);
            }
        }

        private bool containsObsoloeteParentheses(string str)
        {
            int parentheses = 0;
            bool closedInObsoleteParentheses = false;
            if (str[0] == '(') closedInObsoleteParentheses = true;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '(':
                        parentheses++;
                        break;
                    case ')':
                        parentheses--;
                        if (closedInObsoleteParentheses && parentheses == 0 && i != str.Length - 1)
                            closedInObsoleteParentheses = false;
                        break;
                }
            }
            return closedInObsoleteParentheses;
        }

        #endregion
    }
}
