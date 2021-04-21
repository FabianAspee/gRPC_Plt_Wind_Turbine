using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.FetchDataComponent.EventFetchData
{
    internal interface IEventFetchData
    {
        public event EventHandler<string> FetchDataSatus;
    }
}
