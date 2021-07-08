using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace xr.StepByStepFramework.Feedback_old
{

    /// the possible modes for the timescale
    public enum TimescaleModes { Scaled, Unscaled }

    [System.Serializable]
    public class FeedbackTiming
    {
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
    }
}
