using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xr.StepByStep.DataModel;
using xr.StepByStepFramework.Feedback_old;

namespace Assets.Example
{
    public class FeedContentAnim : FeedbackItemHandlerContentBase
    {

        public FeedContentAnim(GameObject owner, FeedbackDataModelBase dataModel) : base(owner, dataModel)
        {

        }

        protected override void CustomInitialization(GameObject owner, FeedbackDataModelBase dataModel)
        {
            Debug.Log("111111111111111111111111111");
        }
    }

}
