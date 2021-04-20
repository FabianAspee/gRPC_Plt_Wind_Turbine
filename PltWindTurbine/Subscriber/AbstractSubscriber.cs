using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber
{
    public class AbstractSubscriber
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
