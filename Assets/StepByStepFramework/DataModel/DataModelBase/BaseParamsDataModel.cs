using System;
using etp.xr.Tools;
using LitJson;

namespace xr.StepByStepFramework.DataModel
{
    [Serializable]
    public class BaseParamsDataModel
    {
        public string Id;

        public string CreateTime;

        public string UpdateTime;

        public bool DeleteFlag;

        public BaseParamsDataModel(JsonData jsonData)
        {
            Id = jsonData.ContainsKey("id") ? jsonData["id"].ToString() : "-1";

            CreateTime = jsonData.ContainsKey("createTime") ? jsonData["createTime"].ToString() : "-1";

            UpdateTime = jsonData.ContainsKey("updateTime") ? jsonData["updateTime"].ToString() : "-1";

            DeleteFlag = jsonData.ContainsKey("delFlag") ? jsonData["delFlag"].ToString() == "1" ? true : false : true;
        }
    }
}
