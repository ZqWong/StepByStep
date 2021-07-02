using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xr.StepByStepFramework.Feedback_old
{
    public interface IFeedbackItemHandlerFactory
    {
        IFeedbackItemHandlerExecute GetHandlerByStepType(string stepType);
    }
}
