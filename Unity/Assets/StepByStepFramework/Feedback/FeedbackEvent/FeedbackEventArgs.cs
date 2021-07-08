using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xr.StepByStepFramework.Feedback
{
    public class FeedbackEventArgs : EventArgs
    {
        public FeedbackEventConst.EventTypes StatusType;

        public FeedbackEventArgs(FeedbackEventConst.EventTypes statusType)
        {
            StatusType = statusType;
        }
    }
}
