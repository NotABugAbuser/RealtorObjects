using RealtyModel.Model.Base;
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

        private ObservableCollection<BaseRealtorObject> originalObjectList = new ObservableCollection<BaseRealtorObject>() {
        };
        public ObservableCollection<bool> ToggledButtons {
            get => toggledButtons; set => toggledButtons = value;
        }
        public ObservableCollection<BaseRealtorObject> OriginalObjectList => originalObjectList;
    }
}
