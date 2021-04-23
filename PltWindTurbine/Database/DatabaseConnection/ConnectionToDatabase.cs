using Microsoft.EntityFrameworkCore;
using PltWindTurbine.Database.TableDatabase;
using PltWindTurbine.Database.Utils;
using SQLitePCL;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace PltWindTurbine.Database.DatabaseConnection
{
    public class ConnectionToDatabase  : DbContext
    { 
        private static readonly DbContextOptionsBuilder<ConnectionToDatabase> Options = new(); 
        private static readonly string PathDb = ConfigurationManager.AppSettings["configpath"];
        private static readonly string PathConfigMysql = ConfigurationManager.AppSettings["mysqlinfo"];  
        private static readonly object lockObject = new();
        private ConnectionToDatabase() { }
        private ConnectionToDatabase(DbContextOptions<ConnectionToDatabase> options) : base(options) { }
       
        public static ConnectionToDatabase CreateSqliteConnection()
        {  
            var contextOptions = Options.UseSqlite(ConnectionStringSqlite()).Options;
            return new ConnectionToDatabase(contextOptions);
        }
        public static ConnectionToDatabase CreateMysqlConnection()
        {
            var contextOptions = new DbContextOptionsBuilder<ConnectionToDatabase>().UseMySQL(ConnectionStringMySql()).Options;
            return new ConnectionToDatabase(contextOptions);
        }
        private static string ConnectionStringSqlite()
        {
            CreateDbSqlite();
            return $"Data Source={ReadFiles.CombinePath(PathDb)}";//mirar aqui poorque se rmpe per concorrenza
        }
        private static string ConnectionStringMySql()
        {
            var mysql = ReadFiles.ReadJsonAndDeserialize<MysqlInfo>(ReadFiles.CombinePath(PathConfigMysql));
            return $"server={mysql.Host};database={mysql.Database};user={mysql.User};password={mysql.Password}";
        }
        private static void CreateDbSqlite()
        {
            lock (lockObject)
            {
                if (!File.Exists(ReadFiles.CombinePath(PathDb)))
                {
                    File.Create(ReadFiles.CombinePath(PathDb)).Close();
                }
            } 
        }
        public DbSet<Error_Code> Error_Code { get; set; }
        public DbSet<Error_Sensor> Error_Sensor { get; set; }
        public DbSet<Sensor_Info> Sensor_Info { get; set; }
        public DbSet<Value_Sensor_Error> Value_Sensor_Error { get; set; }
        public DbSet<Value_Sensor_Turbine> Value_Sensor_Turbine { get; set; }
        public DbSet<Wind_Turbine_Info> Wind_Turbine_Info { get; set; }
    }
} 
