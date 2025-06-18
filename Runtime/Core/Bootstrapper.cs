// Packages/anogame.framework/Runtime/Bootstrap/Bootstrapper.cs
using UnityEngine;

namespace anogame.framework
{
    public class Bootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("[Bootstrapper] Initializing services...");

            // 例：AudioService の登録（非MonoBehaviour）
            //ServiceContainer.Register<IAudioService>(new AudioService());

            // 例：GameEvent の初期化も可能（必要に応じて）
            //GameEvent<string>.Clear();

            // MonoBehaviour 系サービスの登録もここで Find や生成で可能
            // var uiManager = new GameObject("UIManager").AddComponent<UIManager>();
            // ServiceContainer.Register<IUIManager>(uiManager);
        }
    }
}
