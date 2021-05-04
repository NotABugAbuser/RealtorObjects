using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model.Event
{
    public delegate void ConnectedEventHandler(object sender, ConnectedEventArgs e);
    public delegate void DisconnectedEventHandler(object sender, DisconnectedEventArgs e);
    public delegate void ReconnectedEventHandler(object sender, ReconnectedEventArgs e);
    public delegate void UpdateCompletedEventHandler(object sender, UpdateCompletedEventArgs e);
}
