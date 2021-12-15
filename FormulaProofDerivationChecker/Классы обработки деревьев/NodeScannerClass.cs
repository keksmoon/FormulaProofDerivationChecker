using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaProofDerivationChecker
{
    public static class NodeScannerClass
    {
        public static bool IsNodeWithReplacementsMatch(Node axiomNode, Node deritivateNode)
        {
            Dictionary<char, Node> nodeReplacements = new Dictionary<char, Node>();

            Stack<Node> termsAxiom = new Stack<Node>();
            Stack<Node> termsDeritivate = new Stack<Node>();
            while (termsAxiom.Count * termsDeritivate.Count != 0 || (axiomNode != null && deritivateNode != null))
            {
                if (axiomNode != null && deritivateNode != null)
                {
                    if (axiomNode.value != deritivateNode.value)
                    {
                        // Если наткунлись на расхождения в разных деревьях, то производим замену
                        // если замена уже была сделана для данной переменной
                        // то сверяем что ей соответсвует уже с тем что встретили вновь
                        if (!nodeReplacements.ContainsKey(axiomNode.value))
                        {
                            if (!char.IsLetter(axiomNode.value))
                            {
                                //Console.WriteLine("Second f ist keine auch axiom");
                                return false;
                            }

                            nodeReplacements.Add(axiomNode.value, deritivateNode);
                            //Console.WriteLine("visited {0} : {1}", axiomNode.value, deritivateNode.ToString());
                            deritivateNode = axiomNode;
                        } else
                        {
                            if (deritivateNode.ToString() == nodeReplacements[axiomNode.value].ToString())
                            {
                                deritivateNode = axiomNode;
                            } else
                            {
                                //Console.WriteLine("Second f ist keine auch axiom weil {0} = {1} != {2}", axiomNode.value, nodeReplacements[axiomNode.value].ToString(), deritivateNode.ToString());
                                return false;
                            }
                            continue;
                        }

                        // Бежим по второму дереву вперед, пока не встретим операцию
                        if ("+-*>".IndexOf(deritivateNode.value) != -1)
                        {
                            deritivateNode = ((NodeTwo)deritivateNode).left;
                        }
                        else if (deritivateNode.value == '~' || char.IsLetter(deritivateNode.value))
                        {
                            deritivateNode = ((NodeOne)deritivateNode).child;
                        }

                        continue;
                    }

                   // if (nodeReplacements.ContainsKey(axiomNode.value))
                   //     Console.WriteLine("visited {0} : {1}", axiomNode.value, nodeReplacements[axiomNode.value].ToString());
                   // else
                   //     Console.WriteLine("visited {0} : {1}", axiomNode.value, deritivateNode.value);

                    // Параллельный обход одинаковых частей двух деревьев
                    if ("+-*>".IndexOf(axiomNode.value) != -1 && "+-*>".IndexOf(deritivateNode.value) != -1)
                    {
                        if (((NodeTwo)axiomNode).right != null && ((NodeTwo)deritivateNode).right != null)
                        {
                            termsAxiom.Push(((NodeTwo)axiomNode).right);
                            termsDeritivate.Push(((NodeTwo)deritivateNode).right);
                        }
                        axiomNode = ((NodeTwo)axiomNode).left;
                        deritivateNode = ((NodeTwo)deritivateNode).left;
                    }
                    else if ((axiomNode.value == '~' || char.IsLetter(axiomNode.value)) && (deritivateNode.value == '~' || char.IsLetter(deritivateNode.value)))
                    {
                        axiomNode = ((NodeOne)axiomNode).child;
                        deritivateNode = ((NodeOne)deritivateNode).child;
                    }
                }
                else
                {
                    axiomNode = termsAxiom.Pop();
                    deritivateNode = termsDeritivate.Pop();
                }
            }

            return true;
        }

        /// <summary>
        /// Проверка корректности проведения правила MP.
        /// </summary>
        /// <param name="parent">Формула, к которой применяем правило MP</param>
        /// <param name="hypotheses">Формула, которую планируем отрезать с помощью MP</param>
        /// <param name="receiver">Формула, которая получилась после применения MP</param>
        /// <returns>True or False</returns>
        public static bool IsNodeReceivedAsMP(Node parent, Node hypotheses, Node receiver)
        {
            if (parent.value == '>')
            {
                Node theoreticalHypotheses = ((NodeTwo)parent).left;
                Node theoreticalReceiver = ((NodeTwo)parent).right;

                if (theoreticalHypotheses == hypotheses && theoreticalReceiver == receiver) return true;
                return false;
            }
            else return false;
        }
    }
}
