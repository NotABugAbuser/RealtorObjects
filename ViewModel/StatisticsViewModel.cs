using RealtyModel.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.ViewModel
{
    class StatisticsViewModel : BaseViewModel
    {
        public StatisticsViewModel(ObservableCollection<LogMessage> log) : base(log)
        {
            this.Log = log;
        }
    }
}
