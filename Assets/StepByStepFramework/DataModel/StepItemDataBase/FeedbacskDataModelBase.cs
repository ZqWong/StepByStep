using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace xr.StepByStepFramework.DataModel
{
    //[System.Serializable]
    public class FeedbacskDataModelBase : BaseParamsDataModel
    {

        /// <summary>
        /// 初始化反馈数据
        /// </summary>
        /// <param name="jsonData">JsonData</param>
        /// <param name="feedbacksKey">反馈数据组字段头</param>
        public FeedbacskDataModelBase(JsonData jsonData) : base(jsonData)
        {

        }
    }
}
