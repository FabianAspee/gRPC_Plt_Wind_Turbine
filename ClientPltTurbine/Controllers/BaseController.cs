using ClientPltTurbine.EventContainer;
using ClientPltTurbine.Model;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers
{
    public abstract class BaseController: EventHandlerSystem
    {
        private readonly CommonMethodModel model = new();
        protected Task GetAllNameTurbineAndSensor() => Task.Run(async () =>
        {
            var turbine = model.GetAllNameTurbine();
            var sensor = model.GetAllNameSensor();
            await Task.WhenAll(new Task[]{ turbine, sensor });
        });
        protected Task GetAllNameTurbine() => Task.Run(async () =>await model.GetAllNameTurbine());
        protected Task GetAllNameSensor() => Task.Run(async () =>  await model.GetAllNameSensor());
    }
}

