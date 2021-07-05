using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using xr.StepByStep.FSM;

namespace xr.StepByStepFramework.FSM
{
    public class FSMEventArgBase : EventArgs
    {
        public Action ExecuteCompleteCallBack;

        public FSMEventArgBase() { }

        public FSMEventArgBase(Action completeCallback)
        {
            ExecuteCompleteCallBack = completeCallback;
        }
    }

    public class FSMEventStateArg : FSMEventArgBase
    {
        public StepFSMManager.FSMState PreState;

        public StepFSMManager.FSMState CurrentState;

        public FSMEventStateArg(StepFSMManager.FSMState pre, StepFSMManager.FSMState current, Action callback) : base(callback)
        {
            PreState = pre;
            CurrentState = current;
        }
    }
}
