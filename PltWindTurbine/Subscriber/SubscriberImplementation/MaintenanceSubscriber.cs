using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Services.MaintenanceService;
using PltWindTurbine.Subscriber.SubscriberContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberImplementation
{
    public class MaintenanceSubscriber : AbstractSubscriber, IMaintenanceSubscriber
    {
        private readonly IOperationTurbineDatabase database = RetreiveImplementationDatabase.Instance.ImplementationDatabase;

        public Task SaveMaintenanceTurbine(SaveTurbineInfoMaintenance saveTurbine, bool isFinish) => database.SaveMaintenanceTurbines(saveTurbine, isFinish);
    }
}
