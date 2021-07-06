using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xr.StepByStepFramework.Feedback_old
{
    public class FeedbackEventConst
    {
        public enum EventTypes { Play, Pause, Resume, Revert, Complete }

        /// <summary>
        /// FeedbackManager播放状态变更事件
        /// </summary>
        public const string FEEDBACK_STATUS_EVENT_KEY = "FEEDBACK_STATUS_EVENT";
    }
}
