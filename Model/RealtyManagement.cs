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

        internal void Handle(ReceivedFlatEventArgs e)
        {
            if (e.Action == Act.Add)
                realtorObjects.Add(e.Flat);
            else if (e.Action == Act.Change)
            {
                Flat flat = (Flat)realtorObjects.Find(f => f.Id == e.Flat.Id);
                if (flat != null)
                    flat = e.Flat;
            }
        }
    }
}
