using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xr.StepByStepFramework.FSM
{
    public class FSMEventConst
    {
        /// <summary>
        /// 启动FSM
        /// </summary>
        public const string ENABLE_STEP_KEY = "ENABLE_STEP";
        /// <summary>
        /// 进入步骤开始阶段
        /// </summary>
        public const string ENTER_START_STEP_KEY = "ENTER_START_STEP";
        /// <summary>
        /// 步骤开始阶段Update
        /// </summary>
        public const string UPDATE_START_STEP_KEY = "UPDATE_START_STEP";
        /// <summary>
        /// 离开步骤开始阶段
        /// </summary>
        public const string LEAVE_START_STEP_KEY = "LEAVE_START_STEP";
        /// <summary>
        /// 进入步骤处理阶段
        /// </summary>
        public const string ENTER_EXECUTE_STEP_KEY = "ENTER_EXECUTE_STEP";
        /// <summary>
        /// 步骤处理阶段Update
        /// </summary>
        public const string UPDATE_EXECUTE_STEP_KEY = "UPDATE_EXECUTE_STEP";
        /// <summary>
        /// 离开步骤处理阶段
        /// </summary>
        public const string LEAVE_EXECUTE_STEP_KEY = "LEAVE_EXECUTE_STEP";
        /// <summary>
        /// 进入步骤结束阶段
        /// </summary>
        public const string ENTER_END_STEP_KEY = "ENTER_END_STEP";
        /// <summary>
        /// 步骤结束阶段Update
        /// </summary>
        public const string UPDATE_END_STEP_KEY = "UPDATE_END_STEP";
        /// <summary>
        /// 离开步骤结束阶段
        /// </summary>
        public const string LEAVE_END_STEP_KEY = "LEAVE_END_STEP";

    }
}
