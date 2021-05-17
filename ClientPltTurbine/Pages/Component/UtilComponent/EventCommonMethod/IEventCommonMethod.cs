using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.UtilComponent.EventCommonMethod
{
    interface IEventCommonMethod
    {
        public event EventHandler<IEventComponent> CommonInfoEvent;
    }
}
