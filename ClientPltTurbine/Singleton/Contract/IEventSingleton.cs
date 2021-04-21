using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Singleton.Contract
{
    public interface IEventSingleton
    {
        public event EventHandler<string> SendStatus;
    }
}
