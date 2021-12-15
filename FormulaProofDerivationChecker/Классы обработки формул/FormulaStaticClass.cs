using System.Collections.Generic;

namespace FormulaProofDerivationChecker
{
    public static class LogicFormula
    {
        private static bool IsBracketsGood(string expr)
        {
            int n = 0;
            foreach (var c in expr)
            {
                if (c == '(')
                    n++;
                if (c == ')')
                    n--;
            }
            return n == 0 ? true : false;
        }

        public static bool IsValidFormula(string expr)
        {
            if (IsBracketsGood(expr) == false) return false; // Проверка баланса скобочек
            var strChars = expr.ToCharArray(); // Получаем массив символов из формулы

            // Начало строки: 1) "(" 2) Отрицание 3) Переменная, если после неё конец строки
            // После "(": 1) "(" 2) Переменная 3) Отрицание
            // После Переменной: 1) Любая операция (и, или, импликация) 2) ")"
            // После Операции (и, или, импликация, отрицание): 1) Переменная 2) Отрицание 3) "("
            // После ")": 1) ")" 2) Операция (и, или, импликация) 3) Конец строки 

            if (strChars.Length == 0) return false;

            if (!IsBeginOfString(strChars[0])) return false;

            // Частный случай. Если выражение - переменная, то это формула.
            if (char.IsLetter(strChars[0]) && (strChars.Length == 1)) return true;

            // Количество скобочек должно соответствовать 2n-2, где n - количество всех переменных
            if (!IsBracketsValid(expr)) return false;

            // Операции должны быть обособлены скобочками. Должны быть внешние скобочки в длинных формулах.
            // Внутри скобочек должна быть хоть одна операция.
            if (!IsOperationValid(expr)) return false;

            for (int i = 1; i < strChars.Length; i++)
            {
                if ((strChars[i] == ')') && (i == strChars.Length)) continue;
                if ((strChars[i - 1] == '(') && IsCharAfterOpenBracket(strChars[i])) continue;
                if (((strChars[i - 1] == '~') || (strChars[i - 1] == '+') || (strChars[i - 1] == '*') || (strChars[i - 1] == '>')) && IsCharAfterOperation(strChars[i])) continue;
                if (char.IsLetter(strChars[i - 1]) && IsCharAfterLetter(strChars[i])) continue;
                if ((strChars[i - 1] == ')') && IsCharAfterCloseBracket(strChars[i])) continue;

                return false;
            }

            return true;
        }

        private static bool IsOperationValid(string expr)
        {
            List<char> BracketsAndOperations = new List<char>();
            foreach (char c in expr)
            {
                if (char.IsLetter(c) || c == ')' || c == '(') BracketsAndOperations.Add(c);
            }

            for (int i = 1; i < BracketsAndOperations.Count - 1; i++)
            {
                // Блокировка вариантов LLL без учета операций
                if (char.IsLetter(BracketsAndOperations[i - 1]) &&
                    char.IsLetter(BracketsAndOperations[i]) &&
                    char.IsLetter(BracketsAndOperations[i + 1])) return false;
            }

            for (int i = 1; i < expr.Length - 1; i++)
            {
                // Блокировка вариантов SLS с учетом операций
                if (((expr[i - 1] == ')') || (expr[i - 1] == '(')) &&
                    char.IsLetter(expr[i]) &&
                    ((expr[i + 1] == ')') || (expr[i + 1] == '('))) return false;
            }

            if (expr[0] != '~' && (expr[0] != '(' || expr[expr.Length - 1] != ')')) return false;

            return true;
        }
        private static int GetCountLettersInExpr(string expr)
        {
            int cnt = 0;
            foreach (char c in expr)
            {
                if (char.IsLetter(c)) cnt++;
            }

            return cnt;
        }
        private static bool IsBracketsValid(string expr)
        {
            int RealCountOpenBrackets = 0;
            int RealCountCloseBrackets = 0;
            int CountBrackets = 2 * GetCountLettersInExpr(expr) - 2;
            foreach (char c in expr)
            {
                if (c == '(') RealCountOpenBrackets++;
                if (c == ')') RealCountCloseBrackets++;
            }

            return (RealCountOpenBrackets + RealCountCloseBrackets) == CountBrackets;
        }
        private static bool IsCharAfterLetter(char key)
        {
            if ((key == '~') || (key == '>') || (key == '+') || (key == '*') || (key == ')'))
                return true;
            return false;
        }
        private static bool IsCharAfterOperation(char key)
        {
            if ((key == '(') || (key == '~') || (char.IsLetter(key)))
                return true;
            return false;
        }
        private static bool IsBeginOfString(char key)
        {
            if ((key == '(') || (key == '~') || (char.IsLetter(key)))
                return true;
            return false;
        }
        private static bool IsCharAfterOpenBracket(char key)
        {
            if ((key == '(') || (key == '~') || (char.IsLetter(key)))
                return true;
            return false;
        }
        private static bool IsCharAfterCloseBracket(char key)
        {
            if ((key == ')') || (key == '>') || (key == '+') || (key == '*'))
                return true;
            return false;
        }
    }
}
