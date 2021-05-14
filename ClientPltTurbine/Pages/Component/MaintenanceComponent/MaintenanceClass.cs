using ClientPltTurbine.Controllers.MaintenanceController;
using ClientPltTurbine.Pages.Component.UtilComponent;
using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.MaintenanceComponent
{
    public class MaintenanceClass: CommonMethod
    {

        private readonly IMaintenanceController Controller = new MaintenanceController();
        public new async IAsyncEnumerable<Turbine> GetTurbine()
        {
            await foreach (var value in base.GetTurbine())
                yield return value;
        }
    }
}
