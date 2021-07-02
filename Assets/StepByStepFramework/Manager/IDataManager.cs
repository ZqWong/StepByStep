using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace xr.StepByStep.Manager
{
    public interface IDataManager
    {
        void Initialize(JsonData stepJsonData);


        void StepMoveNext();
    }
}
