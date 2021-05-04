using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PltWindTurbine.Database.Utils;
using System;
using System.Configuration;

namespace PltWindTurbine.Database.DatabaseConnection
{
    public class RetreiveImplementationDatabase
    {
        public ConnectionToDatabase connectionToDatabase;
        private static readonly Lazy<RetreiveImplementationDatabase> instance = new(() => new RetreiveImplementationDatabase());
        private bool IsSqlite = false;
        public ConnectionToDatabase GetConnectionToDatabase() => IsSqlite? ConnectionToDatabase.CreateSqliteConnection(): ConnectionToDatabase.CreateMysqlConnection();
        private string DataSource { get; set; }
        private RetreiveImplementationDatabase() { }
        private bool isVerified=false;
        public static RetreiveImplementationDatabase Instance
        {
            get { return instance.Value; }
        }
        private static readonly object lockObject = new();
        public bool IsMysql() => DataSource.Contains("Sqlite");
        public CommonImplementationDatabase ImplementationDatabase=> SelectDatabase() ? new Mysql.TurbineCrud() : Sqlite.TurbineCrud.Instance;

       private bool SelectDatabase()
       {
            lock (lockObject)
            {
                using var sqlite = ConnectionToDatabase.CreateSqliteConnection();
                if (sqlite.Database.CanConnect())
                { 
                    DataSource = sqlite.Database.ProviderName; 
                    if(!isVerified){ 
                        VerifyTableDatabase(sqlite, "sqlitescript");
                        isVerified=true;
                    }
                    IsSqlite = true;
                    return false;
                }
                else
                {
                    using var mysql = ConnectionToDatabase.CreateMysqlConnection();
                    if (mysql.Database.CanConnect())
                    {
                        DataSource = mysql.Database.ProviderName; 
                        return true;
                    }
                    else
                    { 
                        VerifyTableDatabase(mysql, "mysqlscript");
                    }
                    throw new InvalidOperationException();
                }
            } 
             
       } 
       private static void VerifyTableDatabase(ConnectionToDatabase sqlInstance, string key)
       {
            var sql = System.IO.File.ReadAllText(ReadFiles.CombinePath(ConfigurationManager.AppSettings[key])); 
            sqlInstance.Database.ExecuteSqlRaw(sql); 
       }
    }
}
