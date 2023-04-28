using System;
using System.Collections.Generic;
#nullable enable

namespace ExpressionEvaluation
{
    /// <summary>
    /// Represents all ExpressionTree nodes
    /// </summary>
    public abstract class ExpressionTreeNode
    {
        public abstract T Accept<T>(INodeVisitor<T> visitor);
        public abstract void Accept(IVoidNodeVisitor visitor);
    }
    /// <summary>
    /// Represents all operators in the ExpressionTree. When creating new operators, do not inherit from this class, choose Unary/Binary/NaryOperatorNode instead
    /// </summary>
    public abstract class OperatorNode : ExpressionTreeNode
    {
        public abstract string OperatorSign { get; }
        public override string ToString() => OperatorSign;
    }
    /// <summary>
    /// Abstract class, represents unary operator nodes. Stores an ExpressionTreeNode operand.
    /// </summary>
    public abstract class UnaryOperatorNode : OperatorNode
    {
        public ExpressionTreeNode operand;
        public UnaryOperatorNode() => operand = EmptyNode.instance;
        public void SetOperand(ExpressionTreeNode operand) => this.operand = operand;
    }
    /// <summary>
    /// Abstract class, represents binary operator nodes. Operands are stored seperatly as leftOperand and rightOperand.
    /// </summary>
    public abstract class BinaryOperatorNode : OperatorNode
    {
        public ExpressionTreeNode leftOperand;
        public ExpressionTreeNode rightOperand;

        public BinaryOperatorNode() 
        {
            leftOperand = EmptyNode.instance;
            rightOperand = EmptyNode.instance;
        }
        public void SetOperands(ExpressionTreeNode leftOperand, ExpressionTreeNode rightOperand)
        {
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
        }
    }
    /// <summary>
    /// Abstract class for representing operators with any arity. Operands are stored in a List.
    /// </summary>
    public abstract class NaryOperatorNode : OperatorNode
    {
        public abstract int Arity { get; }
        public List<ExpressionTreeNode> operands = new();

        public NaryOperatorNode() { }
        public NaryOperatorNode(params ExpressionTreeNode[] givenOperands) => SetOperands(givenOperands);
        public void SetOperands(params ExpressionTreeNode[] givenOperands)
        {
            operands.Clear();
            for (int i = 0; i < givenOperands.Length; i++)
                operands.Add(givenOperands[i]);
        }

        public void AddOperand(ExpressionTreeNode operand) => operands.Add(operand);
    }
    public sealed class EmptyNode : ExpressionTreeNode
    {
        public readonly static EmptyNode instance = new();
        public override T Accept<T>(INodeVisitor<T> visitor) => throw new ArgumentException("Invalid operation: calling an accept function on empty node.");
        public override void Accept(IVoidNodeVisitor visitor) => throw new ArgumentException("Invalid operation: calling an accept function on empty node.");
    }
    public sealed class NumericNode : ExpressionTreeNode
    {
        public readonly int Value;
        public override T Accept<T>(INodeVisitor<T> visitor) => visitor.Visit(this);
        public override void Accept(IVoidNodeVisitor visitor) => visitor.Visit(this);
        public NumericNode(int value) => this.Value = value;
        public override string ToString() => Value.ToString();
    }
    public sealed class PlusOperatorNode : BinaryOperatorNode
    {
        public override string OperatorSign => "+";
        public PlusOperatorNode() : base() { }
        public override T Accept<T>(INodeVisitor<T> visitor) => visitor.Visit(this);
        public override void Accept(IVoidNodeVisitor visitor) => visitor.Visit(this);
    }
    public sealed class MinusOperatorNode : BinaryOperatorNode
    {
        public override string OperatorSign => "-";
        public MinusOperatorNode() : base() { }
        public override T Accept<T>(INodeVisitor<T> visitor) => visitor.Visit(this);
        public override void Accept(IVoidNodeVisitor visitor) => visitor.Visit(this);
    }
    public sealed class MultiplyOperatorNode : BinaryOperatorNode
    {
        public override string OperatorSign => "*";
        public MultiplyOperatorNode() : base() { }
        public override T Accept<T>(INodeVisitor<T> visitor) => visitor.Visit(this);
        public override void Accept(IVoidNodeVisitor visitor) => visitor.Visit(this);
    }
    public sealed class DivideOperatorNode : BinaryOperatorNode
    {
        public override string OperatorSign => "/";
        public DivideOperatorNode() : base() { }
        public override T Accept<T>(INodeVisitor<T> visitor) => visitor.Visit(this);
        public override void Accept(IVoidNodeVisitor visitor) => visitor.Visit(this);
    }

    public sealed class UnaryMinusOperatorNode : UnaryOperatorNode
    {
        public override string OperatorSign => "~";
        public UnaryMinusOperatorNode() : base() { }
        public override T Accept<T>(INodeVisitor<T> visitor) => visitor.Visit(this);
        public override void Accept(IVoidNodeVisitor visitor) => visitor.Visit(this);
    }
}
