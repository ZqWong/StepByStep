using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using xr.StepByStepFramework.DataModel;

namespace Assets.Example
{
    [Serializable]
    public class FeedbackDataModel: FeedbackDataModelBase
    {
        public string Owner;

        public FeedbackDataModel(JsonData jsonData) : base(jsonData)
        {
            Owner = jsonData["owner"].ToString() == null ? "NONE" : jsonData["owner"].ToString();
        }
    }
}
