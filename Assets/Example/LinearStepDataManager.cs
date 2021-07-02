using etp.xr.Tools;
using LitJson;
using xr.StepByStep.Manager;
using UnityEngine;
using System.Collections.Generic;
using etp.xr.Managers;
using xr.StepByStepFramework.FSM;
using System;

public class LinearStepDataManager : SingletonMonoBehaviourClass<LinearStepDataManager>, IDataManager
{
    [SerializeField]
    private List<StepDataModel> StepDataCollection;

    public List<StepDataModel>.Enumerator StepDataEnumerator;

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
        throw new NotImplementedException();
    }

    private void Awake()
    {
        JsonData jsonData = JsonTool.GetJsonData(Application.streamingAssetsPath + "/StepDataTest.json");
        Debug.Log("jsonString :" + JsonMapper.ToJson(jsonData));


        Initialize(jsonData);
    }

    public void Initialize(JsonData stepJsonData)
    {
        StepDataCollection = new List<StepDataModel>();

        foreach (JsonData stepItem in stepJsonData["stepItems"])
        {
            StepDataCollection.Add(new StepDataModel(stepItem));
        }

        StepDataEnumerator = StepDataCollection.GetEnumerator();

        StepMoveNext();
    }

    public void StepMoveNext()
    {
        StepDataEnumerator.MoveNext();
    }

}

