using ClientPltTurbine.Model.MaintenanceModel.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.MaintenanceModel.Implementation
{
    public class MaintenanceModel : IMaintenanceModel
    {
        public Task SaveMaintenanceTurbines(Dictionary<string, (int id, string date)> infoTurbineMaintenance)
        {
            throw new NotImplementedException();
        }
    }
}
