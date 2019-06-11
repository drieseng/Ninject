using System;
using System.Threading;

namespace Ninject.Tests.Fakes
{
    public class Stoppable : MarshalByRefObject, IStoppable
    {
        private int _stopCount;

        public int StopCount
        {
            get { return _stopCount; }
        }

        public void Stop()
        {
            Interlocked.Increment(ref _stopCount);
        }
    }
}
