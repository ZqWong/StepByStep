using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etp.xr.Tools;
using LitJson;
using UnityEngine;
using xr.StepByStepFramework.DataModel;
using xr.StepByStepFramework.Feedback_old;

namespace Assets.Example
{
    public class FeedbackFactory : SingletonMonoBehaviourClass<FeedbackFactory>, IFeedbackItemHandlerFactory
    {
        public FeedbackItemHandlerContentBase GetHandlerByStepType(JsonData jsonData, string stepType)
        {
            FeedbackItemHandlerContentBase ret = null;
            switch (jsonData[stepType].ToString())
            {
                case "Anim":

                    FeedbackDataModel data = new FeedbackDataModel(jsonData);
                    var owner = GameObject.Find(data.Owner);
                    ret = owner.gameObject.AddComponent<FeedContentAnim>();
                    ret.Initialize(data);
                    break;
            }
            return ret;
        }
    }
}
