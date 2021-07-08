using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace xr.StepByStepFramework.Feedback
{
    public interface IFeedbackItemHandlerFactory
    {
        /// <summary>
        /// 以工厂模式完善逻辑
        /// </summary>
        /// <param name="jsonData">JsonData</param>
        /// <param name="feedbackType">数据中feedback类型</param>
        /// <returns></returns>
        FeedbackItemHandlerContentBase GetHandlerByStepType(JsonData jsonData, string feedbackType);
    }
}
