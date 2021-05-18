using ClientPltTurbine.Model.MaintenanceModel.Contract;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using PltWindTurbine.Services.MaintenanceService;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.MaintenanceModel.Implementation
{
    public class MaintenanceModel : IMaintenanceModel
    {
        private new readonly Maintenances.MaintenancesClient _clientMaintenance;
        private readonly AsyncDuplexStreamingCall<MaintenanceTurbinesRequest, MaintenanceTurbinesResponse> _duplexStreamMaintenance;
        public Task SaveMaintenanceTurbines(Dictionary<string, (int id, string date)> infoTurbineMaintenance) => Task.Run(() =>
        {
            var listTurbineMaintenance = infoTurbineMaintenance.Values.ToList();
            var total = listTurbineMaintenance.Count;
            listTurbineMaintenance.ForEach(values=>{ 
            });
        }); 
    }
}
