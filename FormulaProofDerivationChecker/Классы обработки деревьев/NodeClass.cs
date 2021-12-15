using System;
using System.Collections.Generic;
using System.Text;

namespace FormulaProofDerivationChecker
{
    public abstract class Node : IEquatable<Node>
    {
        public char value;

        public Node(char value)
        {
            this.value = value;
        }

        public static Node BuildTree(ReversePolishNotation reversePolishNotation)
        {
            Stack<Node> stack = new Stack<Node>();
            string reversePolishNotationExpression = reversePolishNotation.ToString();

            foreach (char c in reversePolishNotationExpression)
            {
                Node node = null;
                if (c != ' ')
                {
                    if ("+-*>".IndexOf(c) != -1)
                    {
                        node = new NodeTwo(c);
                    } else
                    {
                        node = new NodeOne(c);
                    }

                    if ("+-*>~".IndexOf(c) != -1)
                    {
                        if (c != '~')
                        {
                            ((NodeTwo)node).right = stack.Pop();
                            ((NodeTwo)node).left = stack.Pop();
                        }
                        else
                        {
                            ((NodeOne)node).child = stack.Pop();
                        }
                    }
                }
                stack.Push(node);
            }

            if (stack.Count != 1)
            {
                throw new Exception("Expected exactly one stack value.");
            }
            return stack.Pop();
        }

        #region PostOrder()
        public string PostOrder()
        {
            StringBuilder sb = new StringBuilder(string.Empty);

            PostOrder(ref sb);

            return sb.ToString();
        }
        private void PostOrder(ref StringBuilder sb)
        {
            if ("+-*>".IndexOf(value) != -1)
            {
                ((NodeTwo)this).left.PostOrder(ref sb);
                ((NodeTwo)this).right.PostOrder(ref sb);
                sb.Append(value);
            } else if (value == '~')
            {
                ((NodeOne)this).child.PostOrder(ref sb);
                sb.Append(value);
            } else
            {
                sb.Append(value);
            }
        }
        #endregion PostOrder()

        #region PreOrder()
        public string PreOrder()
        {
            StringBuilder sb = new StringBuilder(string.Empty);

            PreOrder(ref sb);

            return sb.ToString();
        }
        private void PreOrder(ref StringBuilder sb)
        {
            if ("+-*>".IndexOf(value) != -1)
            {
                sb.Append(value);
                ((NodeTwo)this).left.PreOrder(ref sb);
                ((NodeTwo)this).right.PreOrder(ref sb);
            }
            else if (value == '~')
            {
                ((NodeOne)this).child.PreOrder(ref sb);
                sb.Append(value);
            }
            else
            {
                sb.Append(value);
            }
        }
        #endregion PreOrder()

        public override string ToString()
        {
            if ("+-*>".IndexOf(value) != -1)
            {
                return ((NodeTwo)this).left == null ? value.ToString() :
                 ("(" + ((NodeTwo)this).left + " " + value + " " + ((NodeTwo)this).right + ")");
            } else 
            {
                return ((NodeOne)this).child == null ? value.ToString() : value + " " + ((NodeOne)this).child;
            } 
        }

        public override bool Equals(object obj) => this.Equals(obj as NodeTwo);

        public bool Equals(Node node)
        {
            if (node is null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, node))
            {
                return true;
            }

            if (this.GetType() != node.GetType())
            {
                return false;
            }

            return this.ToString().CompareTo(node.ToString()) == 0;
        }

        public static bool operator ==(Node node1, Node node2)
        {
            if (node1 is null)
            {
                if (node2 is null)
                {
                    return true;
                }

                return false;
            }

            return node1.Equals(node2);
        }

        public static bool operator !=(Node node1, Node node2) => !(node1 == node2);

        public Formula ToFormula()
        {
            return new Formula(string.Join("", this.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
        }
    }
    public class NodeTwo : Node
    {
        public Node left, right;

        public NodeTwo(char value) : base(value)
        {
        }
    }
    public class NodeOne : Node
    {
        public Node child;

        public NodeOne(char value) : base(value)
        {
        }
    }
}
