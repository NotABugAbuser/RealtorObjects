using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Model;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    class RegistrationFormViewModel : BaseViewModel
    {
        private CredentialData credentials = new CredentialData();
        private CustomCommand register;
        public CustomCommand Register => register ?? (register = new CustomCommand(obj =>
        {

            if (credentials.FirstPassword.Equals(credentials.SecondPassword))
            {
                Credential current = new Credential();
                current.Name = $"{credentials.Surname}{credentials.Name[0]}{credentials.Patronymic[0]}";
                current.RegistrationDate = DateTime.Now;
                current.Password = credentials.SecondPassword;
                current.Email = "почта";
                if (Client.Register(current))
                    (obj as RegistrationForm).Close();
            }
            else
            {
                MessageBox.Show("Пароли не совпадают");
            }
        }));
        public CredentialData Credentials
        {
            get => credentials;
            set => credentials = value;
        }
    }
}
