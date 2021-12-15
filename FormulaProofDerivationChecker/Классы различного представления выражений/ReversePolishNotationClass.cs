using System.Collections.Generic;

namespace FormulaProofDerivationChecker
{
    public class ReversePolishNotation
    {
        private string expression;

        public char[] ToCharArray()
        {
            return expression.ToCharArray();
        }

        public override string ToString()
        {
            return expression;
        }

        public ReversePolishNotation(Formula formula)
        {
            string expr = formula.ToString();
            Stack<KeyValuePair<char, int>> stack = new Stack<KeyValuePair<char, int>>();
            List<char> OPZ = new List<char>();
            foreach (char key in expr)
            {
                int Pr = key switch
                {
                    '(' => 0,
                    ')' => 1,
                    '+' => 2,
                    '*' => 3,
                    '>' => 4,
                    '~' => 5,
                    _ => -1,
                };

                if (Pr == -1)
                {
                    OPZ.Add(key);
                }
                else
                {
                    if (Pr == 0 || stack.Count == 0 || Pr >= stack.Peek().Value)
                    {
                        stack.Push(new KeyValuePair<char, int>(key, Pr));
                    }
                    else
                    {
                        while (stack.Count != 0 && Pr <= stack.Peek().Value)
                        {
                            OPZ.Add(stack.Pop().Key);
                        }
                        if (key == ')' && stack.Peek().Key == '(')
                        {
                            stack.Pop();
                        }
                        else
                        {
                            stack.Push(new KeyValuePair<char, int>(key, Pr));
                        }
                    }
                }

            }
            while (stack.Count != 0)
            {
                OPZ.Add(stack.Pop().Key);
            }

            expression = string.Join("", OPZ);
        }
    }
}
