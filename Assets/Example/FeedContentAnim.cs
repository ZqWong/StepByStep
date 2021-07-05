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
            Debug.Log("CustomInitialization :" + dataModel.FeedbackType);
        }

        protected override void CustomPlayCompeteCallback(FeedbackDataModelBase dataModel)
        {
            IsComplete = true;
            PlayComplete?.Invoke(this, EventArgs.Empty);
        }

        protected override void CustomExecuteHandler(FeedbackDataModelBase dataModel)
        {
            Debug.Log("CustomExecuteHandler :" + dataModel.FeedbackType);
        }

        public void OnClickAnimComplete()
        {
            CustomPlayCompeteCallback(FeedbackDataModel);
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
