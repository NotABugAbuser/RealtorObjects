using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model.DerivedClasses
{
    class Flat : BaseRealtorObject
    {
        FlatInfo info = new FlatInfo();

        public FlatInfo Info {
            get => info;
            set {
                info = value;
                OnPropertyChanged();
            }
        }
    }
}
