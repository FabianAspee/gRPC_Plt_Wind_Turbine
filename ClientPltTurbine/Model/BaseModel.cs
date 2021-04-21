using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model
{
    public abstract class BaseModel
    {
        protected GrpcChannel channel = CreatedGrpcChannel();
        private static GrpcChannel CreatedGrpcChannel()
        {
            return GrpcChannel.ForAddress(ConfigurationManager.ConnectionStrings["serverPath"].ConnectionString, new GrpcChannelOptions
            {
                MaxSendMessageSize = 512 * 1024 * 1024,
                MaxReceiveMessageSize = 512 * 1024 * 1024,
                HttpHandler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),

                    EnableMultipleHttp2Connections = true
                }
            });
        }
    }
}
