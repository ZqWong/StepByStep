using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace xr.StepByStepFramework.FeedbackModule
{
    public class Feedbacks : MonoBehaviour
    {
        /// <summary>
        /// 反馈类型列表
        /// </summary>
        public List<Feedback> FeedbackCollection = new List<Feedback>();

        public enum Directions { TopToBottom, BottomToTop }
        /// <summary>
        /// 播放顺序
        /// </summary>
        public Directions Direction = Directions.TopToBottom;

        /// <summary>
        /// 当所有的反馈都播放完后，这个 MMFeedbacks 是否应该反转它的方向
        /// </summary>
        public bool AutoChangeDirectionOnEnd = false;

        public enum InitializationModes { Script, Awake, Start }
        /// <summary>
        /// 初始化模式
        /// </summary>
        public InitializationModes InitializationMode = InitializationModes.Start;

        /// <summary>
        /// 开始时自动播放
        /// </summary>
        public bool AutoPlayOnStart = false;

        /// <summary>
        /// 激活时自动播放
        /// </summary>
        public bool AutoPlayOnEnable = false;

        /// <summary>
        /// 所有反馈持续时间的时间流速系数（初始延迟、持续时间、重复之间的延迟......默认1）
        /// </summary>
        public float DurationMultiplier = 1f;

        /// <summary>
        /// 编辑器模式显示更详细信息
        /// </summary>
        public bool DisplayFullDurationDetails = false;

        /// <summary>
        /// 冷却时间
        /// </summary>
        public float CooldownDuration = 0f;

        /// <summary>
        /// 延迟时间
        /// </summary>
        public float InitialDelay = 0f;

        /// <summary>
        /// 播放此反馈的强度。大多数反馈将使用该值来调整其幅度。 1为正常，0.5为半功率，0为无效。
        /// 请注意，此值控制的内容取决于反馈之间的反馈，请检查代码以了解它的确切作用。
        /// </summary>
        public float FeedbacksIntensity = 1f;

        /// <summary>
        /// 各个阶段触发的事件
        /// </summary>
        public FeedbacksEvents Events;

        /// <summary>
        /// 一个全局开关，用于全局打开或关闭所有反馈
        /// </summary>
        public static bool GlobalFeedbacksActive = true;

        /// <summary>
        /// 当前反馈是否在播放中（理解为还没停止）
        /// 如果你不停止你的 Feedbacks 它当然会保持为true
        /// </summary>
        public bool IsPlaying { get; protected set; }

        /// <summary>
        /// 代码动态阻塞 Feedbacks 序列并等待 Resume() 调用
        /// </summary>
        public bool InScriptDrivenPause { get; set; }

        /// <summary>
        /// 如果此 Feedbacks 包含至少一个循环，则为 true
        /// </summary>
        public bool ContainsLoop { get; set; }

        /// <summary>
        ///下次播放改变这个 Feedbacks 的方向 (pingpong)，单次有效
        /// </summary>
        public bool ShouldRevertOnNextPlay { get; set; }

        /// <summary>
        /// 此 Feedbacks 中所有活动反馈的总持续时间（以秒为单位）
        /// </summary>
        public float TotalDuration
        {
            get
            {
                float total = 0f;
                foreach (Feedback feedback in FeedbackCollection)
                {
                    if ((feedback != null) && (feedback.Active))
                    {
                        total += feedback.TotalDuration;
                    }
                }
                return total;
            }
        }

        /// <summary>
        /// 开始时间戳
        /// </summary>
        protected float m_startTime = 0f;
        /// <summary>
        /// 最长等待时间
        /// </summary>
        protected float m_holdingMax = 0f;
        /// <summary>
        /// 上次播放时间戳
        /// </summary>
        protected float m_lastStartAt = 0f;
        /// <summary>
        /// 初始延迟
        /// </summary>
        protected WaitForSeconds m_initialDelayWFS;

        /// <summary>
        /// Applies this feedback's time multiplier to a duration (in seconds)
        /// 基于用户设定的时间流速返回此反馈的实际用时（以秒为单位）
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public virtual float ApplyTimeMultiplier(float duration)
        {
            return duration * DurationMultiplier;
        }

        /// <summary>        
        /// 如果处于自动模式（Awake），会在程序启动时自动进行初始化
        /// </summary>
        protected virtual void Awake()
        {
            if ((InitializationMode == InitializationModes.Awake) && (Application.isPlaying))
            {
                Initialization(this.gameObject);
            }
        }

        /// <summary>
        /// 如果处于自动模式（Start），会在程序启动时自动进行初始化
        /// </summary>
        protected virtual void Start()
        {
            if ((InitializationMode == InitializationModes.Start) && (Application.isPlaying))
            {
                Initialization(this.gameObject);
            }
            if (AutoPlayOnStart && Application.isPlaying)
            {
                PlayFeedbacks();
            }
        }

        /// <summary>        
        /// 如果处于自动模式，会在程序启动时自动进行初始化
        /// </summary>
        protected virtual void OnEnable()
        {
            if (AutoPlayOnEnable && Application.isPlaying)
            {
                PlayFeedbacks();
            }
        }

        /// <summary>        
        /// 在销毁时，从该 MMFeedbacks 中删除所有反馈以避免任何多余的数据存在
        /// </summary>
        protected virtual void OnDestroy()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// 初始化当前MMFeedbacks，并设置所有者
        /// </summary>
        public virtual void Initialization()
        {
            Initialization(this.gameObject);
        }

        /// <summary>
        /// 公开的初始化函数，指定一个所有者，该所有者将被反馈用作位置和层次结构的参考
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="feedbacksOwner"></param>
        public virtual void Initialization(GameObject owner)
        {
            //if ((SafeMode == MMFeedbacks.SafeModes.RuntimeOnly) || (SafeMode == MMFeedbacks.SafeModes.Full))
            //{
                AutoRepair();
            //}

            IsPlaying = false;

            m_initialDelayWFS = new WaitForSeconds(InitialDelay);

            // 反馈初始化
            for (int i = 0; i < FeedbackCollection.Count; i++)
            {
                if (FeedbackCollection[i] != null)
                {
                    FeedbackCollection[i].Initialization(owner);
                }
            }
        }

        /// <summary>
        /// 使用 MMFeedbacks 的位置作为参考播放所有反馈，并且没有衰减
        /// </summary>
        public virtual void PlayFeedbacks()
        {
            StartCoroutine(PlayFeedbacksInternal(this.transform.position, FeedbacksIntensity));
        }

        /// <summary>
        /// 用于播放反馈的内部方法，不应在外部调用
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual IEnumerator PlayFeedbacksInternal(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
        {
            // 检查下次播放改变播放方向
            if (ShouldRevertOnNextPlay)
            {
                Revert();
                ShouldRevertOnNextPlay = false;
            }

            // 强制定义播放方向
            if (forceRevert)
            {
                Direction = (Direction == Directions.BottomToTop) ? Directions.TopToBottom : Directions.BottomToTop;
            }

            // 检查冷却
            if (CooldownDuration > 0f)
            {
                if (Time.unscaledTime - m_lastStartAt < CooldownDuration)
                {
                    yield break;
                }
            }

            // 启动延迟
            if (InitialDelay > 0f)
            {
                yield return m_initialDelayWFS;
            }

            // 如果是静默状态那么停止
            if (!this.isActiveAndEnabled)
            {
                yield break;
            }

            // 如果全局设置禁用那么停止
            if (!GlobalFeedbacksActive)
            {
                yield break;
            }

            Events.TriggerOnPlay(this);

            m_startTime = Time.unscaledTime;
            m_holdingMax = 0f;
            m_lastStartAt = m_startTime;

            ResetFeedbacks();

            // 检查是否有暂停相关弄能在feedbacks中
            bool pauseFound = false;
            for (int i = 0; i < FeedbackCollection.Count; i++)
            {
                if ((FeedbackCollection[i].Pause != null) && (FeedbackCollection[i].Active))
                {
                    pauseFound = true;
                }
                if ((FeedbackCollection[i].HoldingPause == true) && (FeedbackCollection[i].Active))
                {
                    pauseFound = true;
                }
            }

            if (!pauseFound)
            {
                // 如果没有暂停相关功能那么一次性全部放完事
                IsPlaying = true;
                for (int i = 0; i < FeedbackCollection.Count; i++)
                {
                    if (FeedbackCanPlay(FeedbackCollection[i]))
                    {
                        FeedbackCollection[i].Play(position, feedbacksIntensity);
                    }
                }

                Events.TriggerOnComplete(this);
                ApplyAutoRevert();
            }
            else
            {
                // 发现了暂停功能那么用协程等
                StartCoroutine(PausedFeedbacksCo(position, feedbacksIntensity));
            }
        }

        /// <summary>
        /// 用于暂停反馈的协程
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator PausedFeedbacksCo(Vector3 position, float feedbacksIntensity)
        {
            IsPlaying = true;

            int i = (Direction == Directions.TopToBottom) ? 0 : FeedbackCollection.Count - 1;

            while ((i >= 0) && (i < FeedbackCollection.Count))
            {
                if (!IsPlaying)
                {
                    yield break;
                }

                // 检测是否是动态代码暂停
                if (((FeedbackCollection[i].Active) && (FeedbackCollection[i].ScriptDrivenPause)) || InScriptDrivenPause)
                {
                    InScriptDrivenPause = true;
                    //等代码调用 ResumeFeedbacks();
                    while (InScriptDrivenPause)
                    {
                        yield return null;
                    }
                }

                // 等待其他feedback
                if ((FeedbackCollection[i].Active)
                    && (FeedbackCollection[i].HoldingPause == true))
                {
                    Events.TriggerOnPause(this);
                    // 在这里带其他feedback全部完毕后放行
                    while (Time.unscaledTime - m_lastStartAt < m_holdingMax)
                    {
                        yield return null;
                    }
                    m_holdingMax = 0f;
                    m_lastStartAt = Time.unscaledTime;
                }

                // 播放
                if (FeedbackCanPlay(FeedbackCollection[i]))
                {
                    FeedbackCollection[i].Play(position, feedbacksIntensity);
                }

                // 判断等待协程
                if ((FeedbackCollection[i].Pause != null) && (FeedbackCollection[i].Active))
                {
                    bool shouldPause = true;
                    if (FeedbackCollection[i].Chance < 100)
                    {
                        float random = Random.Range(0f, 100f);
                        if (random > FeedbackCollection[i].Chance)
                        {
                            shouldPause = false;
                        }
                    }

                    if (shouldPause)
                    {
                        yield return FeedbackCollection[i].Pause;
                        Events.TriggerOnResume(this);
                        m_lastStartAt = Time.unscaledTime;
                        m_holdingMax = 0f;
                    }
                }

                // 更新最长等待时间
                if (FeedbackCollection[i].Active)
                {
                    if (FeedbackCollection[i].Pause == null)
                    {
                        float feedbackDuration = FeedbackCollection[i].TotalDuration;
                        m_holdingMax = Mathf.Max(feedbackDuration, m_holdingMax);
                    }
                }

                // 更新播放方向
                i += (Direction == Directions.TopToBottom) ? 1 : -1;
            }
            Events.TriggerOnComplete(this);
            ApplyAutoRevert();
        }


        /// <summary>
        /// 如果为当前feedback定义了重制逻辑则每次播放后都会调用，
        /// 例如：重置闪烁渲染器的初始颜色。
        /// </summary>
        public virtual void ResetFeedbacks()
        {
            for (int i = 0; i < FeedbackCollection.Count; i++)
            {
                if ((FeedbackCollection[i] != null) && (FeedbackCollection[i].Active))
                {
                    FeedbackCollection[i].ResetFeedback();
                }
            }
            IsPlaying = false;
        }

        /// <summary>
        /// 如果指定反馈的 Timing 部分中定义的条件允许它在此 Feedbacks 的当前播放方向上播放，这将返回 true
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        protected bool FeedbackCanPlay(Feedback feedback)
        {
            if (feedback.Timing.MMFeedbacksDirectionCondition == FeedbackTiming.FeedbacksDirectionConditions.Always)
            {
                return true;
            }
            else if (((Direction == Directions.TopToBottom) && (feedback.Timing.MMFeedbacksDirectionCondition == FeedbackTiming.FeedbacksDirectionConditions.OnlyWhenForwards))
                || ((Direction == Directions.BottomToTop) && (feedback.Timing.MMFeedbacksDirectionCondition == FeedbackTiming.FeedbacksDirectionConditions.OnlyWhenBackwards)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 准备 Feedbacks 在下一次播放时反转方向
        /// </summary>
        protected virtual void ApplyAutoRevert()
        {
            if (AutoChangeDirectionOnEnd)
            {
                ShouldRevertOnNextPlay = true;
            }
        }

        /// <summary>
        /// 改变这个 Feedbacks 的方向
        /// </summary>
        public virtual void Revert()
        {
            Events.TriggerOnRevert(this);
            Direction = (Direction == Directions.BottomToTop) ? Directions.TopToBottom : Directions.BottomToTop;
        }

        /// <summary>
        /// 暂停序列的执行，然后可以通过调用 ResumeFeedbacks() 恢复执行
        /// </summary>
        public virtual void PauseFeedbacks()
        {
            Events.TriggerOnPause(this);
            InScriptDrivenPause = true;
        }

        /// <summary>
        /// 如果脚本驱动的暂停正在进行，则恢复序列的执行
        /// </summary>
        public virtual void ResumeFeedbacks()
        {
            Events.TriggerOnResume(this);
            InScriptDrivenPause = false;
        }

        /// <summary>
        /// Unity 有时会出现序列化问题。
        /// 此方法通过修复可能发生的任何不良同步来解决该问题。
        /// 因为我们要保证播放顺序的正确（BTT or TTB）
        /// </summary>
        public virtual void AutoRepair()
        {
            List<Component> components = components = new List<Component>();
            components = this.gameObject.GetComponents<Component>().ToList();
            foreach (Component component in components)
            {
                if (component is Feedback)
                {
                    bool found = false;
                    for (int i = 0; i < FeedbackCollection.Count; i++)
                    {
                        if (FeedbackCollection[i] == (Feedback)component)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        FeedbackCollection.Add((Feedback)component);
                    }
                }
            }
        }
    }
}
