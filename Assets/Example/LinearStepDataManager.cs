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

public class LinearStepDataManager : SingletonMonoBehaviourClass<LinearStepDataManager>, IDataManager
{
    public List<JsonData> StepDataCollection;

    public List<JsonData>.Enumerator StepDataEnumerator;

    private void OnEnable()
    {
        SingletonProvider<EventManager>.Instance.RegisterEvent(FSMEventConst.LEAVE_END_STEP_KEY, StepMoveNextHandler);
        SingletonProvider<EventManager>.Instance.RegisterEvent(FSMEventConst.ENTER_START_STEP_KEY, StartStepFeedback);
    }

    private void OnDisable()
    {
        SingletonProvider<EventManager>.Instance.UnRegisterEventHandler(FSMEventConst.LEAVE_END_STEP_KEY, StepMoveNextHandler);
        SingletonProvider<EventManager>.Instance.UnRegisterEventHandler(FSMEventConst.ENTER_START_STEP_KEY, StartStepFeedback);
    }

    private void StepMoveNextHandler(object sender, EventArgs e)
    {
        StepMoveNext();
    }

    private void StartStepFeedback(object sender, EventArgs e)
    {
        Debug.Log("StartStepFeedback");
        FeedbackManager.Instance.PlayFeedback();
    }

    private void Awake()
    {
        JsonData jsonData = JsonTool.GetJsonData(Application.streamingAssetsPath + "/StepDataTest.json");
        Debug.Log("jsonString :" + JsonMapper.ToJson(jsonData));
        Initialize(jsonData);
    }

    public void OnClickStart()
    {
        SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.ENABLE_STEP_KEY, null);
    }

    public void Initialize(JsonData stepJsonData)
    {
        StepDataCollection = new List<JsonData>();

        foreach (JsonData stepItem in stepJsonData["stepItems"])
        {
            StepDataCollection.Add(stepItem);
        }

        Debug.Log("StepDataCollection.count :" + StepDataCollection.Count);

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

