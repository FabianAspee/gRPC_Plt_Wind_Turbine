using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model
{
    interface ICommonMethodModel
    {
        Task GetAllNameSensor();
        Task GetAllNameTurbine();
    }
}
