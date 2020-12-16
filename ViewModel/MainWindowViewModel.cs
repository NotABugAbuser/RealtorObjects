using RealtorObjects.Model;
using RealtorObjects.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        ObservableCollection<Flat> flats = new ObservableCollection<Flat>() { new Flat(), new Flat(), new Flat() };
        CustomCommand testCommand;
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            FlatForm window = new FlatForm(new FlatFormViewModel(Flats[0]));
            window.Show();
        }));
        CustomCommand secondTestCommand;
        public CustomCommand SecondTestCommand => secondTestCommand ?? (secondTestCommand = new CustomCommand(obj => { 
            MessageBox.Show(JsonSerializer.Serialize(Flats[0]).Replace(',', '\n'));
        }));
        internal ObservableCollection<Flat> Flats {
            get => flats; set => flats = value;
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
