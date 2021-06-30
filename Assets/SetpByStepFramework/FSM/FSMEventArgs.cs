using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace xr.SetpByStepFramework.FSM
{
    public class FSMEventArgBase : EventArgs
    {
        private Action ExecuteCompleteCallBack;

        public FSMEventArgBase() { }

        public FSMEventArgBase(Action completeCallback)
        {
            ExecuteCompleteCallBack = completeCallback;
        }
    }

    public class FSMEventFeedbackArg : FSMEventArgBase
    {
        public JsonData FeedbackJsonData;

        public FSMEventFeedbackArg(JsonData jsonData, Action callback = null) : base(callback)
        {
            FeedbackJsonData = jsonData;
            callback?.Invoke();
        }
    }
}
