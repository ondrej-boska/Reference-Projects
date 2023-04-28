using System;
using System.Collections.Generic;
#nullable enable

namespace ExpressionEvaluation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? currLine;
            ExpressionTree? expression = null;
            while ((currLine = Console.ReadLine()) is not null && currLine != "end")
            {
                if (currLine.Length == 0) continue;
                else if (currLine == "i")
                {
                    if (expression is null) Console.WriteLine(ErrorTypes.ExprMissing);
                    else
                    {
                        int? intResult = expression.EvaluateInt();
                        if (expression.IsValid) Console.WriteLine(intResult);
                        else Console.WriteLine(expression.ErrorValue);
                    }
                }
                else if (currLine == "d")
                {
                    if (expression is null) Console.WriteLine(ErrorTypes.ExprMissing);
                    else
                    {
                        double? doubleResult = expression.EvaluateDouble();
                        if (expression.IsValid && doubleResult is not null) Console.WriteLine(doubleResult.Value.ToString("f05"));
                        else Console.WriteLine(expression.ErrorValue);
                    }
                }
                else if(currLine == "p")
                {
                    if (expression is null) Console.WriteLine(ErrorTypes.ExprMissing);
                    else if (expression.ErrorValue != ErrorTypes.Format)
                    {
                        MaxParenthesesStringVisitor visitor = new();
                        expression.Root!.Accept(visitor);
                        Console.WriteLine(visitor.expressionString.ToString());
                    }
                    else Console.WriteLine(expression.ErrorValue);
                }
                else if (currLine == "P")
                {
                    if (expression is null) Console.WriteLine(ErrorTypes.ExprMissing);
                    else if (expression.ErrorValue != ErrorTypes.Format)
                    {
                        MinParenthesesStringVisitor visitor = new();
                        expression.Root!.Accept(visitor);
                        Console.WriteLine(visitor.expressionString.ToString());
                    }
                    else Console.WriteLine(expression.ErrorValue);
                }
                else if (currLine[0] == '=')
                {
                    if (currLine.Length > 2) expression = ExpressionTree.BuildTreeFromPrefix(currLine.Substring(2));
                    if (currLine.Length <= 2 || !expression!.IsValid)
                    {
                        Console.WriteLine(ErrorTypes.Format);
                        expression = null;
                    }
                }
                else
                    Console.WriteLine(ErrorTypes.Format);
            }
        }
    }
}