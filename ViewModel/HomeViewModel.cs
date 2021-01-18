using RealtyModel.Model;
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
        public HomeViewModel(ObservableCollection<LogMessage> log) : base(log)
        {
            this.Log = log;
        }
    }
}
