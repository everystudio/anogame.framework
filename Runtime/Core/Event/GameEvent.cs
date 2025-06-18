// Packages/anogame.framework/Runtime/Event/GameEvent.cs
using System;
using System.Collections.Generic;

namespace anogame.framework
{
    public static class GameEvent<T>
    {
        private static readonly List<Action<T>> _listeners = new();

        public static void Subscribe(Action<T> listener)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        public static void Unsubscribe(Action<T> listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }

        public static void Publish(T value)
        {
            foreach (var listener in _listeners)
            {
                listener?.Invoke(value);
            }
        }

        public static void Clear()
        {
            _listeners.Clear();
        }
    }
}
