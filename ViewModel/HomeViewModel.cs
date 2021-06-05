﻿using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using RealtorObjects.Model;
using RealtorObjects.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        #region Fields
        private string currentAgentName = "";
        private int currentPage = 1;
        private double widthOfFilters = 200;
        private Filter filter = new Filter();
        private Street[] streets = Array.Empty<Street>();
        private CustomCommand modify;
        private CustomCommand goToPage;
        private CustomCommand openCloseFilters;
        private CustomCommand sendQuery;
        private ObservableCollection<int> pages = new ObservableCollection<int>();
        private List<BaseRealtorObject> currentObjectList = new List<BaseRealtorObject>() { };
        private List<List<BaseRealtorObject>> objectLists = new List<List<BaseRealtorObject>>();
        private CustomCommand createFlat;
        #endregion
        #region Properties
        public int CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                OnPropertyChanged();
            }
        }
        public double WidthOfFilters
        {
            get => widthOfFilters;
            set
            {
                widthOfFilters = value;
                OnPropertyChanged();
            }
        }
        public Filter Filter
        {
            get => filter;
            set => filter = value;
        }
        public List<List<BaseRealtorObject>> ObjectLists
        {
            get => objectLists;
            set => objectLists = value;
        }
        public ObservableCollection<int> Pages
        {
            get => pages; set => pages = value;
        }
        public List<BaseRealtorObject> CurrentObjectList
        {
            get => currentObjectList;
            set
            {
                currentObjectList = value;
                OnPropertyChanged();
            }
        }

        #endregion
        public HomeViewModel()
        {
            //this.CurrentObjectList.Add(Flat.CreateTestFlat());
            this.Streets = Client.RequestStreets();
        }

        private void SplitBy(List<BaseRealtorObject> filteredList, byte pageSize)
        {
            ObjectLists.Clear();
            if (filteredList.Count > pageSize)
            {
                foreach (var batch in filteredList.Batch(25))
                {
                    ObjectLists.Add(batch.ToList());
                }
                CalculatePages(0);
                CurrentPage = 1;
            }
            else
            {
                ObjectLists.Add(new List<BaseRealtorObject>(filteredList));
            }
        }
        public CustomCommand SendQuery => sendQuery ?? (sendQuery = new CustomCommand(obj =>
        {
            CurrentObjectList.Clear();
            SplitBy(Client.RequestRealtorObjects(Filter), 25);
            CurrentObjectList = ObjectLists[0];
        }));
        public CustomCommand CreateFlat => createFlat ?? (createFlat = new CustomCommand(obj =>
        {
            FlatFormViewModel flatFormVM = new FlatFormViewModel(CurrentAgentName);
            flatFormVM.Streets = this.Streets;
            new FlatFormV2(flatFormVM).Show();
        }));
        public CustomCommand OpenCloseFilters => openCloseFilters ?? (openCloseFilters = new CustomCommand(obj =>
        {
            if (WidthOfFilters == 200)
            {
                WidthOfFilters = 0;
            }
            else
            {
                WidthOfFilters = 200;
            }
        }));
        public CustomCommand Modify => modify ?? (modify = new CustomCommand(obj =>
        {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (bro is Flat flat)
            {
                FlatFormViewModel flatFormVM = new FlatFormViewModel(flat, CurrentAgentName);
                flatFormVM.Streets = this.Streets;
                flatFormVM.Streets = flatFormVM.Streets.OrderBy(s => s.Name).ToArray();
                new FlatFormV2(flatFormVM).Show();
            }
        }));
        public CustomCommand GoToPage => goToPage ?? (goToPage = new CustomCommand(obj =>
        {
            Int16 index = (Int16)(Convert.ToInt16(obj) - 1);
            Pages.Clear();
            CalculatePages(index);
            CurrentPage = index + 1;
            CurrentObjectList = ObjectLists[index];
        }));

        public string CurrentAgentName
        {
            get => currentAgentName; set => currentAgentName = value;
        }
        public Street[] Streets
        {
            get => streets;
            set
            {
                streets = value;
                OnPropertyChanged();
            }
        }
        private void CalculatePages(short currentPage)
        {
            int count = ObjectLists.Count;
            if (count < 15)
            {
                for (int i = 0; i < count; i++) { Pages.Add(i + 1); }
            }
            else
            {
                int left = 7;
                int right = 7;
                if (currentPage + 8 > count)
                {
                    int difference = -(count - 8 - currentPage);
                    left += difference;
                    right -= difference;
                }
                if (currentPage - 7 < 0)
                {
                    int difference = -(currentPage - 7);
                    left -= difference;
                    right += difference;
                }
                for (int i = left; i > 0; i--) { Pages.Add(currentPage - i + 1); }
                for (int i = 0; i < right + 1; i++) { Pages.Add(currentPage + i + 1); }
            }
        }
    }
}
