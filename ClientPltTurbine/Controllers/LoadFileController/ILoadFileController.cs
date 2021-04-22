using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.LoadFileController
{
    public interface ILoadFileController
    {
        Task[] ReadBasicFiles();
        Task[] ReadEventSensorTurbine();
        Task[] ReadSensorTurbine();
    }
}
