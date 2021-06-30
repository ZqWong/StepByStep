using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using xr.SetpByStepFramework.FSM;
using xr.StepByStep.DataModel;

[Serializable]
public class StepDataModel : BaseParamsDataModel
{
    public string Param1;
    public string Param2;
    public string Param3;
    public string Param4;
    public string Param5;

    public StepDataModel() : base()
    {

    }

    public StepDataModel(JsonData jsonData) : base(jsonData)
    {

    }

}

