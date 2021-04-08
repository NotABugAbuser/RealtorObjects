using RealtorObjects.Model;
using RealtyModel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private Client client = new Client();
        internal Client Client
        {
            get => client;
            set => client = value;
        }
        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
