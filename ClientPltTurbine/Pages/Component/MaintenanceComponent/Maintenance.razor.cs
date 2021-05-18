using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.MaintenanceComponent
{
    public partial class Maintenance
    {
        private readonly List<string> Ids = new(); 
        private readonly MaintenanceClass maintenance = new();
        private readonly Dictionary<string, (int id, string date)> InfoTurbineMaintenance = new(); 
        private readonly List<Turbine> Turbines = new();
        private async void InitializedComponent()
        {
            maintenance.Service = toastService;
            maintenance.CommonInfoEvent += async (sender, args) =>
               await maintenance.CommonInfo(args);
            maintenance.StatusMaintenance += async (sender, args) =>
               await maintenance.StatusSaveMaintenance(args);
            await Task.Run(() => maintenance.RegisterEvent());
            CreateAndSaveId();
        }
        protected override async Task OnInitializedAsync()
        {
            InitializedComponent();
            await maintenance.CallAllTurbine();
            await AwaitTurbines();
        }
        private async Task AwaitTurbines()
        {  
            await foreach (var turbine in maintenance.GetTurbine())
            {
                Turbines.Add(turbine);
            }
        }
        private void ChangeInfoTurbine(string idTurbine)
        {
            var splitString = idTurbine.Split(",");
            (string id, string idT) = (splitString[0],splitString[1]);
            var myId = Convert.ToInt32(idT);
            if (!InfoTurbineMaintenance.TryGetValue(id, out (int id,string _) info))
            {
                InfoTurbineMaintenance.Add(id,(myId,string.Empty));
            }
            else
            {
                InfoTurbineMaintenance[id]=(myId, info._);
            }
        }
        private void ChangeInfoPeriod(string period)
        {
            var splitString = period.Split(",");
            (string id, string periodT) = (splitString[0], splitString[1]); 
            if (!InfoTurbineMaintenance.TryGetValue(id, out (int _, string date) info))
            {
                InfoTurbineMaintenance.Add(id, (default,periodT));
            }
            else
            {
                InfoTurbineMaintenance[id] = (info._, periodT);
            }
        }
        private async void SaveMaintenanceDate()
        {
            await maintenance.SaveMaintenanceForTurbine(InfoTurbineMaintenance);
        }
        private void CreateAndSaveId()
        {
            var Id = string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17)));
            Ids.Add(Id);
        }
        private void AddRow() => CreateAndSaveId(); 
    }
}
