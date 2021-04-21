using System.Data;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.LoadFileController
{
    public interface ILoadFileController
    { 
        Task ReadBasicFiles();
        Task ReadSensorTurbine();
    }
}
