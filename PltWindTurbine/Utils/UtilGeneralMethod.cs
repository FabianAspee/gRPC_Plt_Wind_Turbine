using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PltWindTurbine.Utils
{
    public static class UtilGeneralMethod
    {
        public static ByteString GetBytes(string values) => ByteString.CopyFrom(Encoding.UTF8.GetBytes(values));
    }
}
