using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace xr.StepByStepFramework.Feedback_old
{
    public interface IFeedbackItemHandlerFactory
    {
        FeedbackItemHandlerContentBase GetHandlerByStepType(JsonData jsonData, string stepType);
    }
}
