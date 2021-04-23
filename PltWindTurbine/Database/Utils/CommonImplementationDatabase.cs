using Microsoft.EntityFrameworkCore;
using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Database.TableDatabase;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Data;
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
            Console.WriteLine($"Inserendo {infoByTurbine.IdTurbine}"); 
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            using var transaction = connection.Database.BeginTransaction();
            using var command = connection.Database.GetDbConnection().CreateCommand();


            var columnsWithoutUnion = infoByTurbine.BaseInfoTurbine.Select(keyValue => keyValue.Key);
            var columns = columnsWithoutUnion.Union(
                new List<string>() { infoByTurbine.IdTurbine.nameColumnT, infoByTurbine.IdSensor.nameColumnS }).ToArray();
            var columnsValues = columns.Select(name => $"${name}");
            command.CommandText = $@"INSERT INTO value_sensor_turbine({string.Join(",", columns)})  VALUES ({string.Join(",", columnsValues)})";
            command.Parameters.AddRange(columnsValues.Select(column =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = column;
                return parameter;
            }).ToArray());
            var idTurbine = infoByTurbine.IdTurbine.idTurbine.ToString();
            var idSensor = infoByTurbine.IdSensor.idSensor.ToString();
            Enumerable.Range(0, infoByTurbine.BaseInfoTurbine.Values.First().Count).ToList().ForEach(index =>
            {
                var row = columnsWithoutUnion.Select(key => infoByTurbine.BaseInfoTurbine[key][index]).Append(idTurbine).Append(idSensor);
                row.Zip(columnsValues).ToList().ForEach(valueWithColumn =>
                {
                    object t = valueWithColumn.Second is not "$value" ? valueWithColumn.First : (valueWithColumn.First is null ? DBNull.Value : valueWithColumn.First);
                    object t2 = valueWithColumn.First is null ? DBNull.Value : valueWithColumn.First;
                    command.Parameters[valueWithColumn.Second].Value = valueWithColumn.Second is not "$value" ? valueWithColumn.First : (valueWithColumn.First is null ? DBNull.Value : valueWithColumn.First);
                });
                command.ExecuteNonQuery();
            });
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
        private static async IAsyncEnumerable<ILoadInfoTurbine> GenerateSequence(OnlySerieByPeriodAndCode info)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase(); 
            foreach (var infoError in connectionTo.Value_Sensor_Error.Where(error => error.Value == Convert.ToDouble(info.Code) && error.Id_Turbine == info.IdTurbine))
            {   
                yield return new StatusEventInfoTurbine(infoError.Id_Turbine.ToString(),Status.InProgress,"Init Search Temporary Data");
                var resultSerie = await connectionTo.Value_Sensor_Turbine.Where(infoSensor => infoSensor.Id_Turbine == info.IdTurbine && infoSensor.Id_Sensor == 1 &&
                   string.Compare(infoSensor.Date, infoError.Date) < 0 && string.Compare(infoSensor.Date, DateTime.Parse(infoError.Date).AddMonths(info.Months).ToString("yyyy/MM/dd HH:mm:ss")) > 0)
                    .ToListAsync();
                yield return new ResponseSerieByPeriod(infoError.Id_Turbine.ToString(),JsonSerializer.Serialize(resultSerie),true); 
            } 
            
        }
        public async void SelectSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info)
        {  
            await foreach(var values in GenerateSequence(info))
            {
                if (values is StatusEventInfoTurbine evento)
                {

                    SendEventLoadInfoTurbine(evento);
                }
                else if (values is ResponseSerieByPeriod serie)
                { 
                    SendEventLoadInfo(serie);
                }
            } 
            /*  select* from(select wt.id, wt.turbine_name, vs.date,
   (case when vs.value in (180, 3370, 186, 182, 181) then concat(vs.value," ", (select group_concat(war.value) from value_sensor_error as war
   where war.id_turbine = 15 and war.value in (892.0, 891.0, 183.0, 79.0, 356.0) and war.date<vs.date and 
STR_TO_DATE(war.date, '%Y/%m/%d %H:%i:%S')>DATE_SUB((DATE_SUB(STR_TO_DATE(vs.date, '%Y/%m/%d %H:%i:%S'), INTERVAL 1 MONTH)), INTERVAL 8 DAY))) else NULL end) as error
from value_sensor_error as vs,wind_turbine_info as wt where vs.id_turbine =15 and wt.id = 15 and
(vs.value in (180, 3370, 186, 182, 181))) as t where t.error != 'NULL'*/ 
        }

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
    }
}
