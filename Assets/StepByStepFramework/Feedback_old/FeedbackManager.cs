using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using etp.xr.Tools;
using LitJson;
using UnityEngine;

namespace xr.StepByStepFramework.Feedback_old
{
    public class FeedbackManager : SingletonMonoBehaviourClass<FeedbackManager>
    {
        /// <summary>
        /// 反馈列表
        /// </summary>
        public List<FeedbackItemHandlerContentBase> FeedbackCollection = new List<FeedbackItemHandlerContentBase>();

        public enum Directions { TopToBottom, BottomToTop }
        /// <summary>
        /// 执行顺序方向
        /// </summary>
        public Directions Direction = Directions.TopToBottom;

        public enum InitializationModes { Script, Awake, Start }
        /// <summary>
        /// 初始化模式
        /// </summary>
        public InitializationModes InitializationMode = InitializationModes.Start;

        /// <summary>
        /// 所有反馈持续时间的时间流速系数（初始延迟、持续时间、重复之间的延迟......默认1）
        /// </summary>
        public float DurationMultiplier = 1f;

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
        public FeedbackEvents Events;

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
        /// 此 Feedbacks 中所有活动反馈的总持续时间（以秒为单位），每一个feedback赋予了正确的时间长度是前提
        /// </summary>
        public float TotalDuration
        {
            get
            {
                float total = 0f;
                foreach (FeedbackItemHandlerContentBase feedback in FeedbackCollection)
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
        /// 初始延迟
        /// </summary>
        protected WaitForSeconds m_initialDelayWFS;

        public void Initialize()
        {
            IsPlaying = false;
            FeedbackCollection.Clear();
        }

        /// <summary>
        /// 工厂模式自定义类型处理类初始化, string = 反馈类型key
        /// </summary>
        public Func<JsonData, string, FeedbackItemHandlerContentBase> FeedbackFactoryInitialize;

        /// <summary>
        /// 初始化步骤反馈信息，先对 FeedbackFactoryInitialize 进行处理
        /// </summary>
        /// <param name="jsonData">JsonData</param>
        /// <param name="feedbacksKey">数据中反馈组的头字段</param>
        /// <param name="feedbackTypeKey">数据中反馈类型的头字段</param>
        public void InitializeWithNewFeedback(JsonData jsonData, string feedbacksKey = "feedbacks", string feedbackTypeKey = "feedbackType")
        {
            IsPlaying = false;

            // 获取Feedback全局的一些设置，如果有的话
            DurationMultiplier = jsonData.ContainsKey("durationMultiplier")
                ? jsonData["durationMultiplier"].ToFloat()
                : 1f;
            CooldownDuration = jsonData.ContainsKey("cooldownDuration")
                ? jsonData["cooldownDuration"].ToFloat()
                : 0f;
            InitialDelay = jsonData.ContainsKey("initialDelay")
                ? jsonData["initialDelay"].ToFloat()
                : 0f;
            FeedbacksIntensity = jsonData.ContainsKey("feedbacksIntensity")
                ? jsonData["feedbacksIntensity"].ToFloat()
                : 1f;

            FeedbackCollection.Clear();

            // 将原来的feedback删除
            if (FeedbackCollection.Count > 0)
            {
                foreach (FeedbackItemHandlerContentBase component in FeedbackCollection)
                {
                    Destroy(component);
                }
            }

            // FeedbackFactoryInitialize 已经初始化完毕，对用户自定义的处理进行工厂处理，并添加到FeedbackCollection中
            foreach (JsonData data in jsonData[feedbacksKey])
            {
                var feedbackComponent = FeedbackFactoryInitialize?.Invoke(data, feedbackTypeKey);
                Debug.Assert(null != feedbackComponent, "Feedback Factory return failed");
                FeedbackCollection.Add(feedbackComponent);
            }
        }

        /// <summary>
        /// 播放反馈
        /// </summary>
        public void  PlayFeedback(Action callback)
        {
            Events.TriggerOnPlay();
            foreach (var feedback in FeedbackCollection)
            {
                feedback.Execute(((sender, args) =>
                {
                    if(FeedbackCollection.All(f =>{return f.IsComplete = true;}))
                    {
                        Debug.Log("All Complete");
                        Events.TriggerOnComplete();
                        callback?.Invoke();
                    }
                }));
            }
        }
    }
}
