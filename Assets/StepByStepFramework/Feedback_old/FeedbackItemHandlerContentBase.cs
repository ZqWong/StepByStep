using System;
using System.Collections;
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
        /// 触发几率
        /// </summary>
        [Range(0, 100)]
        public int Chance = 100;
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
        /// <summary>
        /// 持续时间，用于显示其进度条，意味着每个反馈都会被有意义的数据覆盖(如果需要获取准确的时间请重写此方法)
        /// </summary>
        public virtual float FeedbackDuration { get { return 0f; } set { } }
        /// <summary>
        /// 反馈的基础数据模型
        /// </summary>
        protected FeedbackDataModelBase FeedbackDataModel { get; private set; }
        /// <summary>
        /// 无限循环播放协程
        /// </summary>
        protected Coroutine m_infinitePlayCoroutine;
        /// <summary>
        /// 有限循环播放协程
        /// </summary>
        protected Coroutine m_repeatedPlayCoroutine;
        /// <summary>
        /// 重复播放次数
        /// </summary>
        protected int m_playsLeft;
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

        /// <summary>
        /// 自定义初始化
        /// </summary>
        /// <param name="dataModel"></param>
        protected abstract void CustomInitialization(FeedbackDataModelBase dataModel);

        /// <summary>
        /// 反馈执行处理
        /// </summary>
        /// <param name="executeCompletedEventHandler"></param>
        public virtual void Execute(EventHandler executeCompletedEventHandler)
        {
            if (Timing.InitialDelay > 0f)
            {
                StartCoroutine(ExecuteCoroutine(executeCompletedEventHandler));
            }
            else
            {
                RegularPlay(FeedbackDataModel);
            }
        }

        private IEnumerator ExecuteCoroutine(EventHandler executeCompletedEventHandler)
        {
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                yield return FeedbacksCoroutine.WaitFor(Timing.InitialDelay);
            }
            else
            {
                yield return FeedbacksCoroutine.WaitForUnscaled(Timing.InitialDelay);
            }
            PlayComplete = executeCompletedEventHandler;
            //CustomExecuteHandler(FeedbackDataModel);
            RegularPlay(FeedbackDataModel);
        }

        private void RegularPlay(FeedbackDataModelBase dataModel)
        {
            // 分析触发几率
            if (Chance == 0f)
            {
                return;
            }
            if (Chance != 100f)
            {
                Chance = Mathf.Clamp(Chance, 0, 100);
                // determine the odds
                float random = UnityEngine.Random.Range(0f, 100f);
                if (random > Chance)
                {
                    return;
                }
            }

            // 是否开启无限循环
            if (Timing.RepeatForever)
            {
                m_infinitePlayCoroutine = StartCoroutine(InfinitePlay(dataModel));
                return;
            }

            // 是否启用了有限循环播放
            if (Timing.NumberOfRepeats > 0)
            {
                m_repeatedPlayCoroutine = StartCoroutine(RepeatedPlay(dataModel));
                return;
            }
        }

        private IEnumerator InfinitePlay(FeedbackDataModelBase dataModel)
        {
            while (true)
            {
                CustomExecuteHandler(dataModel);
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    yield return FeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
                }
                else
                {
                    yield return FeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
                }
            }
        }

        private IEnumerator RepeatedPlay(FeedbackDataModelBase dataModel)
        {
            while (m_playsLeft > 0)
            {
                m_playsLeft--;
                CustomExecuteHandler(dataModel);
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    yield return FeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
                }
                else
                {
                    yield return FeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
                }
            }
            m_playsLeft = Timing.NumberOfRepeats + 1;
        }

        /// <summary>
        /// 自定义执行反比毁掉
        /// </summary>
        protected virtual void CustomPlayCompeteCallback()
        {
            IsComplete = true;
            PlayComplete?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 自定义执行逻辑
        /// </summary>
        /// <param name="dataModel"></param>
        protected abstract void CustomExecuteHandler(FeedbackDataModelBase dataModel);

        #region Helper

        /// <summary>
        /// 根据反馈的时间流速获取具体长度
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private float ApplyTimeMultiplier(float duration)
        {
            return duration * FeedbackManager.Instance.DurationMultiplier;
        }

        #endregion
    }
}