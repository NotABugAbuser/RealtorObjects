using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    class House : BaseRealtorObject
    {
        HouseInfo info = new HouseInfo();

        public HouseInfo Info {
            get => info;
            set {
                info = value;
                OnPropertyChanged();
            }
        }
    }
}
