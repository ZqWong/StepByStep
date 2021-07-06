using System;
using LitJson;
using UnityEngine;
using xr.StepByStepFramework.DataModel;

namespace xr.StepByStepFramework.Feedback_old
{
    public abstract class FeedbackItemHandlerContentBase : MonoBehaviour
    {
        /// <summary>
        /// 是否生效
        /// </summary>
        public bool Active = true;
        /// <summary>
        /// 时间设置
        /// </summary>
        public FeedbackTiming Timing;
        /// <summary>
        /// 反馈标识
        /// </summary>
        public string FeedbackIdentify { get; private set; }
        /// <summary>
        /// 完成回调
        /// </summary>
        public EventHandler PlayComplete;
        /// <summary>
        /// 当前反馈类型
        /// </summary>
        public string FeedbackType { get; set; }
        /// <summary>
        /// 当前反馈是否完成
        /// </summary>
        public bool IsComplete { get; set; }

        ///// <summary>
        ///// 当前反馈处理逻辑
        ///// </summary>
        //protected IFeedbackItemHandlerExecute m_handler;

        /// <summary>
        /// 持续时间，用于显示其进度条，意味着每个反馈都会被有意义的数据覆盖(如果需要获取准确的时间请重写此方法)
        /// </summary>
        public virtual float FeedbackDuration { get { return 0f; } set { } }

        protected FeedbackDataModelBase FeedbackDataModel { get; private set; }

        /// <summary>
        /// The total duration of this feedback :
        /// total = initial delay + duration * (number of repeats + delay between repeats)
        /// 此反馈的总持续时间：
        /// 总计 = 初始延迟 + 持续时间 *（重复次数 + 重复之间的延迟）
        /// </summary>
        public float TotalDuration
        {
            get
            {
                float totalTime = 0f;

                if (Timing == null)
                {
                    return 0f;
                }

                if (Timing.InitialDelay != 0)
                {
                    totalTime += ApplyTimeMultiplier(Timing.InitialDelay);
                }

                totalTime += FeedbackDuration;

                if (Timing.NumberOfRepeats != 0)
                {
                    float delayBetweenRepeats = ApplyTimeMultiplier(Timing.DelayBetweenRepeats);

                    totalTime += Timing.NumberOfRepeats * (FeedbackDuration + delayBetweenRepeats);
                }

                return totalTime;
            }
        }

        public virtual void Initialize() { }

        /// <summary>
        /// 构造函数设置初始类型, 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="dataModel"></param>
        public virtual void Initialize (FeedbackDataModelBase dataModel)
        {
            FeedbackType = dataModel.FeedbackType;
            FeedbackDataModel = dataModel;
            CustomInitialization(FeedbackDataModel);
        }

        protected abstract void CustomInitialization(FeedbackDataModelBase dataModel);

        public virtual void Execute(EventHandler executeCompletedEventHandler)
        {
            PlayComplete = executeCompletedEventHandler;
            CustomExecuteHandler(FeedbackDataModel);
        }

        protected virtual void CustomPlayCompeteCallback()
        {
            IsComplete = true;
            PlayComplete?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void CustomExecuteHandler(FeedbackDataModelBase dataModel);

        #region Helper
        private float ApplyTimeMultiplier(float duration)
        {
            return duration * FeedbackManager.Instance.DurationMultiplier;
        }

        #endregion
    }
}