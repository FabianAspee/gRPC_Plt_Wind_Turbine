using ClientPltTurbine.Controllers.GraphicController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.GraphicComponent
{
    public class Graphic
    {
        public int NumTurbine;
        private readonly GraphicController Controller = new();
        public async Task GraphicInfoTurbine()
        {
            Task<int> s= (Task<int>)Controller.GraphAllTurbines();
            await s.ContinueWith(x => NumTurbine = x.Result);
        }
    }
}
