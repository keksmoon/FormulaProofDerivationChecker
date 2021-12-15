using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FormulaProofDerivationChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("To exit enter 'q'. To clear console enter 'c'.");
            Console.Write("Input the number of the proof: ");
            string inputData = Console.ReadLine();
            Console.WriteLine();

            Stopwatch stopWatch = new Stopwatch();

            while (inputData != "q")
            {
                bool inputDataIsDigit = int.TryParse(inputData, out int proofNumber);
                if (inputDataIsDigit == false)
                {
                    if (inputData == "c")
                    {
                        Console.Clear();
                        Console.WriteLine("To exit enter 'q'. To clear console enter 'c'.");
                        inputData = InputDataFromConsole();
                        continue;
                    }

                    Console.WriteLine("Input string must be a number or command.");
                    inputData = InputDataFromConsole();
                    continue;
                }

                if (!Directory.Exists($"proof{proofNumber}"))
                {
                    Console.WriteLine("Directory with proof #{0} named \"proof{0}\" does not exist.", proofNumber);
                    inputData = InputDataFromConsole();
                    continue;
                }

                string nameOfFileWithAxioms = $"proof{proofNumber}/axioms{proofNumber}.txt";
                string nameOfFileWithHypotheses = $"proof{proofNumber}/hypotheses{proofNumber}.txt";
                string nameOfFileWithProof = $"proof{proofNumber}/proof{proofNumber}.txt";

                HashSet<Formula> axioms = new HashSet<Formula>();
                HashSet<Formula> hypotheses = new HashSet<Formula>();
                Dictionary<Formula, string> proof = new Dictionary<Formula, string>();
                List<Formula> proofDecision;

                StreamReader streamReader = null;
                if (!File.Exists(nameOfFileWithAxioms))
                {
                    Console.WriteLine("File {0} was not found", nameOfFileWithAxioms);
                    inputData = InputDataFromConsole();
                    continue;
                }
                streamReader = new StreamReader(nameOfFileWithAxioms);
                try
                {
                    axioms.UnionWith(streamReader.ReadToEnd().Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(f => new Formula(f)));
                    Console.WriteLine("\t Axioms:");
                    int AxiomCounterIndex = 1;
                    foreach (var axiom in axioms)
                    {
                        Console.WriteLine("{0}. {1};", AxiomCounterIndex++, axiom.ToString());
                    }
                    if (axioms.Count == 0)
                    {
                        Console.WriteLine("The set of axioms is empty.");
                    }
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Axioms is not correct because {0}.", ex.Message);
                    inputData = InputDataFromConsole();
                    continue;
                }
                streamReader.Close();

                if (!File.Exists(nameOfFileWithHypotheses))
                {
                    Console.WriteLine("File {0} was not found.", nameOfFileWithHypotheses);
                    inputData = InputDataFromConsole();
                    continue;
                }
                streamReader = new StreamReader(nameOfFileWithHypotheses);
                try
                {
                    hypotheses.UnionWith(streamReader.ReadToEnd().Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(f => new Formula(f)));
                    Console.WriteLine("\t Hypotheses:");
                    int HypothesesCounterIndex = 1;
                    foreach (var hypothese in hypotheses)
                    {
                        Console.WriteLine("{0}. {1};", HypothesesCounterIndex++, hypothese.ToString());
                    }
                    if (hypotheses.Count == 0)
                    {
                        Console.WriteLine("The set of hypotheses is empty.");
                    }
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hypotheses is not correct because {0}.", ex.Message);
                    inputData = InputDataFromConsole();
                    continue;
                }
                streamReader.Close();

                if (!File.Exists(nameOfFileWithProof))
                {
                    Console.WriteLine("File {0} was not found.", nameOfFileWithProof);
                    inputData = InputDataFromConsole();
                    continue;
                }
                streamReader = new StreamReader(nameOfFileWithProof);
                try
                {
                    proofDecision = new List<Formula>(streamReader.ReadToEnd().Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(f => new Formula(f)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Proof is not correct because {0}.", ex.Message);
                    inputData = InputDataFromConsole();
                    continue;
                }
                streamReader.Close();

                stopWatch.Start(); // Начало анализа вывода

                bool proofIsCorrect = true;
                for (int i = 0; i < proofDecision.Count; i++)
                {
                    Formula selected = proofDecision[i];

                    bool selectedFormulaIsHyptheses = false;
                    foreach (var hypFormula in hypotheses)
                    {
                        if (selected == hypFormula)
                        {
                            selectedFormulaIsHyptheses = true;
                            break;
                        }
                    }
                    if (selectedFormulaIsHyptheses)
                    {
                        proof.Add(selected, "hypotheses");
                        continue;
                    }

                    bool selectedFormulaIsAxiom = false;
                    Node selectedFormulaNode = Node.BuildTree(new ReversePolishNotation(selected));
                    foreach (var axiFormula in axioms)
                    {
                        Node formulaInAxiomsNode = Node.BuildTree(new ReversePolishNotation(axiFormula));
                        if (NodeScannerClass.IsNodeWithReplacementsMatch(formulaInAxiomsNode, selectedFormulaNode))
                        {
                            selectedFormulaIsAxiom = true;
                            break;
                        }
                    }
                    if (selectedFormulaIsAxiom)
                    {
                        proof.Add(selected, "axiom");
                        continue;
                    }

                    // проверяем является ли MP
                    bool selectedFormulaReceivedAsMP = false;
                    for (int j = 0; j < proof.Count - 1; j++)
                    {
                        for (int k = j + 1; k < proof.Count; k++)
                        {
                            Formula selectedFirst = proof.ElementAt(j).Key;
                            Formula selectedSecond = proof.ElementAt(k).Key;

                            var selectedFirstNode = Node.BuildTree(new ReversePolishNotation(selectedFirst));
                            var selectedSecondNode = Node.BuildTree(new ReversePolishNotation(selectedSecond));
                            if (NodeScannerClass.IsNodeReceivedAsMP(selectedFirstNode, selectedSecondNode, selectedFormulaNode))
                            {
                                // на случай если из вывод только из аксиом, то это аксиома
                                if (proof.ElementAt(j).Value == "axiom" && proof.ElementAt(k).Value == "axiom")
                                    if (!axioms.Contains(selectedFormulaNode.ToFormula()))
                                        axioms.Add(selectedFormulaNode.ToFormula());
                                    else
                                    if (!hypotheses.Contains(selectedFormulaNode.ToFormula()))
                                        hypotheses.Add(selectedFormulaNode.ToFormula());
                                proof.Add(selected, $"MP {j + 1}, {k + 1}");
                                selectedFormulaReceivedAsMP = true;
                                break;
                            }

                            if (NodeScannerClass.IsNodeReceivedAsMP(selectedSecondNode, selectedFirstNode, selectedFormulaNode))
                            {
                                if (proof.ElementAt(j).Value == "axiom" && proof.ElementAt(k).Value == "axiom")
                                    if (!axioms.Contains(selectedFormulaNode.ToFormula()))
                                        axioms.Add(selectedFormulaNode.ToFormula());
                                    else
                                        if (!hypotheses.Contains(selectedFormulaNode.ToFormula()))
                                        hypotheses.Add(selectedFormulaNode.ToFormula());
                                proof.Add(selected, $"MP {k + 1}, {j + 1}");
                                selectedFormulaReceivedAsMP = true;
                                break;
                            }
                        }
                        if (selectedFormulaReceivedAsMP) break;
                    }
                    if (selectedFormulaReceivedAsMP == false)
                    {
                        //Console.WriteLine("This proof is not correct.");
                        proofIsCorrect = false;
                        break;
                    }
                }

                var streamAnswerWriter = File.CreateText($"proof{proofNumber}/answer{proofNumber}.txt");
                int proofIndexCounter = 1;
                foreach (var p in proof)
                {
                    string stringToWriteAnswer = $"{proofIndexCounter++}. {p.Key}; | {p.Value}";
                    Console.WriteLine(stringToWriteAnswer);
                    streamAnswerWriter.WriteLine(stringToWriteAnswer);
                }

                if (proofIsCorrect == false)
                {
                    string stringToWriteAnswer = proofIndexCounter + ". " + proofDecision[proofIndexCounter - 1] + "; - error";
                    Console.WriteLine(stringToWriteAnswer);
                    streamAnswerWriter.WriteLine(stringToWriteAnswer);
                }

                Console.WriteLine(); streamAnswerWriter.WriteLine();

                if (proofIsCorrect)
                {
                    Console.WriteLine("Proof is correct.");
                    streamAnswerWriter.WriteLine("Proof is correct.");
                }
                else
                {
                    Console.WriteLine("This proof is not correct.");
                    streamAnswerWriter.WriteLine("This proof is not correct.");
                }

                stopWatch.Stop();

                Console.WriteLine("Answer saved to 'proof{0}/answer{0}.txt'", proofNumber);

                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                stopWatch.Reset();
                Console.WriteLine("RunTime: " + elapsedTime);
                Console.WriteLine();

                streamAnswerWriter.Close();

                inputData = InputDataFromConsole();
            }


            //var formulaAxiom = new Formula("((A>B)>((A>(B>C))>(A>C)))");
            //var formulaDerivative = new Formula("((~(A>B)>~(B>A))>((~(A>B)>(~(B>A)>A))>(~(A>B)>A)))");
            //ReversePolishNotation formulaAxiomRPN = new ReversePolishNotation(formulaAxiom);
            //Node formulaAxiomNode = Node.BuildTree(formulaAxiomRPN);

            //ReversePolishNotation formulaDerivativeRPN = new ReversePolishNotation(formulaDerivative);
            //Node formulaDeritivateNode = Node.BuildTree(formulaDerivativeRPN);

            //bool nodeMatchResult = NodeScannerClass.IsNodeWithReplacementsMatch(formulaAxiomNode, formulaDeritivateNode);
        }

        private static string InputDataFromConsole()
        {
            string inputData;
            Console.Write("Input the number of the proof: ");
            inputData = Console.ReadLine();
            Console.WriteLine();
            return inputData;
        }
    }
}
