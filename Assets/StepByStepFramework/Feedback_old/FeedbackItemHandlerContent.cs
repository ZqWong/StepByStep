using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace xr.StepByStepFramework.Feedback_old
{
    public class FeedbackItemHandlerContent
    {
        private IFeedbackItemHandlerExecute m_handler;
        private FeedbackItemHandlerContent()
        {

        }

        public FeedbackItemHandlerContent(IFeedbackItemHandlerExecute handler)
        {
            m_handler = handler;
        }

        public void Excute(JsonData data, EventHandler excuteCompletedEventHandler)
        {
            if (m_handler != null)
            {
                m_handler.Execute(data, excuteCompletedEventHandler);
            }
        }

        public void Close()
        {
            if (m_handler is IFeedbackItemHandlerExecute)
            {
                m_handler.Close();
            }
        }

    }

}
