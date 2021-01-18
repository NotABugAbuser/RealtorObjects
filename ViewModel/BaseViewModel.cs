using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using RealtorObjects.Model;
using RealtyModel.Model;

namespace RealtorObjects.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Коллекция сообщений лога событий данной ViewModel.
        /// </summary>
        protected ObservableCollection<LogMessage> Log { get; set; }

        public BaseViewModel() { }
        public BaseViewModel(ObservableCollection<LogMessage> log)
        {
            this.Log = log;
        }

        /// <summary>
        /// Метод для добавления сообщений в лог событий (Log) при помощи диспетчера основного (UI) потока.
        /// </summary>
        /// <param name="message">message - текст сообщения. При вызове метода желательно указывать место вызова.</param>
        protected void UpdateLog(String message)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                this.Log.Add(new LogMessage(DateTime.Now.ToString("dd:MM:yy hh:mm"), message));
            }));
        }

        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
