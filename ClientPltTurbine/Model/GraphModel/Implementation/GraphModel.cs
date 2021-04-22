using ClientPltTurbine.Model.GraphModel.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.GraphModel.Implementation
{
    public class GraphModel : IGraphModel
    {
        public Task GetAllInfoTurbineForGraph()
        {
            return Task.FromResult(1);
        }
    }
}
