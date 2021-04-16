using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Event;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.Model
{
    public class RealtorObjectOperator
    {
        private Flat flat;
        private House house;
        private FlatFormV2 flatForm;
        private HouseForm houseForm;
        private HomeViewModel homeVM; //для изменения коллекций через VM

        public RealtorObjectOperator(HomeViewModel homeVM)
        {
            this.homeVM = homeVM;
        }

        public void UpdateFlat(Flat flat)
        {

        }
        public void DeleteFlat(Flat flat)
        {

        }
        public void UpdateHouse(House house)
        {

        }
        public void DeleteHouse(House house)
        {

        }
    }
}
