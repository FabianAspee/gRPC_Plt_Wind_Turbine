using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Utils
{
    public static class MethodExtensionDataTable
    {
        public static IEnumerable<string> SelectAllColumn(this DataTable data, Func<DataColumn,string> func) => data.Columns.OfType<DataColumn>().Select(func);
    }
}
