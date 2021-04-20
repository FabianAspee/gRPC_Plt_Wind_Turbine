using PltWindTurbine.Subscriber.SubscriberContract;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberFactory
{
    public interface ISubscriberFactory
    {
        ILoadFileSubscriber GetLoadFileSubscriber();
    }
}
