using ClientPltTurbine.Model.GraphModel.Contract;
using ClientPltTurbine.Model.GraphModel.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.GraphicController
{
    public class GraphicController : BaseController, IGraphicController
    {
        private readonly IGraphModel GraphModel = new GraphModel();
        public Task GraphAllTurbines()
        {
           return GraphModel.GetAllInfoTurbineForGraph();
        }
    }
}
