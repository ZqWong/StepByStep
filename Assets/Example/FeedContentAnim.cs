using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using UnityEngine;
using xr.StepByStepFramework.DataModel;
using xr.StepByStepFramework.Feedback_old;

namespace Assets.Example
{
    public class FeedContentAnim : FeedbackItemHandlerContentBase
    {
        protected override void CustomInitialization(FeedbackDataModelBase dataModel)
        {
            var data = dataModel as FeedbackDataModel;
            Debug.Log("CustomInitialization :" + dataModel.FeedbackType + " data.Owner " + data.Owner);
        }

        protected override void CustomPlayCompeteCallback()
        {
            base.CustomPlayCompeteCallback();
            Debug.Log("CustomPlayCompeteCallback");
        }

        protected override void CustomExecuteHandler(FeedbackDataModelBase dataModel)
        {
            Debug.Log("CustomExecuteHandler :" + dataModel.FeedbackType);
        }

        public void OnClickAnimComplete()
        {
            CustomPlayCompeteCallback();
        }

        void OnGUI()
        {
            if (GUILayout.Button("AnimComplete"))
            {
                OnClickAnimComplete();
            }
        }
    }
}
