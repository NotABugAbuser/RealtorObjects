using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.ViewModel
{
    public class LoadingFormViewModel : INotifyPropertyChanged
    {
        private String text = "";

        public String Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        public LoadingFormViewModel()
        {
            Text = "Подключение";
        }

        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
