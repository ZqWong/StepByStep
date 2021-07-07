using System.Collections;
using UnityEngine;

namespace xr.StepByStepFramework.Feedback_old
{
    public class FeedbacksCoroutine
    {
        /// <summary>
        /// 等待指定的帧数
        /// use : yield return MMCoroutine.WaitFor(1);
        /// </summary>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public static IEnumerator WaitForFrames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }

        /// <summary>
        /// 等待指定的秒数（使用Time.deltaTime）
        /// use : yield return MMCoroutine.WaitFor(1f);
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IEnumerator WaitFor(float seconds)
        {
            for (float timer = 0f; timer < seconds; timer += Time.deltaTime)
            {
                yield return null;
            }
        }

        /// <summary>
        /// 等待指定的秒数（使用Time.unscaledDeltaTime，不受Time.timeScale = 0影响）
        /// use : yield return MMCoroutine.WaitForUnscaled(1f);
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IEnumerator WaitForUnscaled(float seconds)
        {
            for (float timer = 0f; timer < seconds; timer += Time.unscaledDeltaTime)
            {
                yield return null;
            }
        }
    }
}
