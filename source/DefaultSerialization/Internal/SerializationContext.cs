using System;

namespace DefaultSerialization.Internal
{
    internal static class SerializationContext
    {
        private static readonly IntDispenser _idDispenser;

        public static event Action<int> Disposed;

        static SerializationContext()
        {
            _idDispenser = new IntDispenser(-1);
        }

        public static int GetId() => _idDispenser.GetFreeInt();

        public static void ReleaseId(int id)
        {
            Disposed?.Invoke(id);
            _idDispenser.ReleaseInt(id);
        }
    }

    internal static class SerializationContext<T>
    {
        public struct SerializationActions
        {
            public Delegate ValueWrite;
            public Delegate ValueRead;
        }

        private static readonly object _lockObject;

        public static SerializationActions[] Actions;

        static SerializationContext()
        {
            _lockObject = new object();

            Actions = EmptyArray<SerializationActions>.Value;

            SerializationContext.Disposed += id =>
            {
                lock (_lockObject)
                {
                    if (id < Actions.Length)
                    {
                        Actions[id] = default;
                    }
                }
            };
        }

        public static void SetWriteActions(int contextId, Delegate value)
        {
            lock (_lockObject)
            {
                ArrayExtension.EnsureLength(ref Actions, contextId);

                Actions[contextId].ValueWrite = value;
            }
        }

        public static void SetReadActions(int contextId, Delegate value)
        {
            lock (_lockObject)
            {
                ArrayExtension.EnsureLength(ref Actions, contextId);

                Actions[contextId].ValueRead = value;
            }
        }
    }
}
