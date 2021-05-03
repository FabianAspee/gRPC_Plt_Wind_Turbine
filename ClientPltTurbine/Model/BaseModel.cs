using ClientPltTurbine.EventContainer;
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
    public abstract class BaseModel:EventHandlerSystem, IDisposable
    {
        protected GrpcChannel channel = CreatedGrpcChannel();
        
        protected static GrpcChannel CreatedGrpcChannel()
        {
            Console.WriteLine(ConfigurationManager.ConnectionStrings["serverPath"].ConnectionString);
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

        void IDisposable.Dispose()
        {
            try
            {
                GC.SuppressFinalize(this);
                channel.ShutdownAsync();
                channel.Dispose();
            }
            catch
            {
                channel.Dispose();
            }
        }
    }
}
