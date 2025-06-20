using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// モーダル管理システム
    /// </summary>
    public class ModalManager : MonoSingleton<ModalManager>
    {
        // テンプレート管理（ModalId -> プレハブ/テンプレート）
        private readonly Dictionary<string, IModal> modalTemplates = new Dictionary<string, IModal>();
        // アクティブなインスタンス管理（InstanceId -> インスタンス）
        private readonly Dictionary<string, IModal> activeModals = new Dictionary<string, IModal>();
        private readonly Stack<IModal> modalStack = new Stack<IModal>();

        [SerializeField] private Canvas modalCanvas;
        [SerializeField] private int baseSortOrder = 1000;

        /// <summary>
        /// 現在最前面のモーダル
        /// </summary>
        public IModal TopModal => modalStack.Count > 0 ? modalStack.Peek() : null;

        /// <summary>
        /// モーダル状態変更イベント
        /// </summary>
        public event Action<IModal, bool> OnModalStateChanged;

        protected override void OnInitialize()
        {
            // モーダル用Canvasが設定されていない場合は自動作成
            if (modalCanvas == null)
            {
                CreateModalCanvas();
            }
        }

        /// <summary>
        /// モーダルテンプレートを登録する
        /// </summary>
        /// <param name="modal">登録するモーダルテンプレート</param>
        public void RegisterModal(IModal modal)
        {
            if (modal == null)
            {
                Debug.LogError("[ModalManager] 登録するModalがnullです");
                return;
            }

            if (modalTemplates.ContainsKey(modal.ModalId))
            {
                Debug.LogWarning($"[ModalManager] Modal template '{modal.ModalId}' は既に登録されています");
                return;
            }

            modalTemplates[modal.ModalId] = modal;

            // テンプレートを非アクティブにして隠す
            modal.Hide();

            // モーダルをCanvasの子にする
            if (modalCanvas != null && modal.GameObject != null)
            {
                modal.GameObject.transform.SetParent(modalCanvas.transform, false);
            }

            Debug.Log($"[ModalManager] Modal template '{modal.ModalId}' を登録しました");
        }

        /// <summary>
        /// モーダルテンプレートの登録を解除する
        /// </summary>
        /// <param name="modalId">解除するモーダルID</param>
        public void UnregisterModal(string modalId)
        {
            if (modalTemplates.TryGetValue(modalId, out var modal))
            {
                modalTemplates.Remove(modalId);

                // このModalIdのアクティブなインスタンスをすべて閉じる
                var instancesToClose = activeModals.Values
                    .Where(m => m.ModalId == modalId)
                    .ToList();

                foreach (var instance in instancesToClose)
                {
                    CloseModalInstance(instance.InstanceId);
                }

                Debug.Log($"[ModalManager] Modal template '{modalId}' の登録を解除しました");
            }
        }

        /// <summary>
        /// モーダルを開く（基本版）
        /// </summary>
        /// <param name="modalId">開くモーダルID</param>
        /// <returns>作成されたモーダルインスタンス</returns>
        public IModal OpenModal(string modalId)
        {
            return OpenModal(modalId, null);
        }

        /// <summary>
        /// モーダルを開く（パラメータ付き）
        /// </summary>
        /// <param name="modalId">開くモーダルID</param>
        /// <param name="setupAction">モーダル初期化アクション</param>
        /// <returns>作成されたモーダルインスタンス</returns>
        public IModal OpenModal(string modalId, Action<IModal> setupAction)
        {
            if (!modalTemplates.TryGetValue(modalId, out var template))
            {
                Debug.LogError($"[ModalManager] Modal template '{modalId}' が見つかりません");
                return null;
            }

            // 新しいインスタンスを作成
            var modalInstance = CreateModalInstance(template);
            if (modalInstance == null) return null;

            // セットアップアクションを実行
            setupAction?.Invoke(modalInstance);

            // アクティブなモーダルとして管理
            activeModals[modalInstance.InstanceId] = modalInstance;

            // 閉じる要求イベントを購読
            modalInstance.OnCloseRequested += OnModalCloseRequested;

            // ソート順序を設定
            modalInstance.SortOrder = baseSortOrder + modalStack.Count;

            // スタックに追加
            modalStack.Push(modalInstance);

            // モーダルを開く
            modalInstance.Open();

            OnModalStateChanged?.Invoke(modalInstance, true);
            Debug.Log($"[ModalManager] Modal '{modalId}' インスタンス '{modalInstance.InstanceId}' を開きました（深度: {modalStack.Count}）");

            return modalInstance;
        }

        /// <summary>
        /// モーダルインスタンスを作成
        /// </summary>
        /// <param name="template">テンプレート</param>
        /// <returns>作成されたインスタンス</returns>
        private IModal CreateModalInstance(IModal template)
        {
            if (template.GameObject == null)
            {
                Debug.LogError($"[ModalManager] Modal template '{template.ModalId}' のGameObjectがnullです");
                return null;
            }

            // プレハブをインスタンス化
            var instanceObj = Instantiate(template.GameObject, modalCanvas.transform);
            var modalInstance = instanceObj.GetComponent<IModal>();

            if (modalInstance == null)
            {
                Debug.LogError($"[ModalManager] インスタンス化されたオブジェクトにIModalコンポーネントがありません");
                Destroy(instanceObj);
                return null;
            }

            // 新しいインスタンスIDを生成
            if (modalInstance is ModalBase modalBase)
            {
                modalBase.GenerateNewInstanceId();
            }

            return modalInstance;
        }

        /// <summary>
        /// 最上位のモーダルを閉じる
        /// </summary>
        /// <returns>閉じることができたかどうか</returns>
        public bool CloseTopModal()
        {
            if (modalStack.Count == 0)
            {
                Debug.LogWarning("[ModalManager] 閉じるモーダルがありません");
                return false;
            }

            var topModal = modalStack.Peek();
            return CloseModalInstance(topModal.InstanceId);
        }

        /// <summary>
        /// 指定したインスタンスIDのモーダルを閉じる
        /// </summary>
        /// <param name="instanceId">閉じるモーダルのインスタンスID</param>
        /// <returns>閉じることができたかどうか</returns>
        public bool CloseModalInstance(string instanceId)
        {
            if (!activeModals.TryGetValue(instanceId, out var modal))
            {
                Debug.LogWarning($"[ModalManager] インスタンス '{instanceId}' は開いていません");
                return false;
            }

            // 閉じる前の確認
            if (!modal.OnClosing())
            {
                Debug.Log($"[ModalManager] Modal '{modal.ModalId}' インスタンス '{instanceId}' の閉じる処理がキャンセルされました");
                return false;
            }

            // スタックから該当モーダル以降を全て削除
            var tempStack = new Stack<IModal>();
            var targetFound = false;

            while (modalStack.Count > 0)
            {
                var m = modalStack.Pop();
                if (m.InstanceId == instanceId)
                {
                    targetFound = true;
                    break;
                }
                tempStack.Push(m);
            }

            if (targetFound)
            {
                // 対象モーダルを閉じる
                CloseModalInstanceInternal(modal);

                // 上に重なっていたモーダルも閉じる
                while (tempStack.Count > 0)
                {
                    var m = tempStack.Pop();
                    CloseModalInstanceInternal(m);
                    Debug.Log($"[ModalManager] Modal '{m.ModalId}' インスタンス '{m.InstanceId}' も併せて閉じました");
                }

                Debug.Log($"[ModalManager] Modal '{modal.ModalId}' インスタンス '{instanceId}' を閉じました");
                return true;
            }

            return false;
        }

        /// <summary>
        /// モーダルインスタンスの内部的な閉じる処理
        /// </summary>
        /// <param name="modal">閉じるモーダル</param>
        private void CloseModalInstanceInternal(IModal modal)
        {
            // イベント購読解除
            modal.OnCloseRequested -= OnModalCloseRequested;

            // アクティブリストから削除
            activeModals.Remove(modal.InstanceId);

            // モーダルを閉じる
            modal.Close();

            // インスタンスを破棄
            if (modal.GameObject != null)
            {
                Destroy(modal.GameObject);
            }

            OnModalStateChanged?.Invoke(modal, false);
        }

        /// <summary>
        /// 指定したModalIdのモーダルをすべて閉じる
        /// </summary>
        /// <param name="modalId">閉じるモーダルID</param>
        public void CloseAllModalsOfType(string modalId)
        {
            var instancesToClose = activeModals.Values
                .Where(m => m.ModalId == modalId)
                .ToList();

            foreach (var instance in instancesToClose)
            {
                CloseModalInstance(instance.InstanceId);
            }
        }

        /// <summary>
        /// 全てのモーダルを閉じる
        /// </summary>
        public void CloseAllModals()
        {
            while (modalStack.Count > 0)
            {
                var modal = modalStack.Pop();
                CloseModalInstanceInternal(modal);
            }
            Debug.Log("[ModalManager] 全てのモーダルを閉じました");
        }

        /// <summary>
        /// ESCキーが押された時の処理
        /// </summary>
        public void OnEscapeKeyPressed()
        {
            var topModal = TopModal;
            if (topModal != null && topModal.CanCloseByEscapeKey)
            {
                CloseModalInstance(topModal.InstanceId);
            }
        }

        /// <summary>
        /// モーダルからの閉じる要求を処理
        /// </summary>
        /// <param name="modal">閉じる要求を出したモーダル</param>
        private void OnModalCloseRequested(IModal modal)
        {
            CloseModalInstance(modal.InstanceId);
        }

        /// <summary>
        /// モーダル用Canvasを作成
        /// </summary>
        private void CreateModalCanvas()
        {
            var canvasObj = new GameObject("ModalCanvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.layer = LayerMask.NameToLayer("UI");

            modalCanvas = canvasObj.AddComponent<Canvas>();
            modalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            modalCanvas.sortingOrder = baseSortOrder;

            var canvasScaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            Debug.Log("[ModalManager] モーダル用Canvasを作成しました");
        }

        /// <summary>
        /// モーダルスタックの状態をログ出力
        /// </summary>
        [ContextMenu("Debug Modal Stack")]
        public void DebugModalStack()
        {
            Debug.Log($"[ModalManager] Modal Stack: {string.Join(" -> ", modalStack.Select(m => $"{m.ModalId}({m.InstanceId[..8]})"))}");
            Debug.Log($"[ModalManager] Active Modals: {activeModals.Count}");
        }

        /// <summary>
        /// 現在開いているモーダル数を取得
        /// </summary>
        public int OpenModalCount => modalStack.Count;

        /// <summary>
        /// 全てのアクティブなモーダルを取得
        /// </summary>
        /// <returns>アクティブなモーダル一覧</returns>
        public IReadOnlyList<IModal> GetAllActiveModals()
        {
            return activeModals.Values.ToList();
        }

        /// <summary>
        /// 指定したModalIdのアクティブなモーダル一覧を取得
        /// </summary>
        /// <param name="modalId">モーダルID</param>
        /// <returns>該当するモーダル一覧</returns>
        public IReadOnlyList<IModal> GetActiveModalsOfType(string modalId)
        {
            return activeModals.Values
                .Where(m => m.ModalId == modalId)
                .ToList();
        }
    }
}