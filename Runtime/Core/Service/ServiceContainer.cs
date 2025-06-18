// Packages/anogame.framework/Runtime/Service/ServiceContainer.cs
using System;
using System.Collections.Generic;

namespace anogame.framework
{
    public static class ServiceContainer
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// サービスを登録します。
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                UnityEngine.Debug.LogWarning($"Service of type {type.Name} is already registered. Overwriting.");
            }
            _services[type] = service;
        }

        /// <summary>
        /// サービスを取得します。
        /// </summary>
        public static T Resolve<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            UnityEngine.Debug.LogError($"Service of type {type.Name} is not registered.");
            return null;
        }

        /// <summary>
        /// サービスを解除します。
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }

        /// <summary>
        /// すべてのサービスをクリアします。
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }
    }
}
