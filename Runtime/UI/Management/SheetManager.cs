using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace anogame.framework.UI
{
    /// <summary>
    /// シート表示管理マネージャー
    /// </summary>
    public class SheetManager : MonoSingleton<SheetManager>
    {
        private readonly Dictionary<string, List<ISheet>> _sheetGroups = new Dictionary<string, List<ISheet>>();
        private readonly Dictionary<string, ISheet> _activeSheets = new Dictionary<string, ISheet>();
        
        /// <summary>
        /// シート変更イベント（グループID, 前のシート, 新しいシート）
        /// </summary>
        public event Action<string, ISheet, ISheet> OnSheetChanged;
        
        /// <summary>
        /// シートをグループに登録する
        /// </summary>
        /// <param name="groupId">グループID</param>
        /// <param name="sheet">登録するシート</param>
        public void RegisterSheet(string groupId, ISheet sheet)
        {
            if (sheet == null)
            {
                Debug.LogError("[SheetManager] 登録するSheetがnullです");
                return;
            }
            
            if (!_sheetGroups.ContainsKey(groupId))
            {
                _sheetGroups[groupId] = new List<ISheet>();
            }
            
            if (_sheetGroups[groupId].Contains(sheet))
            {
                Debug.LogWarning($"[SheetManager] Sheet '{sheet.SheetId}' は既にグループ '{groupId}' に登録されています");
                return;
            }
            
            _sheetGroups[groupId].Add(sheet);
            Debug.Log($"[SheetManager] Sheet '{sheet.SheetId}' をグループ '{groupId}' に登録しました");
        }
        
        /// <summary>
        /// シートの登録を解除する
        /// </summary>
        /// <param name="groupId">グループID</param>
        /// <param name="sheetId">解除するシートID</param>
        public void UnregisterSheet(string groupId, string sheetId)
        {
            if (_sheetGroups.TryGetValue(groupId, out var sheets))
            {
                var sheet = sheets.FirstOrDefault(s => s.SheetId == sheetId);
                if (sheet != null)
                {
                    sheets.Remove(sheet);
                    
                    // アクティブなシートだった場合は非アクティブにする
                    if (_activeSheets.TryGetValue(groupId, out var activeSheet) && activeSheet == sheet)
                    {
                        _activeSheets.Remove(groupId);
                        sheet.Deactivate();
                    }
                    
                    Debug.Log($"[SheetManager] Sheet '{sheetId}' をグループ '{groupId}' から削除しました");
                }
            }
        }
        
        /// <summary>
        /// 指定したグループのシートをアクティブにする
        /// </summary>
        /// <param name="groupId">グループID</param>
        /// <param name="sheetId">アクティブにするシートID</param>
        public void ActivateSheet(string groupId, string sheetId)
        {
            if (!_sheetGroups.TryGetValue(groupId, out var sheets))
            {
                Debug.LogError($"[SheetManager] グループ '{groupId}' が見つかりません");
                return;
            }
            
            var targetSheet = sheets.FirstOrDefault(s => s.SheetId == sheetId);
            if (targetSheet == null)
            {
                Debug.LogError($"[SheetManager] グループ '{groupId}' にSheet '{sheetId}' が見つかりません");
                return;
            }
            
            var previousSheet = GetActiveSheet(groupId);
            
            // 現在アクティブなシートを非アクティブにする
            if (previousSheet != null && previousSheet != targetSheet)
            {
                previousSheet.Deactivate();
            }
            
            // 新しいシートをアクティブにする
            _activeSheets[groupId] = targetSheet;
            targetSheet.Activate();
            
            OnSheetChanged?.Invoke(groupId, previousSheet, targetSheet);
            Debug.Log($"[SheetManager] グループ '{groupId}' のSheet '{sheetId}' をアクティブにしました");
        }
        
        /// <summary>
        /// 指定したグループの次のシートをアクティブにする
        /// </summary>
        /// <param name="groupId">グループID</param>
        public void ActivateNextSheet(string groupId)
        {
            if (!_sheetGroups.TryGetValue(groupId, out var sheets) || sheets.Count == 0)
                return;
                
            var currentSheet = GetActiveSheet(groupId);
            var currentIndex = currentSheet != null ? sheets.IndexOf(currentSheet) : -1;
            var nextIndex = (currentIndex + 1) % sheets.Count;
            
            ActivateSheet(groupId, sheets[nextIndex].SheetId);
        }
        
        /// <summary>
        /// 指定したグループの前のシートをアクティブにする
        /// </summary>
        /// <param name="groupId">グループID</param>
        public void ActivatePreviousSheet(string groupId)
        {
            if (!_sheetGroups.TryGetValue(groupId, out var sheets) || sheets.Count == 0)
                return;
                
            var currentSheet = GetActiveSheet(groupId);
            var currentIndex = currentSheet != null ? sheets.IndexOf(currentSheet) : -1;
            var previousIndex = currentIndex <= 0 ? sheets.Count - 1 : currentIndex - 1;
            
            ActivateSheet(groupId, sheets[previousIndex].SheetId);
        }
        
        /// <summary>
        /// 指定したグループのアクティブなシートを取得する
        /// </summary>
        /// <param name="groupId">グループID</param>
        /// <returns>アクティブなシート</returns>
        public ISheet GetActiveSheet(string groupId)
        {
            _activeSheets.TryGetValue(groupId, out var activeSheet);
            return activeSheet;
        }
        
        /// <summary>
        /// 指定したグループの全シートを取得する
        /// </summary>
        /// <param name="groupId">グループID</param>
        /// <returns>シートのリスト</returns>
        public IReadOnlyList<ISheet> GetSheetsInGroup(string groupId)
        {
            if (_sheetGroups.TryGetValue(groupId, out var sheets))
            {
                return sheets.AsReadOnly();
            }
            return new List<ISheet>().AsReadOnly();
        }
        
        /// <summary>
        /// 全グループの全シートを非アクティブにする
        /// </summary>
        public void DeactivateAllSheets()
        {
            foreach (var kvp in _activeSheets.ToList())
            {
                kvp.Value.Deactivate();
            }
            _activeSheets.Clear();
            Debug.Log("[SheetManager] 全てのシートを非アクティブにしました");
        }
        
        /// <summary>
        /// 指定したグループの全シートを非アクティブにする
        /// </summary>
        /// <param name="groupId">グループID</param>
        public void DeactivateGroupSheets(string groupId)
        {
            if (_activeSheets.TryGetValue(groupId, out var activeSheet))
            {
                activeSheet.Deactivate();
                _activeSheets.Remove(groupId);
                Debug.Log($"[SheetManager] グループ '{groupId}' のシートを非アクティブにしました");
            }
        }
        
        /// <summary>
        /// グループ情報をログ出力
        /// </summary>
        [ContextMenu("Debug Sheet Groups")]
        public void DebugSheetGroups()
        {
            foreach (var kvp in _sheetGroups)
            {
                var activeSheet = GetActiveSheet(kvp.Key);
                Debug.Log($"[SheetManager] Group '{kvp.Key}': " +
                         $"Sheets=[{string.Join(", ", kvp.Value.Select(s => s.SheetId))}], " +
                         $"Active={activeSheet?.SheetId ?? "None"}");
            }
        }
    }
} 