using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using UnityEngine;
using xr.StepByStep.DataModel;

namespace xr.StepByStepFramework.Feedback_old
{
    public class FeedbacksBase : MonoBehaviour
    {
        public EventHandler PlayComplete;
        public FeedbackTypeEnum FeedbackType { get; set; }
        public JsonData Data { get; set; }
        public bool IsComplete { get; set; }

        private FeedbackItemHandlerContent feedbackItemHandler;

        public FeedbacksBase(FeedbackDataModelBase stepTypeDataModel)
        {

        }

        public void Play()
        {
            if (feedbackItemHandler != null)
            {
                feedbackItemHandler.Excute(Data, (object sender, EventArgs args) =>
                {
                    IsComplete = true;
                    if (PlayComplete != null)
                    {
                        PlayComplete.Invoke(this, EventArgs.Empty);
                    }
                });
            }
        }

        public void Close()
        {
            if (feedbackItemHandler != null)
            {
                feedbackItemHandler.Close();
            }
        }
    }
}
