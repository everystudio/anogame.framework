// Assets/Scripts/ano/core/MonoSingleton.cs
using UnityEngine;

namespace anogame.framework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static bool shuttingDown;

        public static T Instance
        {
            get
            {
                if (shuttingDown) return null;

                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();

                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// インスタンスが存在するかどうか
        /// </summary>
        public static bool HasInstance => instance != null && !shuttingDown;

        /// <summary>
        /// インスタンスを破棄
        /// </summary>
        public static void DestroyInstance()
        {
            if (instance != null)
            {
                DestroyImmediate(instance.gameObject);
                instance = null;
            }
        }

        /// <summary>
        /// シングルトンの初期化処理（継承先で実装）
        /// </summary>
        protected virtual void OnInitialize()
        {
            // 基底クラスでの初期化処理
        }
        
        /// <summary>
        /// Unity標準のAwake。OnInitializeを呼び出す
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                OnInitialize();
            }
            else if (instance != this)
            {
                Debug.LogWarning($"[MonoSingleton] {typeof(T).Name} の複数のインスタンスが検出されました。重複するインスタンスを削除します。");
                Destroy(gameObject);
            }
        }
        
        protected virtual void OnApplicationQuit() => shuttingDown = true;
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                shuttingDown = true;
            }
        }
    }
}
