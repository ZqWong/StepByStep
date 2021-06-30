using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etp.xr.Managers;
using etp.xr.Tools;
using UnityEngine;
using xr.SetpByStep.FSM;

namespace xr.SetpByStepFramework.FSM
{
    public class StepFSMManager : SingletonMonoBehaviourClass<StepFSMManager>
    {
        private enum FSMState
        {
            INITIAL,
            ENTER_STEP,
            EXECUTE_STEP,
            LEAVE_STEP
        }

        private enum FSMEvent
        {
            START,
            ENTER_TO_EXECUTE,
            EXECUTE_TO_LEAVE,
            LEAVE_TO_ENTER
        }

        #region public variables
        /// <summary>
        /// 活跃状态
        /// </summary>
        public bool IsActive { get; private set; }

        public StepDataModel CurrentStepDataModel;

        #endregion

        #region private variables

        private FSM<FSMState, FSMEvent> m_fsm = null;
        private bool m_initialized = false;

        #endregion


        public void Start()
        {
            ConfigureFSM();
            StartFSM();
            m_initialized = true;
        }

        public void Update()
        {
            UpdateFSM();
        }

        private void StartFSM()
        {
            m_fsm.Start();
        }

        private void UpdateFSM()
        {
            m_fsm.Update();
        }

        private void ConfigureFSM()
        {
            m_fsm = new FSM<FSMState, FSMEvent>();

            // setup initial state
            m_fsm.SetReaction(FSMState.INITIAL, FSMEvent.START, FSMState.ENTER_STEP);

            // setup ENTER_STEP state
            m_fsm.SetOnEnter(FSMState.ENTER_STEP, EnterStartStepHandler);
            m_fsm.SetOnUpdate(FSMState.ENTER_STEP, UpdateStartStepHandler);
            m_fsm.SetOnExit(FSMState.ENTER_STEP, ExitStartStepHandler);

            m_fsm.SetReaction(FSMState.ENTER_STEP, FSMEvent.ENTER_TO_EXECUTE, FSMState.EXECUTE_STEP, null);

            // setup EXECUTE_STEP state
            m_fsm.SetOnEnter(FSMState.EXECUTE_STEP, EnterExecuteStepHandler);
            m_fsm.SetOnUpdate(FSMState.EXECUTE_STEP, UpdateExecuteStepHandler);
            m_fsm.SetOnExit(FSMState.EXECUTE_STEP, ExitExecuteStepHandler);

            m_fsm.SetReaction(FSMState.EXECUTE_STEP, FSMEvent.EXECUTE_TO_LEAVE, FSMState.LEAVE_STEP, null);

            // setup LEAVE_STEP state
            m_fsm.SetOnEnter(FSMState.LEAVE_STEP, EnterLeaveStepHandler);
            m_fsm.SetOnUpdate(FSMState.LEAVE_STEP, UpdateLeaveStepHandler);
            m_fsm.SetOnExit(FSMState.LEAVE_STEP, ExitLeaveStepHandler);

            m_fsm.SetReaction(FSMState.LEAVE_STEP, FSMEvent.LEAVE_TO_ENTER, FSMState.ENTER_STEP, null);
        }

        [SerializeField]
        private bool EnterStartStepExecuteComplete = false;
        private void EnterStartStepHandler()
        {
            bool feedbackExecuteComplete = false;
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                FSMEventConst.ENTER_START_STEP_KEY, 
                new FSMEventFeedbackArg(
                    //步骤不应该操作数据，应该是处理模块直接从数据模块拿
                    null,
                    () =>
                {
                    feedbackExecuteComplete = true;
                    EnterStartStepExecuteComplete = feedbackExecuteComplete;
                    if (EnterStartStepExecuteComplete)
                    {
                        m_fsm.ReactTo(FSMEvent.ENTER_TO_EXECUTE);
                    }
                }));
        }

        [SerializeField]
        private bool UpdateStartStepExecuteComplete = false;
        private void UpdateStartStepHandler()
        {
            bool feedbackExecuteComplete = false;
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                FSMEventConst.UPDATE_START_STEP_KEY,
                new FSMEventFeedbackArg(
                    null,
                    () =>
                {

                }));
        }

        [SerializeField]
        private bool ExitStartStepExecuteComplete = false;
        private void ExitStartStepHandler()
        {
            
        }
        private void EnterExecuteStepHandler()
        {
            bool feedbackExecuteComplete = false;
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                FSMEventConst.ENTER_EXECUTE_STEP_KEY,
                new FSMEventFeedbackArg(
                    null,
                    () =>
                {
                    feedbackExecuteComplete = true;
                    EnterStartStepExecuteComplete = feedbackExecuteComplete;
                    if (EnterStartStepExecuteComplete)
                    {
                        m_fsm.ReactTo(FSMEvent.EXECUTE_TO_LEAVE);
                    }
                }));
        }
        private void UpdateExecuteStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.UPDATE_EXECUTE_STEP_KEY, new FSMEventArgBase());
        }
        private void ExitExecuteStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.LEAVE_EXECUTE_STEP_KEY, new FSMEventArgBase());
        }
        private void EnterLeaveStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.ENTER_END_STEP_KEY, new FSMEventArgBase());
        }
        private void UpdateLeaveStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.UPDATE_END_STEP_KEY, new FSMEventArgBase());
        }
        private void ExitLeaveStepHandler()
        {
            SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(FSMEventConst.LEAVE_END_STEP_KEY, new FSMEventArgBase());

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
