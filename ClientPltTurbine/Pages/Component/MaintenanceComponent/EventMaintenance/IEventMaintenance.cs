using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.MaintenanceComponent.EventMaintenance
{
    public interface IEventMaintenance
    { 
        public event EventHandler<IEventComponent> StatusMaintenance;  
    }
    public record MaintenanceStatusRecord(int TypeMsg, string Msg) : IEventComponent;
}
