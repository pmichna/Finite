using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public static class DFABuilder
    {
        private static RegularExpression CreateEmptySet()
        {
            return new RegularExpression(RegularExpression.EMPTY_SET);
        }

        //helper function
        private static RegularExpression v(RegularExpression re)
        {
            RegularExpression r, s;

            if (re.IsEmptyWord)
            {
                return new RegularExpression();
            }
            else if ((!re.IsEmptyWord && re.Value.Length == 1) || re.IsEmptySet)
            {
                return CreateEmptySet();
            }
            else if (re.IsConcatenation)
            {
                re.GetConcatSubExpressions(out r, out s);
                return v(r).Concatenate(v(s));
            }
            else if (re.IsUnion)
            {
                re.GetUnionSubExpressions(out r, out s);
                return v(r).Union(v(s));
            }
            else if (re.IsKleene)
            {
                return new RegularExpression();
            }

            return null; //indicates error
        }

        public static RegularExpression Derive(RegularExpression re, char a)
        {
            RegularExpression r, s;
            if (re.IsEmptyWord)
            {
                return CreateEmptySet();
            }
            else if (re.Value == a.ToString())
            {
                return new RegularExpression();
            }
            else if (re.Value.Length == 1 && !(re.Value == a.ToString()))
            {
                return CreateEmptySet();
            }
            else if (re.Value.Equals(RegularExpression.EMPTY_SET))
            {
                return CreateEmptySet();
            }
            else if (re.IsConcatenation)
            {
                re.GetConcatSubExpressions(out r, out s);
                return Derive(r, a).Concatenate(s).Union(v(r).Concatenate(Derive(s, a)));
            }
            else if (re.IsKleene)
            {
                string expUnderStar;
                if (re.Value[0] == '(')
                {
                    expUnderStar = re.Value.Substring(1, re.Value.Length - 3);
                }
                else
                {
                    expUnderStar = re.Value[0].ToString();
                }
                return Derive(new RegularExpression(expUnderStar), a).Concatenate(new RegularExpression(re.Value));
            }
            else if (re.IsUnion)
            {
                re.GetUnionSubExpressions(out r, out s);
                return Derive(r, a).Union(Derive(s, a));
            }

            return null; //indicates error
        }


    }
}
