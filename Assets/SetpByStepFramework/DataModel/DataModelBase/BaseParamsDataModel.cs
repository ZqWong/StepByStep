using System;
using LitJson;

namespace xr.StepByStep.DataModel
{
    [Serializable]
    public class BaseParamsDataModel
    {
        public string Id;

        public string CreateTime;

        public string UpdateTime;

        public bool DeleteFlag;

        public BaseParamsDataModel() { }

        public BaseParamsDataModel(JsonData jdata)
        {
            Id = jdata.ContainsKey("id") ? jdata["id"].ToString() : "-1";

            CreateTime = jdata.ContainsKey("createTime") ? jdata["createTime"].ToString() : "-1";

            UpdateTime = jdata.ContainsKey("updateTime") ? jdata["updateTime"].ToString() : "-1";

            DeleteFlag = jdata.ContainsKey("delFlag") ? jdata["delFlag"].ToString() == "1" ? true : false : true;
        }
    }
}
