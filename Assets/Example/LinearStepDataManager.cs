using etp.xr.Tools;
using LitJson;
using xr.StepByStep.Manager;
using UnityEngine;
using System.Collections.Generic;
using etp.xr.Managers;
using xr.StepByStepFramework.FSM;
using System;
using Assets.Example;
using xr.StepByStepFramework.Feedback_old;
using JsonData = LitJson.JsonData;

public class LinearStepDataManager : SingletonMonoBehaviourClass<LinearStepDataManager>, IDataManager
{
    
    public List<JsonData> StepDataCollection;

    public List<JsonData>.Enumerator StepDataEnumerator;

    private void OnEnable()
    {
        SingletonProvider<EventManager>.Instance.RegisterEvent(FSMEventConst.LEAVE_END_STEP_KEY, StepMoveNextHandler);
    }

    private void OnDisable()
    {
        SingletonProvider<EventManager>.Instance.UnRegisterEventHandler(FSMEventConst.LEAVE_END_STEP_KEY, StepMoveNextHandler);
    }

    private void StepMoveNextHandler(object sender, EventArgs e)
    {
        StepMoveNext();
    }

    private void Awake()
    {
        JsonData jsonData = JsonTool.GetJsonData(Application.streamingAssetsPath + "/StepDataTest.json");
        Debug.Log("jsonString :" + JsonMapper.ToJson(jsonData));
        Initialize(jsonData);
    }

    public void Initialize(JsonData stepJsonData)
    {
        StepDataCollection = new List<JsonData>();

        foreach (JsonData stepItem in stepJsonData["stepItems"])
        {
            StepDataCollection.Add(stepItem);
        }

        StepDataEnumerator = StepDataCollection.GetEnumerator();
        StepMoveNext();
    }

    public void StepMoveNext()
    {
        if (CheckCurrentStepComplete())
        {
            if (StepDataEnumerator.MoveNext())
            {
                FeedbackManager.Instance.FeedbackFactoryInitialize = 
                    (jsonData, feedbackType) => 
                        FeedbackFactory.Instance.GetHandlerByStepType(jsonData, feedbackType);

                FeedbackManager.Instance.InitializeWithNewFeedback(StepDataEnumerator.Current);
            }
        }
    }

    public bool CheckCurrentStepComplete()
    {
        return true;
    }
}

