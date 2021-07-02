using System;
using LitJson;
using UnityEngine;
using xr.StepByStep.DataModel;

namespace xr.StepByStepFramework.Feedback_old
{
    public abstract class FeedbackItemHandlerContentBase
    {
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
        /// 当前反馈处理逻辑
        /// </summary>
        protected IFeedbackItemHandlerExecute m_handler;


        protected FeedbackItemHandlerContentBase() { }
        /// <summary>
        /// 构造函数设置初始类型, 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="dataModel"></param>
        protected FeedbackItemHandlerContentBase(GameObject owner, FeedbackDataModelBase dataModel)
        {
            FeedbackType = dataModel.FeedbackType;
            CustomInitialization(owner, dataModel);
        }

        protected abstract void CustomInitialization(GameObject owner, FeedbackDataModelBase dataModel);

        public virtual void Execute(JsonData data, EventHandler executeCompletedEventHandler)
        {
            executeCompletedEventHandler += (object sender, EventArgs args) =>
            {
                IsComplete = true; 
                PlayComplete?.Invoke(this, EventArgs.Empty);
                CustomPlayCompeteCallback();
            };

            m_handler?.Execute(data, executeCompletedEventHandler);
        }

        protected virtual void CustomPlayCompeteCallback() { }

        public virtual void Close()
        {
            m_handler?.Close();
        }

    }

}
