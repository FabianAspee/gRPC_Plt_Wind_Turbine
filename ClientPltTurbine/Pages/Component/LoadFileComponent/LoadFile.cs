using ClientPltTurbine.Controllers.LoadFileController; 
using System.Threading.Tasks;
using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.EventContainer.Implementation;
using ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile;
using System;
using ClientPltTurbine.EventContainer;
using Blazored.Toast.Services;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent
{
    
    class LoadFile:EventHandlerSystem, IEventLoadFile
    {
        public IToastService Service;
        public string status = string.Empty;
        public event EventHandler<LoadStatusRecord> LoadSatus;
        private readonly LoadFileController Controller = new();
        private readonly IEventContainer container = EventContainer.Implementation.EventContainer.Container;
        public async Task RegisterEvent()
        {
            await container.AddEvent(EventKey.LOAD_FILE_KEY, LoadSatus);
        }
        public Task WriteInfo(LoadStatusRecord loadStatus) => loadStatus switch {
            LoadStatusRecord {Msg:_,TypeMsg:1 } => Task.Run(() => Service.ShowInfo(va)),
            LoadStatusRecord value => Task.Run(() => Service.ShowSuccess(msg))
        };
           
        
        public async Task LoadInfoTurbines()
        {
            SendEventLoadFile("Start load basic file");
            var allTask = Controller.ReadBasicFiles();
            await Task.WhenAll(allTask);
             
        }
        public async Task LoadSensorTurbines()
        {
            status = "Load Sensor Turbine!";
            await Task.Run(() => {
                SendEventLoadFile("Start load sensor file");
                var allTask = Controller.ReadSensorTurbine();
                Task.WaitAll(allTask.ToArray()); 
            });  
        }
        public async Task LoadEventSensorTurbines()
        {
            SendEventLoadFile("Start load event file");
            var allTask = Controller.ReadEventSensorTurbine();
            await Task.WhenAll(allTask).ConfigureAwait(false);

        }
    }
}
