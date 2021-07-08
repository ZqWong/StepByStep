using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xr.StepByStepFramework.FeedbackModule
{
    public class FeedbackAnimation : Feedback
    {
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            
        }
    }
}
