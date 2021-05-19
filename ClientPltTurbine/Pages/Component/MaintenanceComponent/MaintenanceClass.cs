using Blazored.Toast.Services;
using ClientPltTurbine.Controllers.MaintenanceController;
using ClientPltTurbine.EventContainer;
using ClientPltTurbine.Pages.Component.MaintenanceComponent.EventMaintenance;
using ClientPltTurbine.Pages.Component.UtilComponent;
using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.MaintenanceComponent
{
    public class MaintenanceClass: CommonMethod, IEventMaintenance
    {

        private readonly IMaintenanceController Controller = new MaintenanceController();

        public IToastService Service; 
        public event EventHandler<IEventComponent> StatusMaintenance;
        public new Task CommonInfo(IEventComponent turbineAndSensor) => base.CommonInfo(turbineAndSensor);
        public new async IAsyncEnumerable<Turbine> GetTurbine()
        {
            await foreach (var value in base.GetTurbine())
                yield return value;
        }
        internal Task StatusSaveMaintenance(IEventComponent maintenanceStatus) => maintenanceStatus switch
        {
            MaintenanceStatusRecord { Msg: _, TypeMsg: 1 } status => Task.Run(() => Service.ShowInfo(status.Msg)),
            MaintenanceStatusRecord { Msg: _, TypeMsg: 2 } status => Task.Run(() => Service.ShowSuccess(status.Msg)),
            MaintenanceStatusRecord { Msg: _, TypeMsg: 3 } status => Task.Run(() => Service.ShowError(status.Msg)),
            _ => Task.Run(() => Service.ShowError("ERROR"))
        };
        internal new void RegisterEvent()
        {
            container.AddEvent(EventKey.MAINTENANCE_KEY, StatusMaintenance);
            base.RegisterEvent();
        }

        internal async Task CallAllTurbine()
        {
            InitliazidedComponent();
            await Controller.CallAllTurbines().ConfigureAwait(false);
        }

        internal Task SaveMaintenanceForTurbine(Dictionary<string, (int id, string date, string datef)> infoTurbineMaintenance) => Controller.SaveMaintenanceTurbines(infoTurbineMaintenance);
    }
}
