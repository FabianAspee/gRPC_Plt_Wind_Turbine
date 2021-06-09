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
        private readonly Dictionary<string, (int id, string dateInit,string dateFinish, bool isNormalMaintenance)> InfoTurbineMaintenance = new(); 
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
            if (!InfoTurbineMaintenance.TryGetValue(id, out (int id,string _, string __, bool ___) info))
            {
                InfoTurbineMaintenance.Add(id,(myId, string.Empty, string.Empty, default));
            }
            else
            {
                InfoTurbineMaintenance[id]=(myId, info._, info.__,info.___);
            }
        }
        private void ChangeInfoPeriod(string period)
        {
            var splitString = period.Split(",");
            (string id, string periodT) = (splitString[0], splitString[1]); 
            if (!InfoTurbineMaintenance.TryGetValue(id, out (int _, string date, string __, bool ___) info))
            {
                InfoTurbineMaintenance.Add(id, (default,periodT,string.Empty, default));
            }
            else
            {
                InfoTurbineMaintenance[id] = (info._, periodT,info.__, info.___);
            }
        }
        private void ChangeInfoPeriodFinish(string period)
        {  //validation date for all date existing in this point
            var splitString = period.Split(",");
            (string id, string periodT) = (splitString[0], splitString[1]);
            if (!InfoTurbineMaintenance.TryGetValue(id, out (int _, string __, string datef, bool ___) info))
            {
                InfoTurbineMaintenance.Add(id, (default, string.Empty, periodT, default));
            }
            else
            {
                InfoTurbineMaintenance[id] = (info._, info.__, periodT, info.___);
            }
        }

        private void ChangeIsNormalMaintenance(string period)
        {  //validation date for all date existing in this point
            var splitString = period.Split(",");
            (string id, bool isNormalMaintenance) = (splitString[0], Convert.ToBoolean(splitString[1]));
            if (!InfoTurbineMaintenance.TryGetValue(id, out (int _, string __, string ___, bool isNormalMaintenance) info))
            {
                InfoTurbineMaintenance.Add(id, (default, string.Empty, string.Empty, isNormalMaintenance));
            }
            else
            {
                InfoTurbineMaintenance[id] = (info._, info.__, info.___, isNormalMaintenance);
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
