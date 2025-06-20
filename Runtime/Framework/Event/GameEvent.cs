// Packages/anogame.framework/Runtime/Event/GameEvent.cs
using System;
using System.Collections.Generic;

namespace anogame.framework
{
    public static class GameEvent<T>
    {
        private static readonly List<Action<T>> listeners = new();

        public static void Subscribe(Action<T> listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public static void Unsubscribe(Action<T> listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }

        public static void Publish(T eventData)
        {
            foreach (var listener in listeners)
            {
                listener?.Invoke(eventData);
            }
        }

        public static void Clear()
        {
            listeners.Clear();
        }
    }
}
