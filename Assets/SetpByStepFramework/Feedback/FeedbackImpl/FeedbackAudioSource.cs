using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace xr.SetpByStepFramework.FeedbackModule
{
    public class FeedbackAudioSource : Feedback
    {
        public enum Modes { Play, Pause, UnPause, Stop }


        [Header("AudioSource")]
        [Tooltip("目标音频")]
        public AudioSource TargetAudioSource;
        [Tooltip("播放模式")]
        public Modes Mode = Modes.Play;

        [Header("随机音效")]        
        [Tooltip("随机音效池")]
        public AudioClip[] RandomSfx;

        [Header("音量设置")]
        [Tooltip("最小音量")]
        public float MinVolume = 1f;
        [Tooltip("最大音量")]
        public float MaxVolume = 1f;

        [Header("音调设置")]
        [Tooltip("最小音调")]
        public float MinPitch = 1f;
        [Tooltip("最大音调")]
        public float MaxPitch = 1f;

        [Header("混音器")]
        [Tooltip("混音器（可选）")]
        public AudioMixerGroup SfxAudioMixerGroup;

        protected AudioClip m_randomClip;
        protected float m_duration;

        public override float FeedbackDuration { get { return m_duration; } set { m_duration = value; } }

        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (Active)
            {
                float intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;
                switch (Mode)
                {
                    case Modes.Play:
                        if (RandomSfx.Length > 0)
                        {
                            m_randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];
                            TargetAudioSource.clip = m_randomClip;
                        }
                        float volume = Random.Range(MinVolume, MaxVolume) * intensityMultiplier;
                        float pitch = Random.Range(MinPitch, MaxPitch);
                        m_duration = TargetAudioSource.clip.length;
                        PlayAudioSource(TargetAudioSource, volume, pitch);
                        break;

                    case Modes.Pause:
                        m_duration = 0.1f;
                        TargetAudioSource.Pause();
                        break;

                    case Modes.UnPause:
                        m_duration = 0.1f;
                        TargetAudioSource.UnPause();
                        break;

                    case Modes.Stop:
                        m_duration = 0.1f;
                        TargetAudioSource.Stop();
                        break;
                }
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        protected virtual void PlayAudioSource(AudioSource audioSource, float volume, float pitch)
        {
            // we set the audio source volume to the one in parameters
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.timeSamples = 0;
            // we start playing the sound
            audioSource.Play();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public override void Stop(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            base.Stop(position, feedbacksIntensity);
            if (TargetAudioSource != null)
            {
                TargetAudioSource?.Stop();
            }
        }
    }
}