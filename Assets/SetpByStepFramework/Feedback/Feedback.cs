using UnityEngine;
using System.Collections;

namespace xr.SetpByStepFramework.FeedbackModule
{
    public abstract class Feedback : MonoBehaviour
    {
        /// <summary>
        /// 是否生效
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// 此 Feedback 在 Inspector 界面显示的名字
        /// </summary>
        public string Label = "Feedback";

        /// <summary>
        /// 时间相关的配置
        /// </summary>
        public FeedbackTiming Timing;

        /// <summary>
        /// 反馈发生的几率，验证使用随机数（百分比：100：一直发生，0：从未发生，50：每两次调用发生一次，等等）
        /// </summary>
        [Range(0, 100)]
        public float Chance = 100f;

        /// <summary>
        /// 反馈的所有者，在调用 Initialization 方法时定义
        /// </summary>
        public GameObject Owner { get; set; }

        /// <summary>
        /// 如果您的反馈应该暂停反馈序列的执行，则将此设置为 true
        /// </summary>
        public virtual IEnumerator Pause { get { return null; } }

        /// <summary>
        /// 如果这是真的，这个反馈将等到所有以前的反馈都运行
        /// </summary>
        public virtual bool HoldingPause { get { return false; } }

        /// <summary>
        /// 如果这是真的，这个反馈将暂停并等待，直到在其父 MMFeedbacks 上调用 Resume() 以恢复执行
        /// </summary>
        public virtual bool ScriptDrivenPause { get; set; }

        /// <summary>
        /// 如果这是真的，这个反馈将等到所有以前的反馈都运行，然后再次运行所有以前的反馈
        /// </summary>
        public virtual bool LooperStart { get { return false; } }

        /// <summary>
        /// 如果此反馈此时处于冷却中（因此无法播放），则返回 true，否则返回 false
        /// </summary>
        public virtual bool InCooldown { get { return (Timing.CooldownDuration > 0f) && (FeedbackTime - m_lastPlayTimestamp < Timing.CooldownDuration); } }

        /// <summary>
        /// 基于所选时序设置的时间（Time.time / Time.unscaledTime）
        /// </summary>
        public float FeedbackTime
        {
            get
            {
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    return Time.time;
                }
                else
                {
                    return Time.unscaledTime;
                }
            }
        }

