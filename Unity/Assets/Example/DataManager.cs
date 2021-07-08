using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etp.xr.Tools;
using LitJson;
using xr.StepByStep.Manager;

namespace Assets.Example
{
    public class DataManager : SingletonMonoBehaviourClass<DataManager>, IDataManager
    {
        public void Awake()
        {
         
        }

        public void Start()
        {
          
        }

        public void Initialize(JsonData stepJsonData)
        {
           
        }

        public void StepMoveNext()
        {
          
        }

        public bool CheckCurrentStepComplete()
        {
            return true;
        }
    }
}
