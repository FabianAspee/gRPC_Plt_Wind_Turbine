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
            await model.GetAllNameTurbines();
            await model.GetAllNameSensors(); 
        });
        protected Task GetAllNameTurbines() => Task.Run(async () =>await model.GetAllNameTurbines());
        protected Task GetAllNameSensors() => Task.Run(async () =>  await model.GetAllNameSensors());
    }
}

