using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etp.xr.Managers;
using etp.xr.Tools;
using UnityEngine.Events;

namespace xr.StepByStepFramework.Feedback
{
    [System.Serializable]
    public class FeedbackEvents
    {
        /// <summary>
        /// 是否需要触发事件
        /// </summary>
        public bool TriggerFeedbackEvents = true;
        /// <summary>
        /// 是否触发UnityEvent事件
        /// </summary>
        public bool TriggerUnityEvents = false;

        public UnityEvent OnPlay;
        public UnityEvent OnPause;
        public UnityEvent OnResume;
        public UnityEvent OnRevert;
        public UnityEvent OnComplete;

        public bool OnPlayIsNull { get; protected set; }
        public bool OnPauseIsNull { get; protected set; }
        public bool OnResumeIsNull { get; protected set; }
        public bool OnRevertIsNull { get; protected set; }
        public bool OnCompleteIsNull { get; protected set; }

        /// <summary>
        /// 在初始化时判断事件是否有必要调用
        /// </summary>
        public virtual void Initialization()
        {
            OnPlayIsNull = OnPlay == null;
            OnPauseIsNull = OnPause == null;
            OnResumeIsNull = OnResume == null;
            OnRevertIsNull = OnRevert == null;
            OnCompleteIsNull = OnComplete == null;
        }

        public virtual void TriggerOnPlay()
        {
            OnPlayIsNull = OnPlay == null;

            if (!OnPlayIsNull && TriggerUnityEvents)
            {
                OnPlay.Invoke();
            }

            if (TriggerFeedbackEvents)
            {
                SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                    FeedbackEventConst.FEEDBACK_STATUS_EVENT_KEY, 
                    new FeedbackEventArgs(FeedbackEventConst.EventTypes.Play));
            }
        }

        public virtual void TriggerOnPause()
        {
            OnPauseIsNull = OnPause == null;

            if (!OnPauseIsNull && TriggerUnityEvents)
            {
                OnPause.Invoke();
            }

            if (TriggerFeedbackEvents)
            {
                SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                    FeedbackEventConst.FEEDBACK_STATUS_EVENT_KEY,
                    new FeedbackEventArgs(FeedbackEventConst.EventTypes.Pause));
            }
        }

        public virtual void TriggerOnResume()
        {
            OnResumeIsNull = OnResume == null;

            if (!OnResumeIsNull && TriggerUnityEvents)
            {
                OnResume.Invoke();
            }

            if (TriggerFeedbackEvents)
            {
                SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                    FeedbackEventConst.FEEDBACK_STATUS_EVENT_KEY,
                    new FeedbackEventArgs(FeedbackEventConst.EventTypes.Resume));
            }
        }

        public virtual void TriggerOnRevert()
        {
            OnRevertIsNull = OnRevert == null;

            if (!OnRevertIsNull && TriggerUnityEvents)
            {
                OnRevert.Invoke();
            }

            if (TriggerFeedbackEvents)
            {
                SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                    FeedbackEventConst.FEEDBACK_STATUS_EVENT_KEY,
                    new FeedbackEventArgs(FeedbackEventConst.EventTypes.Revert));
            }
        }

        public virtual void TriggerOnComplete()
        {
            OnCompleteIsNull = OnComplete == null;

            if (!OnCompleteIsNull && TriggerUnityEvents)
            {
                OnComplete.Invoke();
            }

            if (TriggerFeedbackEvents)
            {
                SingletonProvider<EventManager>.Instance.RaiseEventByEventKey(
                    FeedbackEventConst.FEEDBACK_STATUS_EVENT_KEY,
                    new FeedbackEventArgs(FeedbackEventConst.EventTypes.Complete));
            }
        }
    }
}
