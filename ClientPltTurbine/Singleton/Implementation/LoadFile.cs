using ClientPltTurbine.Controllers.LoadFileController;
using System.Threading.Tasks;

namespace ClientPltTurbine.Singleton.Implementation
{
    public class LoadFile
    {
        private readonly LoadFileController Controller = new();
        private readonly EventSingleton EventSingletonInstance;
        public LoadFile(EventSingleton eventSingleton) => EventSingletonInstance = eventSingleton; 
        public async Task LoadAllFiles()
        {
            EventSingletonInstance.SendMessage("123"); 
            await Controller.ReadAllFiles();
        }
    }
}
