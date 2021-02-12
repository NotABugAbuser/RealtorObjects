using RealtyModel.Model.Base;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.ViewModel
{
    class HomeViewModel : BaseViewModel
    {
        ObservableCollection<bool> toggledButtons = new ObservableCollection<bool> {
            true,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
        };
        private ObservableCollection<CheckAndHeightPair> filterAreaSections = new ObservableCollection<CheckAndHeightPair>() { 
            new CheckAndHeightPair(true, 200),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50)
        };
        private ObservableCollection<BaseRealtorObject> originalObjectList = new ObservableCollection<BaseRealtorObject>() {
        };
        private CustomCommand openOrCloseFilterSection;
        public CustomCommand OpenOrCloseFilterSection => openOrCloseFilterSection ?? (openOrCloseFilterSection = new CustomCommand(obj => {
            object[] objects = obj as object[];
            byte index = Convert.ToByte(objects[0]);
            Int16 height = Convert.ToInt16(objects[1]);

            if (!FilterAreaSections[index].Key) {
                FilterAreaSections[index].Value = 50;
            } else {
                FilterAreaSections[index].Value = height;
            }
        }));
        public ObservableCollection<bool> ToggledButtons {
            get => toggledButtons; 
            set => toggledButtons = value;
        }
        public ObservableCollection<CheckAndHeightPair> FilterAreaSections => filterAreaSections;
        public ObservableCollection<BaseRealtorObject> OriginalObjectList => originalObjectList;
    }
}
