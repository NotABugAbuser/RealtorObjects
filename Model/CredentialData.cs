using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    public class CredentialData : INotifyPropertyChanged
    {
        private string currentUsername = "";
        private string currentPassword = "";
        private string name = "";
        private string surname = "";
        private string patronymic = "";
        private string email = "";
        private string firstPassword = "";
        private string secondPassword = "";
        public event PropertyChangedEventHandler PropertyChanged;


        public void SetNewUsername()
        {
            CurrentUsername = $"{Surname}{Name[0]}{Patronymic[0]}";
        }
        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public string CurrentUsername
        {
            get => currentUsername;
            set
            {
                currentUsername = value;
                OnPropertyChanged();
            }
        }
        public string CurrentPassword
        {
            get => currentPassword;
            set
            {
                currentPassword = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public string Surname
        {
            get => surname;
            set
            {
                surname = value;
                OnPropertyChanged();
            }
        }
        public string Patronymic
        {
            get => patronymic;
            set
            {
                patronymic = value;
                OnPropertyChanged();
            }
        }
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }
        public string FirstPassword
        {
            get => firstPassword;
            set
            {
                firstPassword = value;
                OnPropertyChanged();
            }
        }
        public string SecondPassword
        {
            get => secondPassword;
            set
            {
                secondPassword = value;
                OnPropertyChanged();
            }
        }

        
    }
}
