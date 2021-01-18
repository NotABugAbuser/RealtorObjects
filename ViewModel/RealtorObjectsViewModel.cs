using RealtyModel.Model;
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

        public ObservableCollection<bool> ToggledButtons {
            get => toggledButtons; set => toggledButtons = value;
        }

        public RealtorObjectsViewModel() { }
        public RealtorObjectsViewModel(ObservableCollection<LogMessage> log) : base(log)
        {
            this.Log = log;
        }
    }
}
