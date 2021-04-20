using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFillesAsDatatable.Controller.LoadFileController
{
    public interface ILoadFileController
    {
        Task ReadBasicFiles(DataTable file, string name, string type, string sep, int id);
        Task ReadSensorTurbine(DataTable file, string name, string type, string sep, bool isEvent);
    }
}
