using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using Grpc.Core;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model
{
    public class CommonMethodModel:BaseModel,ICommonMethodModel
    {
        private ObtainInfoTurbines.ObtainInfoTurbinesClient _clientChart;
        private void RecreateClient() => _clientChart = new ObtainInfoTurbines.ObtainInfoTurbinesClient(channel);
        public Task GetAllNameSensor()
        {
            return Task.Run(async () =>
            {
                RecreateClient();
                using var call = _clientChart.GetNameTurbineAndSensor(new WithoutMessage());
                while (await call.ResponseStream.MoveNext())
                {
                    ResponseNameTurbineAndSensor response = call.ResponseStream.Current;
                    HandleResponsesInfoTurbineSensorAsync(response);
                }
            });
        }
        public Task GetAllNameTurbine()
        {
            return Task.Run(async () =>
            {
                RecreateClient();
                using var call = _clientChart.GetNameTurbineAndSensor(new WithoutMessage());
                while (await call.ResponseStream.MoveNext())
                {
                    ResponseNameTurbineAndSensor response = call.ResponseStream.Current;
                    HandleResponsesInfoTurbineSensorAsync(response);
                }
            });
        }
        private void HandleResponsesInfoTurbineSensorAsync(ResponseNameTurbineAndSensor turbineSensor)
        {
            switch (turbineSensor.ActionCase)
            {
                case ResponseNameTurbineAndSensor.ActionOneofCase.None:
                    SendEventErrorLoadInfoTurbine("No Action specified.");
                    break;
                case ResponseNameTurbineAndSensor.ActionOneofCase.Msg4:
                    SendEventInfoTurbineAndSensor(new AllTurbineInfo(turbineSensor.Msg4.Msg.Select(turbine => new TurbineInfo(turbine.IdTurbine, turbine.NameTurbine)).ToList()));
                    break;
                case ResponseNameTurbineAndSensor.ActionOneofCase.Msg3:
                    SendEventInfoTurbineAndSensor(new AllSensorInfo(turbineSensor.Msg3.Msg.Select(sensor => new SensorInfo(sensor.IdSensor, sensor.NameSensor, sensor.IsOwn)).ToList()));
                    break;
                default:
                    SendEventErrorLoadInfoTurbine($"Unknown Action '{turbineSensor.ActionCase}'.");
                    break;
            }

        }
    }
}
