using System.Collections.Concurrent;
using System.Threading;

namespace DefaultSerialization.Internal
{
    internal sealed class IntDispenser
    {
        private readonly ConcurrentStack<int> _freeInts;

        private int _lastInt;

        public int LastInt => _lastInt;

        public IntDispenser(int startInt)
        {
            _freeInts = new ConcurrentStack<int>();

            _lastInt = startInt;
        }

        public int GetFreeInt()
        {
            if (!_freeInts.TryPop(out int freeInt))
            {
                freeInt = Interlocked.Increment(ref _lastInt);
            }

            return freeInt;
        }

        public void ReleaseInt(int releasedInt) => _freeInts.Push(releasedInt);
    }
}
