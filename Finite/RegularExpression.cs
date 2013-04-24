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

        

        public RegularExpression() : this(EMPTY_WORD) {}

        public RegularExpression(char c) : this(c.ToString()) {}

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
                            if (parentheses == 0)
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
                //TODO
                return false;
            }
        }

        public bool IsKleene
        {
            get
            {
                //TODO
                return false;
            }
        }

        public void GetConcatSubExpressions(out RegularExpression r, out RegularExpression s)
        {
            r = null;
            s = null;

            //Beginning with a character
            if (Value[0] != '(')
            {
                r = new RegularExpression(Value[0]);
                s = new RegularExpression(Value.Substring(1));
                return;
            }

            //Beginning with a '('
            Stack stack = new Stack();
            int parenthesesCounter = 0;
            for(int i = 0; i < Value.Length; i++)
            {
                if (Value[i] == '(') parenthesesCounter++;
                else if (Value[i] == ')') parenthesesCounter--;
                stack.Push(Value[i]);
                if (parenthesesCounter == 0)
                {
                    StringBuilder sb = new StringBuilder();
                    while(stack.Count != 0)
                        sb.Append(stack.Pop());
                    char[] rValue = sb.ToString().ToCharArray();
                    Array.Reverse(rValue);
                    r = new RegularExpression(new String(rValue));
                    s = new RegularExpression(Value.Substring(i+1));
                    return;
                }
            }
        }

        public void GetUnionSubExpressions(out RegularExpression r, out RegularExpression s)
        {
            //TODO
            r = s = null;
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
