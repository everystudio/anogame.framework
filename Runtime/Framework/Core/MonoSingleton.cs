// Assets/Scripts/ano/core/MonoSingleton.cs
using UnityEngine;

namespace anogame.framework
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
        private void Awake()
        {
            // インスタンスの設定確認
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnInitialize();
            }
            else if (_instance != this)
            {
                // 既に他のインスタンスが存在する場合は削除
                Destroy(gameObject);
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
