using ClientPltTurbine.Model.MaintenanceModel.Contract; 
using System.Collections.Generic;
using System.Linq;
using PltWindTurbine.Services.MaintenanceService;
using System.Threading.Tasks;
using Grpc.Core;
using PltTurbineShared.ExtensionMethodList;

namespace ClientPltTurbine.Model.MaintenanceModel.Implementation
{
    public class MaintenanceModel :BaseModel, IMaintenanceModel
    {
        private readonly Maintenances.MaintenancesClient _clientMaintenance;
        private readonly AsyncDuplexStreamingCall<MaintenanceTurbinesRequest, MaintenanceTurbinesResponse> _duplexStreamMaintenance;
        public MaintenanceModel()
        {
            _clientMaintenance = new Maintenances.MaintenancesClient(channel);
            _duplexStreamMaintenance = _clientMaintenance.SaveMaintenanceTurbines();
            _ = HandleResponsesMaintenanceAsync();
        }

        private async Task HandleResponsesMaintenanceAsync()
        {
            await foreach(var response in _duplexStreamMaintenance.ResponseStream.ReadAllAsync())
            {
                SendEventLoadMaintenanceInfo(response.Name, response.Description, response.Status);
            }
        }

        public Task SaveMaintenanceTurbines(Dictionary<string, (int id, string date, string datef)> infoTurbineMaintenance) => Task.Run(() =>
        {
            var listTurbineMaintenance = infoTurbineMaintenance.Values.ToList();
            var total = listTurbineMaintenance.Count; 
            listTurbineMaintenance.ForEach((index,values)=>{
                var request = new MaintenanceTurbinesRequest() { Msg1 = new SaveTurbineInfoMaintenance() { IdTurbine = values.id, Date = values.date, Datef = values.datef },IsFinish=total==(index+1)};
                _duplexStreamMaintenance.RequestStream.WriteAsync(request).Wait();
            });
        }); 
    }
}
