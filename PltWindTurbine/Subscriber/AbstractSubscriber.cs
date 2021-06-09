using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber
{
    public abstract class AbstractSubscriber:EventHandlerSystem
    { 

        private readonly CancellationTokenSource _cancellationTokenSource;
        public AbstractSubscriber()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public void Dispose()
        {
            _cancellationTokenSource.Cancel(); 
        }
    }
}
