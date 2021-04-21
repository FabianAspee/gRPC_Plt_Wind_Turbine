using ClientPltTurbine.Singleton.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Singleton.Implementation
{
    public class EventSingleton : IEventSingleton
    {
        public event EventHandler<string> SendStatus;

        public void SendMessage(string msg) => SendStatus.Invoke(this, msg);
    }
}
