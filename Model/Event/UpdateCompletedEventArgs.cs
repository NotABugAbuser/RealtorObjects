using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model.Event
{
    public class UpdateCompletedEventArgs
    {
        private DateTime dateTime = DateTime.Now;
        public DateTime DateTime
        {
            get => dateTime;
            set => dateTime = value;
        }
    }
}