        /// <summary>
        /// 基于所选时序设置的增量时间（Time.deltaTime / Time.unscaledDeltaTime）
        /// </summary>
        public float FeedbackDeltaTime
        {
            get
            {
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    return Time.deltaTime;
                }
                else
                {
                    return Time.unscaledDeltaTime;
                }
            }
        }

        /// <summary>
        /// 反馈的感知持续时间，用于显示其进度条，意味着每个反馈都会被有意义的数据覆盖(如果需要获取准确的时间请重写此方法)
        /// </summary>
        public virtual float FeedbackDuration { get { return 0f; } set { } }

        /// <summary>
        /// 上次播放此反馈的时间戳
        /// </summary>
        public virtual float FeedbackStartedAt { get { return m_lastPlayTimestamp; } }

        /// <summary>
        /// 此反馈是否正在播放
        /// </summary>
        public virtual bool FeedbackPlaying { get { return ((FeedbackStartedAt > 0f) && (Time.time - FeedbackStartedAt < FeedbackDuration)); } }

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

        /// <summary>
        /// 是否初始化
        /// </summary>
        protected bool m_initializedm = false;
        /// <summary>
        /// 重复播放次数
        /// </summary>
        protected int m_playsLeft;
        /// <summary>
        /// 上次播放时间戳
        /// </summary>
        protected float m_lastPlayTimestamp = -1f;
        /// <summary>
        /// 当前Feedback对象
        /// </summary>
        protected Feedbacks m_hostMMFeedbacks;
        /// <summary>
        /// 当前Feedback是否为空
        /// </summary>
        protected bool m_isHostMMFeedbacksNotNull;
        /// <summary>
        /// 播放协程
        /// </summary>
        protected Coroutine m_playCoroutine;
        /// <summary>
        /// 无限循环播放协程
        /// </summary>
        protected Coroutine m_infinitePlayCoroutine;
        /// <summary>
        /// 顺序播放协程
        /// </summary>
        //protected Coroutine m_sequenceCoroutine;

        /// <summary>
        /// 有限循环播放协程
        /// </summary>
        protected Coroutine m_repeatedPlayCoroutine;

        protected void OnEnable()
        {
            m_hostMMFeedbacks = this.gameObject.GetComponent<Feedbacks>();
            m_isHostMMFeedbacksNotNull = m_hostMMFeedbacks != null;
        }

        /// <summary>
        /// 初始化反馈及其时序相关变量
        /// </summary>
        /// <param name="owner"></param>
        public virtual void Initialization(GameObject owner)
        {
            m_initializedm = true;
            Owner = owner;
            m_playsLeft = Timing.NumberOfRepeats + 1;
            m_hostMMFeedbacks = this.gameObject.GetComponent<Feedbacks>();

            // 设置初始延迟时间
            SetInitialDelay(Timing.InitialDelay);
            // 设置循环播放间隙时间
            SetDelayBetweenRepeats(Timing.DelayBetweenRepeats);
            //SetSequence(Timing.Sequence);

            //绑定自定义初始化
            CustomInitialization(owner);
        }

        /// <summary>        
        /// 播放反馈
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void Play(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            // 检查激活状态
            if (!Active)
            {
                return;
            }

            // 检查初始化
            if (!m_initializedm)
            {
                Debug.LogWarning("The " + this + " feedback is being played without having been initialized. Call Initialization() first.");
            }

            // 检查是否在冷却中
            if (InCooldown)
            {
                return;
            }

            // 检查延迟播放
            if (Timing.InitialDelay > 0f)
            {
                m_playCoroutine = StartCoroutine(PlayCoroutine(position, feedbacksIntensity));
            }
            else
            {
                m_lastPlayTimestamp = FeedbackTime;
                RegularPlay(position, feedbacksIntensity);
            }
        }


        /// <summary>
        /// 延迟反馈的初始播放的内部协程
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator PlayCoroutine(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                yield return FeedbacksCoroutine.WaitFor(Timing.InitialDelay);
            }
            else
            {
                yield return FeedbacksCoroutine.WaitForUnscaled(Timing.InitialDelay);
            }
            m_lastPlayTimestamp = FeedbackTime;
            RegularPlay(position, feedbacksIntensity);
        }

        /// <summary>
        /// Triggers delaying coroutines if needed
        /// 正常播放
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void RegularPlay(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            // 分析触发几率
            if (Chance == 0f)
            {
                return;
            }
            if (Chance != 100f)
            {
                // determine the odds
                float random = Random.Range(0f, 100f);
                if (random > Chance)
                {
                    return;
                }
            }

            // 强度范围检测
            if (Timing.UseIntensityInterval)
            {
                if ((feedbacksIntensity < Timing.IntensityIntervalMin) || (feedbacksIntensity >= Timing.IntensityIntervalMax))
                {
                    return;
                }
            }

            // 是否开启无限循环
            if (Timing.RepeatForever)
            {
                m_infinitePlayCoroutine = StartCoroutine(InfinitePlay(position, feedbacksIntensity));
                return;
            }

            // 是否启用了有限循环播放
            if (Timing.NumberOfRepeats > 0)
            {
                m_repeatedPlayCoroutine = StartCoroutine(RepeatedPlay(position, feedbacksIntensity));
                return;
            }
            //if (Timing.Sequence == null)
            //{
                CustomPlayFeedback(position, feedbacksIntensity);
            //}
            //else
            //{
            //    m_sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));
            //}

        }

        /// <summary>
        /// Internal coroutine used for repeated play without end
        /// 内部协程用于无限循环播放
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator InfinitePlay(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            while (true)
            {
                m_lastPlayTimestamp = FeedbackTime;
                //if (Timing.Sequence == null)
                //{
                    CustomPlayFeedback(position, feedbacksIntensity);
                    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    {
                        yield return FeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
                    }
                    else
                    {
                        yield return FeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
                    }
                //}
                //else
                //{
                //    m_sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));

                //    float delay = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
                //    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                //    {
                //        yield return FeedbacksCoroutine.WaitFor(delay);
                //    }
                //    else
                //    {
                //        yield return FeedbacksCoroutine.WaitForUnscaled(delay);
                //    }
                //}
            }
        }

        /// <summary>
        /// Internal coroutine used for repeated play
        /// 用于有限循环播放的内部协程
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator RepeatedPlay(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            while (m_playsLeft > 0)
            {
                m_lastPlayTimestamp = FeedbackTime;
                m_playsLeft--;
                //if (Timing.Sequence == null)
                //{
                    CustomPlayFeedback(position, feedbacksIntensity);

                    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    {
                        yield return FeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
                    }
                    else
                    {
                        yield return FeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
                    }
                //}
                //else
                //{
                //    m_sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));

                //    float delay = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
                //    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                //    {
                //        yield return MMFeedbacksCoroutine.WaitFor(delay);
                //    }
                //    else
                //    {
                //        yield return MMFeedbacksCoroutine.WaitForUnscaled(delay);
                //    }
                //}
            }
            m_playsLeft = Timing.NumberOfRepeats + 1;
        }


        /// <summary>        
        /// 停止播放所有反馈
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void Stop(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (m_playCoroutine != null) { StopCoroutine(m_playCoroutine); }
            if (m_infinitePlayCoroutine != null) { StopCoroutine(m_infinitePlayCoroutine); }
            if (m_repeatedPlayCoroutine != null) { StopCoroutine(m_repeatedPlayCoroutine); }
            //if (m_sequenceCoroutine != null) { StopCoroutine(m_sequenceCoroutine); }

            m_lastPlayTimestamp = 0f;
            m_playsLeft = Timing.NumberOfRepeats + 1;
            CustomStopFeedback(position, feedbacksIntensity);
        }

        /// <summary>        
        /// 调用此方法设置有限重复播放次数
        /// </summary>
        public virtual void ResetFeedback()
        {
            m_playsLeft = Timing.NumberOfRepeats + 1;
            CustomReset();
        }

        /// <summary>
        /// 调用此方法设置播放延迟时间
        /// </summary>
        /// <param name="delay"></param>
        public virtual void SetInitialDelay(float delay)
        {
            Timing.InitialDelay = delay;
        }

        /// <summary>
        /// 调用此方法设置重复播放间隙延迟时间
        /// </summary>
        /// <param name="delay"></param>
        public virtual void SetDelayBetweenRepeats(float delay)
        {
            Timing.DelayBetweenRepeats = delay;
        }

        /// <summary>        
        /// 基于用户设定的时间流速获取时间
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected virtual float ApplyTimeMultiplier(float duration)
        {
            if (m_isHostMMFeedbacksNotNull)
            {
                return m_hostMMFeedbacks.ApplyTimeMultiplier(duration);
            }

            return duration;
        }

        /// <summary>
        /// 使用此方法在运行时更改此反馈的顺序
        /// </summary>
        /// <param name="newSequence"></param>
        //public virtual void SetSequence(Sequence newSequence)
        //{
        //    Timing.Sequence = newSequence;
        //    if (Timing.Sequence != null)
        //    {
        //        for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
        //        {
        //            if (Timing.Sequence.SequenceTracks[i].ID == Timing.TrackID)
        //            {
        //                _sequenceTrackID = i;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 除了主要的 Initialization 方法之外，此方法还描述了反馈所需的所有自定义初始化过程
        /// </summary>
        /// <param name="owner"></param>
        protected virtual void CustomInitialization(GameObject owner) { }

        /// <summary>        
        /// 这个方法描述了当反馈被播放时会发生什么
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected abstract void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f);

        /// <summary>       
        /// 这个方法描述了当反馈停止时会发生什么
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f) { }

        /// <summary>        
        /// 这个方法描述了当反馈被重置时会发生什么
        /// </summary>
        protected virtual void CustomReset() { }

    }
}