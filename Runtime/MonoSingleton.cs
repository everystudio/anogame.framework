// Assets/Scripts/ano/core/MonoSingleton.cs
using UnityEngine;

namespace ano.core
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _shuttingDown;

        public static T Instance
        {
            get
            {
                if (_shuttingDown) return null;

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        var obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }

                return _instance;
            }
        }

        protected virtual void OnApplicationQuit() => _shuttingDown = true;
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _shuttingDown = true;
            }
        }
    }
}
