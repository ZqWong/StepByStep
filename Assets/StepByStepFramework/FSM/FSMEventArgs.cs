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
        /// <summary>
        /// 处理完成回调
        /// </summary>
        public Action ExecuteCompleteCallBack;

        public FSMEventArgBase() { }

        public FSMEventArgBase(Action completeCallback)
        {
            ExecuteCompleteCallBack = completeCallback;
        }
    }

    /// <summary>
    /// FSM 状态变更事件
    /// </summary>
    public class FSMEventStateArg : FSMEventArgBase
    {
        /// <summary>
        /// 变更前的状态
        /// </summary>
        public StepFSMManager.FSMState PreState;

        /// <summary>
        /// 变更后的装填
        /// </summary>
        public StepFSMManager.FSMState CurrentState;

        public FSMEventStateArg(StepFSMManager.FSMState pre, StepFSMManager.FSMState current, Action callback) : base(callback)
        {
            PreState = pre;
            CurrentState = current;
        }
    }
}
