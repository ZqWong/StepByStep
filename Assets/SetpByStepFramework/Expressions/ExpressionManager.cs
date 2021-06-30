using System.Collections.Generic;
using NCalc;
using UnityEngine;
using etp.xr.Tools;

/// <summary>
/// https://github.com/ncalc/ncalc
/// </summary>

namespace xr.SetpByStepFramework.Expressions
{
    public class ExpressionManager : SingletonMonoBehaviourClass<ExpressionManager>
    {
        public Dictionary<string, ExpressionTemplate> ExpressionCollection = new Dictionary<string, ExpressionTemplate>();

        public void CreateExpression(string expressionKey, EvaluateFunctionHandler expression)
        {
            ExpressionCollection.Add(expressionKey, new ExpressionTemplate(expressionKey, expression));
        }

        public object ExecuteExpression(string expressionKey, string expressionStr)
        {
            Debug.Assert(ExpressionCollection.ContainsKey(expressionKey), $"Can't find target expression key ({expressionKey}) in Expression collection!");
            return ExpressionCollection[expressionKey].RunExpression(expressionStr);
        }
    }

    public class ExpressionTemplate
    {
        public string ExpressionKey { get; private set; }
        public string ExpressionStr { get; private set; }
        public EvaluateFunctionHandler EvaluateFunction { get; private set; }

        public ExpressionTemplate() { }

        public ExpressionTemplate(string expressionKey, EvaluateFunctionHandler Expression)
        {
            ExpressionKey = expressionKey;
            EvaluateFunction += Expression;
        }

        public object RunExpression(string expressionStr)
        {
            Expression e = new Expression(expressionStr);
            e.EvaluateFunction += EvaluateFunction;
            if (e.HasErrors())
            {
                Debug.LogError($"RunExpression failed, ExpressionKey : {ExpressionKey}, ExpressionStr : {ExpressionStr}");
                return null;
            }

            return e.Evaluate();            
        }
    }
}
