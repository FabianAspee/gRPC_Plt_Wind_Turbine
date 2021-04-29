using ClientPltTurbine.Controllers.LoadFileController; 
using System.Threading.Tasks;
using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.EventContainer.Implementation;
using ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile;
using System;
using ClientPltTurbine.EventContainer;
using Blazored.Toast.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent
{
    
    class LoadFile:EventHandlerSystem, IEventLoadFile
    {
        public IToastService Service;
        public string status = string.Empty;
        public event EventHandler<IEventComponent> LoadSatus;
        private readonly ILoadFileController Controller = new LoadFileController();
        private readonly IEventContainer container = EventContainer.Implementation.EventContainer.Container;
        public void RegisterEvent()
        {
            container.AddEvent(EventKey.LOAD_FILE_KEY, LoadSatus);
        }
        public Task WriteInfo(IEventComponent loadStatus) => loadStatus switch
        {
            LoadStatusRecord { Msg: _, TypeMsg: 1 } status => Task.Run(() => Service.ShowInfo(status.Msg)),
            LoadStatusRecord { Msg: _, TypeMsg: 2 } status => Task.Run(() => Service.ShowSuccess(status.Msg)),
            LoadStatusRecord { Msg: _, TypeMsg: 3 } status => Task.Run(() => Service.ShowError(status.Msg)),
            _ => Task.Run(() => Service.ShowError("ERROR"))
        };
           
        
        public async Task LoadInfoTurbines(Dictionary<string, IBrowserFile> files)
        {
            SendEventLoadFile("Start load basic file");
            try {

                var allTask = Controller.ReadBasicFiles(files);
                await Task.WhenAll(allTask);
            }
            catch(Exception e)
            {
                SendEventLoadFile(e.Message); 
            }
             
        }
        public async Task LoadSensorTurbines(Dictionary<string, IBrowserFile> files)
        {
            status = "Load Sensor Turbine!"; 
            SendEventLoadFile("Start load sensor file");
            var allTask = Controller.ReadSensorTurbine(files); 
            await Task.WhenAll(allTask); 
        }
        public async Task LoadEventSensorTurbines(Dictionary<string, IBrowserFile> files)
        {
            SendEventLoadFile("Start load event file");
            var allTask = Controller.ReadEventSensorTurbine(files);
            await Task.WhenAll(allTask).ConfigureAwait(false);

        }
    }
}
