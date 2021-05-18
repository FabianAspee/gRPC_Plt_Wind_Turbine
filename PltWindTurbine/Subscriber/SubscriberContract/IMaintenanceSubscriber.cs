using PltWindTurbine.Services.MaintenanceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberContract
{
    public interface IMaintenanceSubscriber : ISubscriber
    {
        Task SaveMaintenanceTurbine(SaveTurbineInfoMaintenance saveTurbine, bool isFinish);
    }
}
