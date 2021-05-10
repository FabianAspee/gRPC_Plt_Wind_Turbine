using ClientPltTurbine.Pages.Component.ModelPredictionComponent.EventModelPrediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
namespace ClientPltTurbine.Pages.Component.ModelPredictionComponent
{
    public partial class ModelPredictions:IEventModelPrediction
    {
        private readonly List<PltTurbineShared.Model> Models = new();
        private int NumModel;
        private async Task RunModel()
        {

        }
    }
}
