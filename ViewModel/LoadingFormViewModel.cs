using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class LoadingFormViewModel : INotifyPropertyChanged
    {
        private String text = "";
        private CustomCommand closeApp;
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
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
            Text = "Загрузка";
        }

        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
