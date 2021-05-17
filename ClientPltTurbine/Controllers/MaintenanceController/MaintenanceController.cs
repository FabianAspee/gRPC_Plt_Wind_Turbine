using ClientPltTurbine.Model.MaintenanceModel.Contract;
using ClientPltTurbine.Model.MaintenanceModel.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.MaintenanceController
{
    public class MaintenanceController : BaseController, IMaintenanceController
    {
        private readonly IMaintenanceModel Maintenance = new MaintenanceModel();
        public Task CallAllTurbines() => GetAllNameTurbines();
        public Task SaveMaintenanceTurbines(Dictionary<string, (int id, string date)> infoTurbineMaintenance) => Maintenance.SaveMaintenanceTurbines(infoTurbineMaintenance);
    }
}
