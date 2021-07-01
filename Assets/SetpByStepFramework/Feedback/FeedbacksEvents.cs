using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace xr.SetpByStepFramework.FeedbackModule
{    
    public struct FeedbacksEvent
    {
        public enum EventTypes { Play, Pause, Resume, Revert, Complete }

        public delegate void Delegate(Feedbacks source, EventTypes type);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(Feedbacks source, EventTypes type)
        {
            OnEvent?.Invoke(source, type);
        }
    }

    [System.Serializable]
    public class FeedbacksEvents
    {
        /// <summary>
        /// 是否需要触发事件
        /// </summary>
        public bool TriggerFeedbacksEvents = false;
        /// <summary>
        /// 是否触发UnityEvent事件
        /// </summary>
        public bool TriggerUnityEvents = true;

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

        public virtual void TriggerOnPlay(Feedbacks source)
        {
            if (!OnPlayIsNull && TriggerUnityEvents)
            {
                OnPlay.Invoke();
            }

            if (TriggerFeedbacksEvents)
            {
                FeedbacksEvent.Trigger(source, FeedbacksEvent.EventTypes.Play);
            }
        }

        public virtual void TriggerOnPause(Feedbacks source)
        {
            if (!OnPauseIsNull && TriggerUnityEvents)
            {
                OnPause.Invoke();
            }

            if (TriggerFeedbacksEvents)
            {
                FeedbacksEvent.Trigger(source, FeedbacksEvent.EventTypes.Pause);
            }
        }

        public virtual void TriggerOnResume(Feedbacks source)
        {
            if (!OnResumeIsNull && TriggerUnityEvents)
            {
                OnResume.Invoke();
            }

            if (TriggerFeedbacksEvents)
            {
                FeedbacksEvent.Trigger(source, FeedbacksEvent.EventTypes.Resume);
            }
        }

        public virtual void TriggerOnRevert(Feedbacks source)
        {
            if (!OnRevertIsNull && TriggerUnityEvents)
            {
                OnRevert.Invoke();
            }

            if (TriggerFeedbacksEvents)
            {
                FeedbacksEvent.Trigger(source, FeedbacksEvent.EventTypes.Revert);
            }
        }

        public virtual void TriggerOnComplete(Feedbacks source)
        {
            if (!OnCompleteIsNull && TriggerUnityEvents)
            {
                OnComplete.Invoke();
            }

            if (TriggerFeedbacksEvents)
            {
                FeedbacksEvent.Trigger(source, FeedbacksEvent.EventTypes.Complete);
            }
        }
    }
}