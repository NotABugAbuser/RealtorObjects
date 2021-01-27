using RandomFlatGenerator;
using RealtyModel.Model.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.ViewModel
{
    class RealtorObjectsViewModel : BaseViewModel
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
        FlatGenerator flatGenerator = new FlatGenerator();
        private ObservableCollection<BaseRealtorObject> originalObjectList = new ObservableCollection<BaseRealtorObject>() { 
        };
        public RealtorObjectsViewModel() {
            //OriginalObjectList.Add(flatGenerator.CreateFlat());
        }
        public ObservableCollection<bool> ToggledButtons {
            get => toggledButtons; set => toggledButtons = value;
        }
        public ObservableCollection<BaseRealtorObject> OriginalObjectList => originalObjectList;
    }
}
