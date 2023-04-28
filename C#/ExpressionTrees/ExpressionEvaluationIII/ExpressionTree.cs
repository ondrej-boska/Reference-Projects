using System;
using System.Collections.Generic;
#nullable enable

namespace ExpressionEvaluation
{
    public static class ErrorTypes
    {
        public const string Overflow = "Overflow Error";
        public const string DivideByZero = "Divide Error";
        public const string Format = "Format Error";
        public const string ExprMissing = "Expression Missing";
    }
    /// <summary>
    /// Stores an expression in form of a tree
    /// </summary>
    public class ExpressionTree
    {
        public ExpressionTreeNode? Root { get; private set; }
        public bool IsValid { get; private set; } = true;
        public string? ErrorValue { get; private set; }

        /// <summary>
        /// Builds the tree from given prefix expression.
        /// </summary>
        /// <param name="expression">Expression in prefix notation</param>
        /// <returns>New instance of ExpressionTree</returns>
        public static ExpressionTree BuildTreeFromPrefix(string expression)
        {
            ExpressionTree tree = new();
            var operands = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            try
            {
                int index = -1;
                tree.Root = RecursivePrefixBuilder(ref index, operands);
                if (index != operands.Length - 1) throw new ArgumentException(ErrorTypes.Format);
            }
            catch(ArgumentException)
            {
                tree.ErrorValue = ErrorTypes.Format;
                tree.IsValid = false;
                tree.Root = null;
            }

            return tree;
        }

        static ExpressionTreeNode RecursivePrefixBuilder(ref int index, string[] operands)
        {
            ++index;
            if (index == operands.Length) throw new ArgumentException(ErrorTypes.Format);
            else if (int.TryParse(operands[index], out int value)) return new NumericNode(value);
            else if(OperatorFactory.Contains(operands[index]))
            {
                OperatorNode currNode = OperatorFactory.CreateNode(operands[index]);

                if (currNode is BinaryOperatorNode currBinaryOperatorNode)
                    currBinaryOperatorNode.SetOperands(RecursivePrefixBuilder(ref index, operands), RecursivePrefixBuilder(ref index, operands));
                else if (currNode is UnaryOperatorNode currUnaryOperatorNode)
                    currUnaryOperatorNode.SetOperand(RecursivePrefixBuilder(ref index, operands));
                else if (currNode is NaryOperatorNode currNaryOperatorNode)
                    for (int i = 0; i < currNaryOperatorNode.Arity; ++i) currNaryOperatorNode.AddOperand(RecursivePrefixBuilder(ref index, operands));
                else throw new ArgumentException(ErrorTypes.Format);

                return currNode;
            }
            else throw new ArgumentException(ErrorTypes.Format);
        }

        /// <summary>
        /// Evalutes the expression stored in the expression tree
        /// </summary>
        /// <returns> int? representing the result. If the result could not be computed, returns null and sets ErrorValue accordingly. </returns>
        public int? EvaluateInt()
        {
            if (Root is null) return null;
            try
            {   
                return Root.Accept(new IntNodeVisitor());
            }
            catch (DivideByZeroException)
            {
                ErrorValue = ErrorTypes.DivideByZero;
            }
            catch (OverflowException)
            {
                ErrorValue = ErrorTypes.Overflow;
            }

            IsValid = false;
            return null;
        }

        public double? EvaluateDouble()
        {
            if (Root is null) return null;
            if (ErrorValue != ErrorTypes.Format)
            {
                IsValid = true;
                ErrorValue = null;
            }
            return Root.Accept(new DoubleNodeVisitor());
        }
    }

    public static class OperatorFactory
    {
        static readonly Dictionary<string, Func<OperatorNode>> operators = new()
                {
                    { "+", () => new PlusOperatorNode() },
                    { "-", () => new MinusOperatorNode() },
                    { "*", () => new MultiplyOperatorNode() },
                    { "/", () => new DivideOperatorNode() },
                    { "~", () => new UnaryMinusOperatorNode() }
                };

        /// <summary>
        /// Adds support in the parser for new operator inheriting Unary/Binary/Nary OperatorNode
        /// </summary>
        /// <typeparam name="T">Class inheriting Unary/Binary/Nary OperatorNode</typeparam>
        public static void AddOperator<T>() where T : OperatorNode, new()
        {
            operators.TryAdd(new T().OperatorSign, () => new T());
        }
        /// <summary>
        /// Checks if given operator sign is supported
        /// </summary>
        /// <returns>True if given operator sign is supported, else returns false</returns>
        public static bool Contains(string operatorSign) => operators.ContainsKey(operatorSign);
        /// <summary>
        /// Returns a new instance of OperatorNode which correspons with given sign
        /// </summary>
        /// <returns>OperatorNode of correct type</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static OperatorNode CreateNode(string operatorSign) => operators[operatorSign]();
        /// <summary>
        /// Removes support for given operator sign
        /// </summary>
        public static void DeleteOperator(string operatorSign)
        {
            operators.Remove(operatorSign);
        }
    }
}
