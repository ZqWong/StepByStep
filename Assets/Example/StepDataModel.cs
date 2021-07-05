using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Example;
using etp.xr.Tools;
using LitJson;
using xr.StepByStepFramework.DataModel;
using xr.StepByStepFramework.FSM;

[Serializable]
public class StepDataModel : BaseParamsDataModel
{
    public int Level;
    public string Param1;
    public string Param2;
    public string Param3;
    public string Param4;
    public string Param5;
    
    public List<FeedbackDataModel> FeedbackDataCollection;

    public StepDataModel(JsonData jsonData) : base(jsonData)
    {
        Level = jsonData.ContainsKey("level") ? jsonData["level"].ToInt() : -1;

        if (jsonData.ContainsKey("feedbacks"))
        {
            FeedbackDataCollection = new List<FeedbackDataModel>();

            foreach (JsonData feedback in jsonData["feedbacks"])
            {
                FeedbackDataCollection.Add(new FeedbackDataModel(feedback));
            }
        }
    }

    public StepDataModel(JsonData jsonData, string feedbacksKey) : base(jsonData)
    {
        Level = jsonData.ContainsKey("level") ? jsonData["level"].ToInt() : -1;

        if (jsonData.ContainsKey(feedbacksKey))
        {
            FeedbackDataCollection = new List<FeedbackDataModel>();

            foreach (JsonData feedback in jsonData[feedbacksKey])
            {
                FeedbackDataCollection.Add(new FeedbackDataModel(feedback));
            }
        }
    }
}

