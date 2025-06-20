using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// モーダル表示管理マネージャー
    /// </summary>
    public class ModalManager : MonoSingleton<ModalManager>
    {
        private readonly Dictionary<string, IModal> _registeredModals = new Dictionary<string, IModal>();
        private readonly Stack<IModal> _modalStack = new Stack<IModal>();
        
        [SerializeField] private Canvas _modalCanvas;
        [SerializeField] private int _baseSortOrder = 1000;
        
        /// <summary>
        /// 現在のトップモーダル
        /// </summary>
        public IModal TopModal => _modalStack.Count > 0 ? _modalStack.Peek() : null;
        
        /// <summary>
        /// モーダル開閉イベント（開いたモーダル, true=開く/false=閉じる）
        /// </summary>
        public event Action<IModal, bool> OnModalStateChanged;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // モーダル用のCanvasを作成
            if (_modalCanvas == null)
            {
                CreateModalCanvas();
            }
        }
        
        /// <summary>
        /// モーダルを登録する
        /// </summary>
        /// <param name="modal">登録するモーダル</param>
        public void RegisterModal(IModal modal)
        {
            if (modal == null)
            {
                Debug.LogError("[ModalManager] 登録するModalがnullです");
                return;
            }
            
            if (_registeredModals.ContainsKey(modal.ModalId))
            {
                Debug.LogWarning($"[ModalManager] Modal '{modal.ModalId}' は既に登録されています");
                return;
            }
            
            _registeredModals[modal.ModalId] = modal;
            
            // モーダル閉じる要求イベントを購読
            modal.OnCloseRequested += OnModalCloseRequested;
            
            // モーダルをCanvasの子にする
            if (_modalCanvas != null && modal.GameObject != null)
            {
                modal.GameObject.transform.SetParent(_modalCanvas.transform, false);
            }
            
            Debug.Log($"[ModalManager] Modal '{modal.ModalId}' を登録しました");
        }
        
        /// <summary>
        /// モーダルの登録を解除する
        /// </summary>
        /// <param name="modalId">解除するモーダルID</param>
        public void UnregisterModal(string modalId)
        {
            if (_registeredModals.TryGetValue(modalId, out var modal))
            {
                modal.OnCloseRequested -= OnModalCloseRequested;
                _registeredModals.Remove(modalId);
                
                // スタックからも削除
                if (_modalStack.Contains(modal))
                {
                    var tempStack = new Stack<IModal>();
                    while (_modalStack.Count > 0)
                    {
                        var m = _modalStack.Pop();
                        if (m != modal)
                        {
                            tempStack.Push(m);
                        }
                    }
                    while (tempStack.Count > 0)
                    {
                        _modalStack.Push(tempStack.Pop());
                    }
                }
                
                Debug.Log($"[ModalManager] Modal '{modalId}' の登録を解除しました");
            }
        }
        
        /// <summary>
        /// モーダルを開く
        /// </summary>
        /// <param name="modalId">開くモーダルID</param>
        public void OpenModal(string modalId)
        {
            var modal = GetModal(modalId);
            if (modal == null) return;
            
            // 既に開いている場合は何もしない
            if (_modalStack.Contains(modal))
            {
                Debug.LogWarning($"[ModalManager] Modal '{modalId}' は既に開いています");
                return;
            }
            
            // ソート順序を設定
            modal.SortOrder = _baseSortOrder + _modalStack.Count;
            
            // スタックに追加
            _modalStack.Push(modal);
            
            // モーダルを開く
            modal.Open();
            
            OnModalStateChanged?.Invoke(modal, true);
            Debug.Log($"[ModalManager] Modal '{modalId}' を開きました（深度: {_modalStack.Count}）");
        }
        
        /// <summary>
        /// 最上位のモーダルを閉じる
        /// </summary>
        /// <returns>閉じることができたかどうか</returns>
        public bool CloseTopModal()
        {
            if (_modalStack.Count == 0)
            {
                Debug.LogWarning("[ModalManager] 閉じるモーダルがありません");
                return false;
            }
            
            var topModal = _modalStack.Peek();
            return CloseModal(topModal.ModalId);
        }
        
        /// <summary>
        /// 指定したモーダルを閉じる
        /// </summary>
        /// <param name="modalId">閉じるモーダルID</param>
        /// <returns>閉じることができたかどうか</returns>
        public bool CloseModal(string modalId)
        {
            var modal = GetModal(modalId);
            if (modal == null) return false;
            
            // スタックに含まれていない場合は何もしない
            if (!_modalStack.Contains(modal))
            {
                Debug.LogWarning($"[ModalManager] Modal '{modalId}' は開いていません");
                return false;
            }
            
            // 閉じる前の確認
            if (!modal.OnClosing())
            {
                Debug.Log($"[ModalManager] Modal '{modalId}' の閉じる処理がキャンセルされました");
                return false;
            }
            
            // スタックから該当モーダル以降を全て削除
            var tempStack = new Stack<IModal>();
            var targetFound = false;
            
            while (_modalStack.Count > 0)
            {
                var m = _modalStack.Pop();
                if (m == modal)
                {
                    targetFound = true;
                    break;
                }
                tempStack.Push(m);
            }
            
            if (targetFound)
            {
                // 対象モーダルを閉じる
                modal.Close();
                OnModalStateChanged?.Invoke(modal, false);
                
                // 上に重なっていたモーダルも閉じる
                while (tempStack.Count > 0)
                {
                    var m = tempStack.Pop();
                    m.Close();
                    OnModalStateChanged?.Invoke(m, false);
                    Debug.Log($"[ModalManager] Modal '{m.ModalId}' も併せて閉じました");
                }
                
                Debug.Log($"[ModalManager] Modal '{modalId}' を閉じました");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 全てのモーダルを閉じる
        /// </summary>
        public void CloseAllModals()
        {
            while (_modalStack.Count > 0)
            {
                var modal = _modalStack.Pop();
                modal.Close();
                OnModalStateChanged?.Invoke(modal, false);
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
                CloseModal(topModal.ModalId);
            }
        }
        
        /// <summary>
        /// 登録されているモーダルを取得する
        /// </summary>
        /// <param name="modalId">モーダルID</param>
        /// <returns>モーダルインスタンス</returns>
        private IModal GetModal(string modalId)
        {
            if (!_registeredModals.TryGetValue(modalId, out var modal))
            {
                Debug.LogError($"[ModalManager] Modal '{modalId}' が見つかりません");
                return null;
            }
            return modal;
        }
        
        /// <summary>
        /// モーダルからの閉じる要求を処理
        /// </summary>
        /// <param name="modal">閉じる要求を出したモーダル</param>
        private void OnModalCloseRequested(IModal modal)
        {
            CloseModal(modal.ModalId);
        }
        
        /// <summary>
        /// モーダル用Canvasを作成
        /// </summary>
        private void CreateModalCanvas()
        {
            var canvasObj = new GameObject("ModalCanvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.layer = LayerMask.NameToLayer("UI");
            
            _modalCanvas = canvasObj.AddComponent<Canvas>();
            _modalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _modalCanvas.sortingOrder = _baseSortOrder;
            
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
            Debug.Log($"[ModalManager] Modal Stack: {string.Join(" -> ", _modalStack.Select(m => m.ModalId))}");
        }
        
        /// <summary>
        /// 現在開いているモーダル数を取得
        /// </summary>
        public int OpenModalCount => _modalStack.Count;
        
        /// <summary>
        /// 全てのモーダルの一覧を取得
        /// </summary>
        /// <returns>登録されているモーダルの一覧</returns>
        public IReadOnlyList<IModal> GetAllModals()
        {
            return _registeredModals.Values.ToList();
        }
    }
} 