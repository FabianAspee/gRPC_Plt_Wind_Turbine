using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.MaintenanceModel.Contract
{
    interface IMaintenanceModel
    {
        Task SaveMaintenanceTurbines(Dictionary<string, (int id, string date, string datef)> infoTurbineMaintenance);
    }
}
