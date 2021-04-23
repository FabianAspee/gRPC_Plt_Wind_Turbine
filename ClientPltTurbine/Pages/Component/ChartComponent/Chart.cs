using Blazored.Toast.Services;
using ClientPltTurbine.Controllers.ChartController;
using ClientPltTurbine.EventContainer;
using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    public class Chart: IEventChart
    {
        public int NumTurbine;
        public IToastService Service;
        private readonly ChartController Controller = new();
        private readonly IEventContainer container = EventContainer.Implementation.EventContainer.Container;

        public event EventHandler<IEventComponent> InfoChart;

        public void RegisterEvent()
        {
            container.AddEvent(EventKey.GRAPH_KEY, InfoChart);
        }
        public Task WriteInfo(IEventComponent loadStatus) => loadStatus switch
        {
            LoadStatusChart { Msg: _, TypeMsg: 1 } status => Task.Run(() => Service.ShowInfo($"Turbine {status.NameTurbine} Status {status.Msg}")),
            ResponseSerieByPeriod status => Task.Run(() => Service.ShowSuccess(status.Values)),
            ResponseSerieByPeriodWithStandardDeviation status => Task.Run(() => Service.ShowError(status.StandardDeviation.ToString())),
            _ => Task.Run(() => Service.ShowError("ERROR"))
        };
        public async Task GraphicInfoTurbine()
        {
            await Controller.GraphAllTurbines().ConfigureAwait(false); 
        }
    }
}
