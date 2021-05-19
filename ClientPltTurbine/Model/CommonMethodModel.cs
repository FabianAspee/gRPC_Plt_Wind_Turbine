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
        //realizar TF entre direccion nacelle e direccion viento para ver que sucede
        private readonly AsyncDuplexStreamingCall<TurbineOrSensor, ResponseNameTurbineAndSensor> _duplexStreamTurbineSensor;
        protected ObtainInfoTurbines.ObtainInfoTurbinesClient _clientCommonInfo;
        public CommonMethodModel()
        {
            _clientCommonInfo = new ObtainInfoTurbines.ObtainInfoTurbinesClient(channel); 
            _duplexStreamTurbineSensor = _clientCommonInfo.GetNameTurbineAndSensor();
            _ = HandleResponsesInfoTurbineSensorAsync(); 
        }
        public Task GetAllNameSensors()
        {
            return Task.Run(() =>
            {
                var InfoSensor = new TurbineOrSensor()
                {
                    Msg2 = new Sensor()
                };
                return _duplexStreamTurbineSensor.RequestStream.WriteAsync(InfoSensor); 
            });
        }
        public Task GetAllNameTurbines()
        {
            return Task.Run(() =>
            {
                var InfoSensor = new TurbineOrSensor()
                {
                    Msg1 = new Turbine()
                };
                return _duplexStreamTurbineSensor.RequestStream.WriteAsync(InfoSensor);
            });
        }
        private async Task HandleResponsesInfoTurbineSensorAsync()
        {
            await foreach (var turbineSensor in _duplexStreamTurbineSensor.ResponseStream.ReadAllAsync())
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
}
