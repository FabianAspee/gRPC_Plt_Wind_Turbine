using ClientPltTurbine.Shared.Chart3DComponent.ConfigGeneral;
using ClientPltTurbine.Shared.Chart3DComponent.DrawCalendarChart.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ModelPredictionComponent
{
    public partial class ModelPredictions
    {
        private readonly List<PltTurbineShared.Model> Models = new();
        public int NumModel;
        private readonly ModelPrediction Prediction = new();
        public async Task RunModel()
        {
            await Prediction.RunModel();
        }
        private static ConfigChart3D GetConfig()
        {
            return new CalendarChart();
        } 
    }
} 