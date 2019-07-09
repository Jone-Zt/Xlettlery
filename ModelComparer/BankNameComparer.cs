using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using System.Text;

namespace ModelComparer
{
    public class BankNameComparer : EqualityComparer<SESENT_BankLineNumber>
    {
        public override bool Equals(SESENT_BankLineNumber x, SESENT_BankLineNumber y)
        {
            if (x == null || y == null)
                return false;
            else
                return x.BankName == y.BankName;
        }

        public override int GetHashCode(SESENT_BankLineNumber obj)
        {
            return obj.BankName.GetHashCode();
        }
    }
}
