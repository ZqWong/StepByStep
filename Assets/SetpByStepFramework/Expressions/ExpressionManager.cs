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
            // 支持一个表达式执行多个响应函数， 如果多个响应函数有返回值，则得到的值是最后一个执行的响应函数所得。
            if (ExpressionCollection.ContainsKey(expressionKey))
            {
                Debug.LogWarning($"The current expression key ( <color=red>{expressionKey}</color> ) has been used, \nplease confirm whether you want to add a new response function for this expression");
                ExpressionCollection[expressionKey].EvaluateFunction += expression;
            }
            else
            {
                ExpressionCollection.Add(expressionKey, new ExpressionTemplate(expressionKey, expression));
            }            
        }

        public object ExecuteExpression(string expressionKey, string expressionStr)
        {
            Debug.Assert(ExpressionCollection.ContainsKey(expressionKey), $"Can't find target expression key ( {expressionKey} ) in Expression collection!");
            return ExpressionCollection[expressionKey].RunExpression(expressionStr);
        }
    }

    public class ExpressionTemplate
    {
        public string ExpressionKey { get; private set; }
        public string ExpressionStr { get; private set; }
        public EvaluateFunctionHandler EvaluateFunction { get; set; }

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
