using System.Text;
#nullable enable

namespace ExpressionEvaluation
{
    public interface INodeVisitor<T>
    {
        public T Visit(NumericNode node);
        public T Visit(PlusOperatorNode node);
        public T Visit(MinusOperatorNode node);
        public T Visit(MultiplyOperatorNode node);
        public T Visit(DivideOperatorNode node);
        public T Visit(UnaryMinusOperatorNode node);
    }

    public interface IVoidNodeVisitor
    {
        public void Visit(NumericNode node);
        public void Visit(PlusOperatorNode node);
        public void Visit(MinusOperatorNode node);
        public void Visit(MultiplyOperatorNode node);
        public void Visit(DivideOperatorNode node);
        public void Visit(UnaryMinusOperatorNode node);
    }

    public class IntNodeVisitor : INodeVisitor<int>
    {
        public int Visit(NumericNode node) => node.Value;
        public int Visit(PlusOperatorNode node) => checked(node.leftOperand.Accept(this) + node.rightOperand.Accept(this));
        public int Visit(MinusOperatorNode node) => checked(node.leftOperand.Accept(this) - node.rightOperand.Accept(this));
        public int Visit(MultiplyOperatorNode node) => checked(node.leftOperand.Accept(this) * node.rightOperand.Accept(this));
        public int Visit(DivideOperatorNode node) => checked(node.leftOperand.Accept(this) / node.rightOperand.Accept(this));
        public int Visit(UnaryMinusOperatorNode node) => checked(-node.operand.Accept(this));
    }

    public class DoubleNodeVisitor : INodeVisitor<double>
    {
        public double Visit(NumericNode node) => node.Value;
        public double Visit(PlusOperatorNode node) => node.leftOperand.Accept(this) + node.rightOperand.Accept(this);
        public double Visit(MinusOperatorNode node) => node.leftOperand.Accept(this) - node.rightOperand.Accept(this);
        public double Visit(MultiplyOperatorNode node) => node.leftOperand.Accept(this) * node.rightOperand.Accept(this);
        public double Visit(DivideOperatorNode node) => node.leftOperand.Accept(this) / node.rightOperand.Accept(this);
        public double Visit(UnaryMinusOperatorNode node) => -node.operand.Accept(this);
    }

    public class MaxParenthesesStringVisitor : IVoidNodeVisitor
    {
        public StringBuilder expressionString = new();
        public void Visit(NumericNode node) => expressionString.Append(node.Value);

        public void AppendBinaryOperator(BinaryOperatorNode node)
        {
            expressionString.Append('(');
            node.leftOperand.Accept(this);
            expressionString.Append(node.OperatorSign);
            node.rightOperand.Accept(this);
            expressionString.Append(')');
        }
        public void Visit(PlusOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(MinusOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(MultiplyOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(DivideOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(UnaryMinusOperatorNode node)
        {
            expressionString.Append("(-");
            node.operand.Accept(this);
            expressionString.Append(')');
        }
    }

    public class MinParenthesesStringVisitor : IVoidNodeVisitor
    {
        readonly OperationPriortyVisitor priority = new();
        public StringBuilder expressionString = new();

        public void Visit(NumericNode node) => expressionString.Append(node.Value);
        public void AppendBinaryOperator(BinaryOperatorNode node)
        {
            bool leftOperandParenthesized = node.leftOperand.Accept(priority) < node.Accept(priority);
            bool rightOperandParenthesized = node.rightOperand.Accept(priority) < node.Accept(priority);

            if (leftOperandParenthesized) expressionString.Append('(');
            node.leftOperand.Accept(this);
            if (leftOperandParenthesized) expressionString.Append(')');
            expressionString.Append(node.OperatorSign);
            if (rightOperandParenthesized) expressionString.Append('(');
            node.rightOperand.Accept(this);
            if (rightOperandParenthesized) expressionString.Append(')');
        }
        public void Visit(PlusOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(MinusOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(MultiplyOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(DivideOperatorNode node) => AppendBinaryOperator(node);
        public void Visit(UnaryMinusOperatorNode node)
        {
            bool operandParenthesized = node.operand.Accept(priority) < node.Accept(priority);
            expressionString.Append('-');
            if (operandParenthesized) expressionString.Append('(');
            node.operand.Accept(this);
            if (operandParenthesized) expressionString.Append(')');
        }
    }

    public class OperationPriortyVisitor : INodeVisitor<int>
    {
        public int Visit(NumericNode node) => 4;
        public int Visit(PlusOperatorNode node) => 1;
        public int Visit(MinusOperatorNode node) => 2;
        public int Visit(MultiplyOperatorNode node) => 3;
        public int Visit(DivideOperatorNode node) => 3;
        public int Visit(UnaryMinusOperatorNode node) => 3;
    }
}
