using UnityEngine;
using System.Collections;

namespace xr.SetpByStepFramework.FeedbackModule
{
    /// the possible modes for the timescale
    public enum TimescaleModes { Scaled, Unscaled }

    [System.Serializable]
    public class FeedbackTiming
    {

        // 方向播放限制，控制只有当正或反序播放时才播放
        public enum FeedbacksDirectionConditions { Always, OnlyWhenForwards, OnlyWhenBackwards };
       
        //public enum PlayDirections { FollowMMFeedbacksDirection, OppositeMMFeedbacksDirection, AlwaysNormal, AlwaysRewind }

        [Header("Timescale")]       
        [Tooltip("使用 Time.deltaTime 或 Time.unscaledDeltaTime 模式, 区别在于是否受Time.timeScale影响")]
        public TimescaleModes TimescaleMode = TimescaleModes.Scaled;

        [Header("延迟播放参数")]
        
        [Tooltip("播放延迟时间（以秒为单位）")]
        public float InitialDelay = 0f;
        
        [Tooltip("播放的冷却时间")]
        public float CooldownDuration = 0f;

        [Header("重复播放参数")]
        
        [Tooltip("重复播放次数")]
        public int NumberOfRepeats = 0;
        
        [Tooltip("是否无限循环播放")]
        public bool RepeatForever = false;
        
        [Tooltip("重复播放间隙时间（以秒为单位）")]
        public float DelayBetweenRepeats = 1f;

        [Header("播放顺序")]
        [Tooltip("仅当播放顺序\"为自下而上（OnlyWhenBackwards\"或\"自上而下（OnlyWhenForwards）\"时，或不受限制（always）时才播放 :" +
                 "- always (default) : 不受顺序限制直接播放" +
                 "- OnlyWhenForwards : 只有当播放顺序为 自上而下 时才播放当前feedback" +
                 "- OnlyWhenBackwards : 只有当播放顺序为 自下而上 时才播放当前feedback")]
        public FeedbacksDirectionConditions MMFeedbacksDirectionCondition = FeedbacksDirectionConditions.Always;

        //[Tooltip("this defines the way this feedback will play. It can play in its normal direction, or in rewind (a sound will play backwards," +
        //         " an object normally scaling up will scale down, a curve will be evaluated from right to left, etc)" +
        //         "- BasedOnMMFeedbacksDirection : will play normally when the host MMFeedbacks is played forwards, in rewind when it's played backwards" +
        //         "- OppositeMMFeedbacksDirection : will play in rewind when the host MMFeedbacks is played forwards, and normally when played backwards" +
        //         "- Always Normal : will always play normally, regardless of the direction of the host MMFeedbacks" +
        //         "- Always Rewind : will always play in rewind, regardless of the direction of the host MMFeedbacks")]
        //public PlayDirections PlayDirection = PlayDirections.FollowMMFeedbacksDirection;

        [Header("强度(只是一个范类，可能不同的feedback所对应的意义不同)")]
        [Tooltip("是否受父级的强度因素影响 true=不受，fals=受")]
        public bool ConstantIntensity = false;

        [Tooltip("对强度进行范围检测，只有当强度值大于IntensityIntervalMin且小于IntensityIntervalMax才播放此feedback")]
        public bool UseIntensityInterval = false;
        
        [Tooltip("此反馈播放所需的最小强度")]       
        public float IntensityIntervalMin = 0f;
        
        [Tooltip("此反馈播放所需的最大强度")]
        public float IntensityIntervalMax = 0f;
    }
}