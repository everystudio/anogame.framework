// Packages/anogame.framework/Runtime/Service/ServiceContainer.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace anogame.framework
{
    public static class ServiceContainer
    {
        private static readonly Dictionary<Type, object> services = new();

        /// <summary>
        /// サービスを登録する
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} is already registered. Overwriting.");
            }
            services[type] = service;
        }

        /// <summary>
        /// サービスを取得する
        /// </summary>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return service as T;
            }
            return null;
        }

        /// <summary>
        /// サービスが登録されているかチェック
        /// </summary>
        public static bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// サービスの登録を解除する
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            services.Remove(typeof(T));
        }

        /// <summary>
        /// 全てのサービスをクリア
        /// </summary>
        public static void Clear()
        {
            services.Clear();
        }
    }
}


