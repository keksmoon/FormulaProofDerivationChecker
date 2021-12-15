using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaProofDerivationChecker
{
    public class Formula : IEquatable<Formula>
    {
        private string Expression;

        public override string ToString()
        {
            return Expression;
        }

        public override bool Equals(object obj) => this.Equals(obj as Formula);

        public bool Equals(Formula formula)
        {
            if (formula is null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, formula))
            {
                return true;
            }

            if (this.GetType() != formula.GetType())
            {
                return false;
            }

            return this.Expression.CompareTo(formula.Expression) == 0;
        }

        public override int GetHashCode() => Expression.GetHashCode();

        public static bool operator ==(Formula formula1, Formula formula2)
        {
            if (formula1 is null)
            {
                if (formula2 is null)
                {
                    return true;
                }

                return false;
            }

            return formula1.Equals(formula2);
        }

        public static bool operator !=(Formula formula1, Formula formula2) => !(formula1 == formula2);

        public Formula(string expression)
        {
            if (!LogicFormula.IsValidFormula(expression))
            {
                throw new Exception($"{expression} is not formula");
            }

            Expression = expression;
        }
    }
}
