using System;
using System.Threading;

namespace Ninject.Tests.Fakes
{
    public class Startable : MarshalByRefObject, IStartable
    {
        private int _startCount;

        public int StartCount
        {
            get { return _startCount; }
        }

        public void Start()
        {
            Interlocked.Increment(ref _startCount);
        }
    }
}
