using System;
using LitJson;
using xr.StepByStep.DataModel;

namespace xr.StepByStep.DataModel
{
    [Serializable]
    public class FeedbackDataModelBase : BaseParamsDataModel
    {
        public FeedbackDataModelBase() { }

        public FeedbackTypeEnum StepType { get; set; }


        public FeedbackDataModelBase(JsonData jdata) : base(jdata)
        {
            var str = jdata["FeedbackType"].ToString() == null ? "NONE" : jdata["FeedbackType"].ToString();


        }
    }
}
