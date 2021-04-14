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
        private Client client;

        public Client Client
        {
            get => client;
            set => client = value;
        }

        public RealtorObjectOperator()
        {
        }
        public void HandleFlat(object _, FlatCreatedEventArgs e)
        {
            Operation operation = new Operation(Client.CurrentAgent, JsonSerializer.Serialize(e.Flat), OperationDirection.Realty, OperationType.Add, TargetType.Flat);
            if (!e.IsNew)
            {
                operation.OperationParameters.Type = OperationType.Change;
            }
            Client.SendMessage(operation);
        }
        public void CreateFlat()
        {
            Flat flat = new Flat(true) { Agent = Client.CurrentAgent };
            FlatFormV2 flatForm = new FlatFormV2();
            FlatFormViewModel flatFormVM = new FlatFormViewModel();
            flatFormVM.Title = "[Квартира] — Создание";
            flatFormVM.Flat = flat;
            flatFormVM.IsCurrentFlatNew = true;
            flatFormVM.LocationOptions = new LocationOptions();
            flatFormVM.FlatCreated = (s, e) => ((App)Application.Current).OperationManagement.SendFlat(flat, OperationType.Add);
            flatForm.DataContext = flatFormVM;
            flatForm.Show();
        }
        public void ModifyFlat(Flat flat)
        {
            FlatFormViewModel flatFormVM = new FlatFormViewModel();
            flatFormVM.Title = "[Квартира] — Редактирование";
            flatFormVM.Flat = JsonSerializer.Deserialize<Flat>(JsonSerializer.Serialize(flat)); //нужно, чтобы разорвать связь объекта в форме и объекта в списке
            flatFormVM.IsCurrentFlatNew = false;
            flatFormVM.LocationOptions = new LocationOptions();
            flatFormVM.FlatCreated +=(s,e)=> ((App)Application.Current).OperationManagement.SendFlat(flat, OperationType.Change);
            new FlatFormV2 { DataContext = flatFormVM }.Show();
        }
        public void HandleHouse()
        {
        }
        public void CreateHouse()
        {
        }
        public void ModifyHouse(House house)
        {
        }
        public void UpdateFlat(Flat flat)
        {

        }
        public void DeleteFlat(Flat flat)
        {

        }
    }
}
