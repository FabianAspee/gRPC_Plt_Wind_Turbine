using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PltTurbineShared.ExtensionMethodDataTable
{
    public static class ExtensionMethodDataTable
    {
        public static IEnumerable<string> SelectAllColumn(this DataTable data, Func<DataColumn, string> func) => data.Columns.OfType<DataColumn>().Select(func);
    }
}
