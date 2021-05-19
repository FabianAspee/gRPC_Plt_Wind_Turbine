using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.MaintenanceController
{
    interface IMaintenanceController
    {
        Task CallAllTurbines();
        Task SaveMaintenanceTurbines(Dictionary<string, (int id, string date, string datef)> infoTurbineMaintenance);
    }
}
