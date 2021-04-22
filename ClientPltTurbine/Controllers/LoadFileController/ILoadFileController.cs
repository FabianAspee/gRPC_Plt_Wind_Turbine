using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.LoadFileController
{
    public interface ILoadFileController
    {
        List<Task> ReadBasicFiles();
        List<Task> ReadEventSensorTurbine();
        List<Task> ReadSensorTurbine();
    }
}
