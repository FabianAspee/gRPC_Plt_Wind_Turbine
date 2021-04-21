using Microsoft.EntityFrameworkCore;
using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Database.TableDatabase;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Utils
{
    public abstract class CommonImplementationDatabase : IOperationTurbineDatabase
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
         
        public virtual Task InsertInfoWindTurbine(InfoByTurbineToTable infoByTurbine)
        { 
            return Task.Run(() =>
            { 
                Console.WriteLine($"Inserendo {infoByTurbine.IdTurbine}");
                lock (this)
                {
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
            }); 
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
