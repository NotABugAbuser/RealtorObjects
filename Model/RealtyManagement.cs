using RealtyModel.Events.Realty;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Threading;

namespace RealtorObjects.Model
{
    public class RealtyManagement
    {
        private List<BaseRealtorObject> realtorObjects { get; set; }

        public RealtyManagement(List<BaseRealtorObject> objectList)
        {
            realtorObjects = objectList;
        }

    }
}
