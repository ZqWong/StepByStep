using System;
using LitJson;

namespace xr.StepByStepFramework.DataModel
{
    [Serializable]
    public class FeedbackDataModelBase : BaseParamsDataModel
    {
        public string FeedbackType { get; set; }

        public string Params { get; set; }

        public FeedbackDataModelBase(JsonData jsonData, string feedbackTypeKey = "feedbackType", string paramsKey = "params") : base(jsonData)
        {
            FeedbackType = jsonData[feedbackTypeKey].ToString() == null ? "NONE" : jsonData[feedbackTypeKey].ToString();

            Params = jsonData[paramsKey].ToString() == null ? "NONE" : jsonData[paramsKey].ToString();
        }
    }
}
