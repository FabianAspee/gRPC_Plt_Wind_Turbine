using System.Data;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.LoadFileController
{
    public interface ILoadFileController
    {
        Task ReadAllFiles();
        Task ReadBasicFiles(DataTable file, string name, string type, string sep, int id);
        Task ReadSensorTurbine(DataTable file, string name, string type, string sep, bool isEvent);
    }
}
