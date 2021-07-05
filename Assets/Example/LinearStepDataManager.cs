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

        // 监听FeedbackManager状态
        SingletonProvider<EventManager>.Instance.RegisterEvent(FeedbackEventConst.FEEDBACK_STATUS_EVENT_KEY, FeedbackStatusAdjustment);
    }

    private void OnDisable()
    {
        SingletonProvider<EventManager>.Instance.UnRegisterEventHandler(FSMEventConst.LEAVE_END_STEP_KEY, StepMoveNextHandler);
        SingletonProvider<EventManager>.Instance.UnRegisterEventHandler(FSMEventConst.ENTER_START_STEP_KEY, StartStepFeedback);

        // 监听FeedbackManager状态
        SingletonProvider<EventManager>.Instance.UnRegisterEventHandler(FeedbackEventConst.FEEDBACK_STATUS_EVENT_KEY, FeedbackStatusAdjustment);
    }

    /// <summary>
    /// 跳转下一步处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StepMoveNextHandler(object sender, EventArgs e)
    {
        StepMoveNext();
    }

    /// <summary>
    /// 开始进入步骤反馈处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartStepFeedback(object sender, EventArgs e)
    {
        Debug.Log("StartStepFeedback");
        FeedbackManager.Instance.PlayFeedback();
    }

    /// <summary>
    /// 工作流变更处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FeedbackStatusAdjustment(object sender, EventArgs e)
    {
        if (e is FeedbackEventArgs args)
        {
            switch (args.StatusType)
            {
                case FeedbackEventConst.EventTypes.Play:
                    break;

                case FeedbackEventConst.EventTypes.Pause:
                    break;

                case FeedbackEventConst.EventTypes.Resume:
                    break;

                case FeedbackEventConst.EventTypes.Revert:
                    break;

                case FeedbackEventConst.EventTypes.Complete:
                    break;

            }
        }
    }

    /// <summary>
    /// Awake初始化（test）
    /// </summary>
    private void Awake()
    {
        JsonData jsonData = JsonTool.GetJsonData(Application.streamingAssetsPath + "/StepDataTest.json");
        Debug.Log("jsonString :" + JsonMapper.ToJson(jsonData));
        Initialize(jsonData);
    }

    /// <summary>
    /// 启动步骤工作流
    /// </summary>
    public void EnableStepWorkFlow()
    {
        SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.ENABLE_STEP_KEY, null);
    }

    /// <summary>
    /// 初始化步骤模拟器
    /// </summary>
    /// <param name="stepJsonData"></param>
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

    /// <summary>
    /// 跳转下一步
    /// </summary>
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

