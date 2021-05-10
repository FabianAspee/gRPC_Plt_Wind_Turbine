using Microsoft.EntityFrameworkCore;
using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Database.TableDatabase;
using PltWindTurbine.Database.Utils.EqualityComparerElement;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Utils
{
    public abstract class CommonImplementationDatabase : EventHandlerSystem, IOperationTurbineDatabase
    {
        public void InsertInfoPlt(DataTable dt_info, string name_table)
        {
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            using var transaction = connection.Database.BeginTransaction();
            using var command = connection.Database.GetDbConnection().CreateCommand();
            var columns = dt_info.SelectAllColumn(column => $"${column.ColumnName}").ToArray();
            command.CommandText = $@"INSERT INTO {name_table}({string.Join(",", dt_info.SelectAllColumn(x => x.ColumnName))})  VALUES ({string.Join(",", columns)})";

            command.Parameters.AddRange(columns.Select(column => {
                var parameter = command.CreateParameter();
                parameter.ParameterName = column;
                return parameter;
            }).ToArray());

            dt_info.AsEnumerable().ToList().ForEach(row => {
                row.ItemArray.Zip(columns).ToList().ForEach(valueWithColumn => {
                    command.Parameters[valueWithColumn.Second].Value = valueWithColumn.First;
                });
                command.ExecuteNonQuery();
            });
            transaction.Commit();
        } 
         
        public virtual void InsertInfoWindTurbine(InfoByTurbineToTable infoByTurbine)
        {
            SendEventFile(infoByTurbine.IdTurbine.ToString(),$"Insert sensor info {infoByTurbine.IdSensor}"); 
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            using var transaction = connection.Database.BeginTransaction();
            using var command = connection.Database.GetDbConnection().CreateCommand(); 
            GetDbCommand(command, infoByTurbine, "value_sensor_turbine");
            transaction.Commit(); 
        }
        private static void GetDbCommand(DbCommand command, InfoByTurbineToTable infoByTurbine, string nameTable)
        {
            var columnsWithoutUnion = infoByTurbine.BaseInfoTurbine.Select(keyValue => keyValue.Key);
            var columns = columnsWithoutUnion.Union(
                new List<string>() { infoByTurbine.IdTurbine.nameColumnT, infoByTurbine.IdSensor.nameColumnS }).ToArray();
            var columnsValues = columns.Select(name => $"${name}");
            command.CommandText = $@"INSERT INTO {nameTable}({string.Join(",", columns)})  VALUES ({string.Join(",", columnsValues)})";
            command.Parameters.AddRange(columnsValues.Select(column =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = column;
                return parameter;
            }).ToArray());
            (string idTurbine, string idSensor) = GetIdTurbineAndSensor(infoByTurbine);
            Enumerable.Range(0, infoByTurbine.BaseInfoTurbine.Values.First().Count).ToList().ForEach(index =>
            {
                var row = columnsWithoutUnion.Select(key => infoByTurbine.BaseInfoTurbine[key][index]).Append(idTurbine).Append(idSensor);
                row.Zip(columnsValues).ToList().ForEach(valueWithColumn =>
                command.Parameters[valueWithColumn.Second].Value = valueWithColumn.Second is not "$value" ? valueWithColumn.First : (valueWithColumn.First is null ? DBNull.Value : valueWithColumn.First));
                command.ExecuteNonQuery();
            });
        }
        private static (string, string) GetIdTurbineAndSensor(InfoByTurbineToTable infoByTurbine) => (infoByTurbine.IdTurbine.idTurbine.ToString(), infoByTurbine.IdSensor.idSensor.ToString());
        public virtual void InsertInfoEventWindTurbine(InfoByTurbineToTable infoByTurbine)
        {
            SendEventFile(infoByTurbine.IdTurbine.ToString(), $"Insert Event sensor info {infoByTurbine.IdSensor}"); 
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            using var transaction = connection.Database.BeginTransaction();
            using var command = connection.Database.GetDbConnection().CreateCommand();
            GetDbCommand(command, infoByTurbine, "value_sensor_error"); 
            transaction.Commit();
        }
        public List<Wind_Turbine_Info> ReadAllTurbine() 
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Wind_Turbine_Info.ToList();
        } 
        public DataTable ReadInfoByTurbine(string path, string nameFile)
        {
            throw new NotImplementedException();
        }

        public List<Sensor_Info> SelectAllNameSensor()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Sensor_Info.ToList();
        } 
 
        private readonly Func<Sensor_Info, SensorInfo> SelectNameAndIdSensor = sensor => new SensorInfo(sensor.Id, sensor.Sensor_Name);
        private readonly Func<Wind_Turbine_Info, TurbineInfo> SelectNameAndIdTurbine = turbine => new TurbineInfo(turbine.Id, turbine.Turbine_Name);
        public async void SelectAllSensorAndTurbine()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            await SendEventLoadInfoTurbine(new AllSensorInfo(connectionTo.Sensor_Info.Select(SelectNameAndIdSensor).ToList()));
            await SendEventLoadInfoTurbine(new AllTurbineInfo(connectionTo.Wind_Turbine_Info.Select(SelectNameAndIdTurbine).ToList()));
            await SendEventLoadInfoTurbine(new FinishMessage());
        }
        public List<Error_Sensor> SelectAllNameSensorError()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Error_Sensor.ToList();
        }
        
        public Dictionary<string, List<string>> SelectAllSerieBySensorByTurbineByError()
        {
            throw new NotImplementedException();
        }

        public List<Wind_Turbine_Info> SelectAllTurbineInfo()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Wind_Turbine_Info.ToList();
        }

        public List<string> SelectColumnFrom(string nameTable)
        {
            lock (this)
            { 
                return RetreiveImplementationDatabase.Instance.GetConnectionToDatabase().Model.GetRelationalModel().
                Tables.FirstOrDefault(table => table.Name.Equals(nameTable))?.Columns.Select(column => column.Name.ToLower()).ToList();
            }
        }
        public DataTable SelectErrorTableByTurbine()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectErrorTurbineByCondition()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectPivotValueSensorByTurbine()
        {
            throw new NotImplementedException();
        }
        private static List<(Value_Sensor_Error values, DateTime dates)> GetDateBetweenValues(List<Value_Sensor_Error> values, int period)
        {
            List<(Value_Sensor_Error values, DateTime dates)> _GetDateBetweenValues(List<Value_Sensor_Error> valuesRemaining, List<(Value_Sensor_Error values, DateTime dates)> valueAndDates) => valuesRemaining switch
            {
                (Value_Sensor_Error head, Value_Sensor_Error head2, List<Value_Sensor_Error> tail) =>_GetDateBetweenValues(tail.Prepend(head2).ToList(), valueAndDates.Append((head,DateTime.Parse(head2.Date).AddSeconds(5))).ToList()),
                (Value_Sensor_Error head, Value_Sensor_Error head2, _)=> _GetDateBetweenValues(new List<Value_Sensor_Error>() { head2}, valueAndDates.Append((head, DateTime.Parse(head2.Date).AddSeconds(5))).ToList()),
                (Value_Sensor_Error head, _) => valueAndDates.Append((head, DateTime.Parse(head.Date).AddMonths(period))).ToList()
            };

            return _GetDateBetweenValues(values, new List<(Value_Sensor_Error, DateTime)>());
        }

        private async IAsyncEnumerable<ILoadInfoTurbine> GenerateSequence(OnlySerieByPeriodAndCode info, bool isWarning=false)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var values = connectionTo.Value_Sensor_Error.Where(error => error.Id_Turbine == info.IdTurbine && error.Value ==info.Code).OrderByDescending(value =>value.Date).ToList(); 
            var last = values.LastOrDefault();
            yield return new StatusEventInfoTurbine(info.NameTurbine, Status.InProgress, $"Init Search Temporary Data with {values.Count} precence of error {info.Code}");
            var newValuesWithDate = values.Count>1? GetDateBetweenValues(values, info.Months):values.Select(val=>(val, DateTime.Parse(values.First().Date).AddMonths(info.Months))).ToList();
            foreach (var (infoError, dates) in newValuesWithDate)
            {   
                var resultSerie = await connectionTo.Value_Sensor_Turbine.Where(infoSensor => infoSensor.Id_Turbine == info.IdTurbine && infoSensor.Id_Sensor == info.IdSensor &&
                   string.Compare(infoSensor.Date, infoError.Date) < 0 && string.Compare(infoSensor.Date, dates.ToString("yyyy/MM/dd HH:mm:ss")) > 0)
                    .Select(values=>new SerieBySensorTurbineError(values.Id,values.Date,values.Value)).ToListAsync();
               
                if (isWarning)
                {
                   var warning = await connectionTo.Value_Sensor_Error.Where(error =>error.Value.HasValue && error.Id_Turbine == info.IdTurbine &&
                   string.Compare(error.Date, infoError.Date) < 0 && string.Compare(error.Date, dates.ToString("yyyy/MM/dd HH:mm:ss")) > 0 )
                        .Select(values => new SerieBySensorTurbineWarning(values.Id, values.Date, values.Value)).ToListAsync();
                    warning = warning.OrderBy(val => val.Date).ToList();
                    warning = DateTimeRange(dates, ParserDateSpecificFormat(infoError.Date), 10, warning).ToList();  
                    var serieByPeriod = new ResponseSerieByPeriod(info.NameTurbine, info.NameSensor, JsonSerializer.Serialize(resultSerie), infoError.Id == last.Id); 
                    yield return new ResponseSerieByPeriodWithWarning(serieByPeriod, JsonSerializer.Serialize(warning), JsonSerializer.Serialize(warnings));
                }
                else
                { 
                    yield return new ResponseSerieByPeriod(info.NameTurbine, info.NameSensor, JsonSerializer.Serialize(resultSerie), infoError.Id == last.Id);
                }
            } 
            
        }

      
        private static string ValidationFormatData(string date) => DateTime.Parse(date).ToString("yyyy/MM/dd HH:mm:ss");
        private static DateTime ParserDateSpecificFormat(string date) => DateTime.Parse(DateTime.Parse(date).ToString("yyyy/MM/dd HH:mm:ss"));
        private static IEnumerable<SerieBySensorTurbineWarning> DateTimeRange(DateTime start, DateTime end, int delta, List<SerieBySensorTurbineWarning> serieBySensors)
        {
            var current = start.Minute%10==0?start: start.AddMinutes(-(start.Minute % 10));
            current =  current.AddSeconds(-(current.Second));
            var id = 0;
            while (current < end)
            {

                if (serieBySensors.Count > 0 && Math.Abs((current - ParserDateSpecificFormat(ValidationFormatData(serieBySensors.First().Date))).TotalSeconds / 60) <= 5)
                {
                    var result = serieBySensors.First();
                    serieBySensors = serieBySensors.Skip(1).ToList();
                    yield return result;
                }
                else
                {
                    var currentAux = current;
                    current = currentAux.AddMinutes(delta);
                    yield return new SerieBySensorTurbineWarning(id++,currentAux.ToString("yyyy/MM/dd HH:mm:ss"), double.Parse("0"));

                }
            }

        } 

        public async Task CallSelectSeries(OnlySerieByPeriodAndCode info, bool isWarning = false)
        {
            await foreach (var values in GenerateSequence(info, isWarning))
            { 
                SendEventLoadInfo(values); 
            }
        }
        public async void SelectSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info) => await CallSelectSeries(info);
       

        public async void SelectSerieBySensorByTurbineByErrorWithWarning(OnlySerieByPeriodAndCode info) => await CallSelectSeries(info, true); 

        private readonly IReadOnlyList<double> errors = new List<double> { 180, 3370, 186, 182, 181 };
        private readonly IReadOnlyList<double> warnings = new List<double> { 892, 891, 183, 79, 356 };
        public Task<List<string>> GetErrorByTurbine(int idTurbine) => Task.Run(() =>
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Value_Sensor_Error.Where(turbine=>turbine.Id_Turbine==idTurbine && turbine.Value.HasValue &&
            errors.Contains(turbine.Value.Value))
            .GroupBy(value=>value.Value)
            .Select(value =>value.Key.ToString()).ToList(); 
            
        });
        
        public Dictionary<string, List<string>> SelectSerieTurbineByError()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectValueSensorByTurbine()
        {
            throw new NotImplementedException();
        }

        public (List<string>, bool) TurbineExistInDatabase()
        {
            throw new NotImplementedException();
        }
        public Task<List<(int, string)>> GetNameChart()=>Task.Run(() =>
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Chart_System.ToList().Select(chart=>(chart.Id,chart.Chart_Name)).ToList(); 
        });

        public virtual async Task SelectWarningAllTurbines(int period)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var allTurbines = await connectionTo.Wind_Turbine_Info.Select(turbine=> new TurbineInfo(turbine.Id,turbine.Turbine_Name)).ToListAsync();
            await foreach(var infoTurbine in SelectWarningAllTurbineByPeriod(allTurbines))
            {

            }
        }

        private IAsyncEnumerable<object> SelectWarningAllTurbineByPeriod(List<TurbineInfo> allTurbines)
        {
            throw new NotImplementedException();
        }
    }
}
