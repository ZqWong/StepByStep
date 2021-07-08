using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Example;
using LitJson;
using NCalc;
using UnityEngine;
using xr.StepByStepFramework.DataModel;
using xr.StepByStepFramework.Expressions;
using xr.StepByStepFramework.Feedback;

public class AppManager : MonoBehaviour
{
    // Start is called before the first frame update

    public 

    void Start()
    {

        //Debug.Log(LinearStepDataManager.Instance.StepDataEnumerator.Current.Id);

        //FeedContentAnim var = new FeedContentAnim(this.gameObject, new FeedbackDataModelBase());

        //Expression e = new Expression("2 + 3 * 5");
        //object result = e.Evaluate();

        //Debug.Log(result);


        //Debug.Log(123456 == int.Parse(new Expression("123456").Evaluate().ToString())); // integers
        //Debug.Assert(new DateTime(2001, 01, 01) == new Expression("#01/01/2001#").Evaluate()); // date and times
        //Debug.Assert(123.456 == new Expression("123.456").Evaluate()); // floating point numbers
        //Debug.Assert(true == new Expression("true").Evaluate()); // booleans
        //Debug.Assert("azerty" == new Expression("'azerty'").Evaluate()); // strings

        //Debug.Log(new Expression("Sqrt(4)").Evaluate());

        //Expression e = new Expression("SecretOperation(3, 6)");
        //e.EvaluateFunction += Plus;
        //Debug.Log(e.Evaluate());


        //ExpressionManager.Instance.CreateExpression("Plus", Plus);
        //ExpressionManager.Instance.CreateExpression("Plus", Max);

        //var ret = ExpressionManager.Instance.ExecuteExpression("Plus", "Plus(1, 2)");

        //Debug.Log(ret);
    }

    //public void Function(string name, FunctionArgs args)
    //{
    //    if (name == "SecretOperation")
    //        args.Result = (int)args.Parameters[0].Evaluate() + (int)args.Parameters[1].Evaluate();
    //}

    //public void Plus(string name, FunctionArgs args)
    //{
    //    if (name == "Plus")
    //    {
    //        args.Result = (int) args.Parameters[0].Evaluate() + (int) args.Parameters[1].Evaluate();
    //    }
    //}

    //private int max = 0;
    //public void Max(string name, FunctionArgs args)
    //{
    //    max = (int)args.Parameters[0].Evaluate() + (int)args.Parameters[1].Evaluate();
    //    Debug.LogError(max);
    //}

    //// Update is called once per frame
    //public void getbool(string name, FunctionArgs args)
    //{
    //    args.Result = true;
    //}
}
