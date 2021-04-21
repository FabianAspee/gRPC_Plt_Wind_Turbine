using ClientPltTurbine.Controllers.LoadFileController; 
using System.Threading.Tasks;
using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.EventContainer.Implementation;
using ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile;
using System;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent
{
    
    class LoadFile: IEventLoadFile
    {
        public string status = string.Empty;
        public event EventHandler<string> LoadSatus;
        private readonly LoadFileController Controller = new();
        private readonly IEventContainer container = EventContainer.Implementation.EventContainer.Container;
        private async Task WriteInfo(string response)
        {
            status = response;
        }
        public async Task LoadInfoTurbines()
        {
            LoadSatus += async (sender, args) =>
              await WriteInfo(args);
            LoadSatus.Invoke(this,"123");
            await Controller.ReadBasicFiles();
             
        }
        public async Task LoadSensorTurbines()
        {
            LoadSatus += async (sender, args) =>
              await WriteInfo(args);
            LoadSatus.Invoke(this, "123");
            await Controller.ReadSensorTurbine();

        }
    }
}
