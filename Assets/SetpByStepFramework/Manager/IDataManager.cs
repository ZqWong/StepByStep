﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;

namespace xr.SetpByStep.Manager
{
    public interface IDataManager
    {
        void Initialize(JsonData stepJsonData);


    }
}
