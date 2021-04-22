using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.GraphModel.Contract
{
    interface IGraphModel
    {
        Task GetAllInfoTurbineForGraph();
    }
}
