using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.IndexComponent.EventIndex
{
    internal interface IEventIndex
    {
        public event EventHandler<string> IndexStatus;
    }
}
