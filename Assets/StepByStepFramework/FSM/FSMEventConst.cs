using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xr.StepByStepFramework.FSM
{
    public class FSMEventConst
    {
        public const string ENABLE_STEP_KEY = "ENABLE_STEP";

        public const string ENTER_START_STEP_KEY = "ENTER_START_STEP";
        public const string UPDATE_START_STEP_KEY = "UPDATE_START_STEP";
        public const string LEAVE_START_STEP_KEY = "LEAVE_START_STEP";

        public const string ENTER_EXECUTE_STEP_KEY = "ENTER_EXECUTE_STEP";
        public const string UPDATE_EXECUTE_STEP_KEY = "UPDATE_EXECUTE_STEP";
        public const string LEAVE_EXECUTE_STEP_KEY = "LEAVE_EXECUTE_STEP";

        public const string ENTER_END_STEP_KEY = "ENTER_END_STEP";
        public const string UPDATE_END_STEP_KEY = "UPDATE_END_STEP";
        public const string LEAVE_END_STEP_KEY = "LEAVE_END_STEP";

    }
}
