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
        private readonly List<string> Date = new();
        private readonly List<Turbine> Turbines = new();
        protected override Task OnInitializedAsync()
        {
            Ids.Add(string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17))));
            return base.OnInitializedAsync();
        }
        private void ChangeInfoTurbine(string idTurbine)
        { 
        }
        private void ChangeInfoPeriod(string period)
        {
            Date.Add(period);
        }
        private async void SaveMaintenanceDate()
        {
        }
        private async void AddRow()
        {
            Ids.Add(string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17))));
        }
    }
}
