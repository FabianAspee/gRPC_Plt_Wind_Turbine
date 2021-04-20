using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.DatabaseConnection
{
    public class MysqlInfo
    {
        public MysqlInfo(string host, string database, string user, string password, int port)
        {
            Host = host;
            Database = database;
            User = user;
            Password = password;
            Port = port;
        }

        public string Host { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
