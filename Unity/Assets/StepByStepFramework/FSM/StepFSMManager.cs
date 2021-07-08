using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etp.xr.Managers;
using etp.xr.Tools;
using UnityEngine;
using xr.StepByStep.FSM;

namespace xr.StepByStepFramework.FSM
{
    public class StepFSMManager : SingletonMonoBehaviour<StepFSMManager>
    {
        /// <summary>
        /// FMS状态
        /// </summary>
        public enum FSMState
        {
            INITIAL,
            ENTER_STEP,
            EXECUTE_STEP,
            LEAVE_STEP
        }

        /// <summary>
        /// FMS事件
        /// </summary>
        private enum FSMEvent
        {
            START,
            ENTER_TO_EXECUTE,
            EXECUTE_TO_LEAVE,
            LEAVE_TO_ENTER
        }

        #region public variables

        /// <summary>
        /// FSM 当前状态
        /// </summary>
        public FSMState CurrentState => m_fsm.CurrentState;

        /// <summary>
        /// 是否启用Update
        /// </summary>
        public bool UseUpdate { get; set; }

        /// <summary>
        /// 活跃状态
        /// </summary>
        public bool IsActive { get; private set; }

        #endregion

        #region private variables

        private FSM<FSMState, FSMEvent> m_fsm = null;
        private bool m_initialized = false;

        #endregion

        void OnEnable()
        {
            // 开始监听步骤处理启动事件
            SingletonProvider<EventManager>.Instance.RegisterEvent(FSMEventConst.ENABLE_STEP_KEY, StartFSM);
        }

        void OnDisable()
        {
            // 结束监听步骤处理启动事件
            SingletonProvider<EventManager>.Instance.UnRegisterEventHandler(FSMEventConst.ENABLE_STEP_KEY, StartFSM);
        }
        
        /// <summary>
        /// 初始化FSM
        /// </summary>
        public void Start()
        {
            //ConfigureFSM();
            //StartFSM(this, null);
            //m_initialized = true;
        }

        public void Update()
        {
            if (UseUpdate)
            {
                UpdateFSM();
            }
        }

        /// <summary>
        /// 开始FSM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartFSM(object sender, EventArgs e)
        {
            m_initialized = true;
            ConfigureFSM();
            m_fsm.Start();
        }

        private void UpdateFSM()
        {
            m_fsm.Update();
        }

        /// <summary>
        /// 配置
        /// </summary>
        private void ConfigureFSM()
        {
            m_fsm = new FSM<FSMState, FSMEvent>();

            // setup initial state
            m_fsm.SetReaction(FSMState.INITIAL, FSMEvent.START, FSMState.ENTER_STEP);

            // setup ENTER_STEP state
            m_fsm.SetOnEnter(FSMState.ENTER_STEP, EnterStartStepHandler);
            if (UseUpdate)
            {
                m_fsm.SetOnUpdate(FSMState.ENTER_STEP, UpdateStartStepHandler);
            }
            m_fsm.SetOnExit(FSMState.ENTER_STEP, ExitStartStepHandler);

            m_fsm.SetReaction(FSMState.ENTER_STEP, FSMEvent.ENTER_TO_EXECUTE, FSMState.EXECUTE_STEP, null);

            // setup EXECUTE_STEP state
            m_fsm.SetOnEnter(FSMState.EXECUTE_STEP, EnterExecuteStepHandler);
            if (UseUpdate)
            {
                m_fsm.SetOnUpdate(FSMState.EXECUTE_STEP, UpdateExecuteStepHandler);
            }
            m_fsm.SetOnExit(FSMState.EXECUTE_STEP, ExitExecuteStepHandler);

            m_fsm.SetReaction(FSMState.EXECUTE_STEP, FSMEvent.EXECUTE_TO_LEAVE, FSMState.LEAVE_STEP, null);

            // setup LEAVE_STEP state
            m_fsm.SetOnEnter(FSMState.LEAVE_STEP, EnterLeaveStepHandler);
            if (UseUpdate)
            {
                m_fsm.SetOnUpdate(FSMState.LEAVE_STEP, UpdateLeaveStepHandler);
            }
            m_fsm.SetOnExit(FSMState.LEAVE_STEP, ExitLeaveStepHandler);

            m_fsm.SetReaction(FSMState.LEAVE_STEP, FSMEvent.LEAVE_TO_ENTER, FSMState.ENTER_STEP, null);
        }

        private void EnterStartStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.ENTER_START_STEP_KEY, 
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, () =>
                {
                    m_fsm.ReactTo(FSMEvent.ENTER_TO_EXECUTE);
                }));
        }
        private void UpdateStartStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.UPDATE_START_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, null));
        }
        private void ExitStartStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.LEAVE_START_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, null));
        }
        private void EnterExecuteStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.ENTER_EXECUTE_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, () =>
                {
                    m_fsm.ReactTo(FSMEvent.EXECUTE_TO_LEAVE);
                }));
        }
        private void UpdateExecuteStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.UPDATE_EXECUTE_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, null));
        }
        private void ExitExecuteStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.LEAVE_EXECUTE_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, null));
        }
        private void EnterLeaveStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.ENTER_END_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, () =>
                {
                    m_fsm.ReactTo(FSMEvent.LEAVE_TO_ENTER);
                }));
        }
        private void UpdateLeaveStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.UPDATE_END_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, null));
        }
        private void ExitLeaveStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.LEAVE_END_STEP_KEY,
                new FSMEventStateArg(m_fsm.PreviousState, m_fsm.CurrentState, null));
        }

        #region Debug

        public void OnClick1()
        {
            m_fsm.ReactTo(FSMEvent.ENTER_TO_EXECUTE);
        }

        public void OnClick2()
        {
            m_fsm.ReactTo(FSMEvent.EXECUTE_TO_LEAVE);
        }

        public void OnClick3()
        {
            m_fsm.ReactTo(FSMEvent.LEAVE_TO_ENTER);
        }

        #endregion
    }
}
