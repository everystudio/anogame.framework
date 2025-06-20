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
        private readonly Dictionary<string, List<ISheet>> sheetGroups = new Dictionary<string, List<ISheet>>();
        private readonly Dictionary<string, ISheet> activeSheets = new Dictionary<string, ISheet>();
        
        /// <summary>
        /// シート切り替えイベント（グループ名, 新しいシート, 前のシート）
        /// </summary>
        public event Action<string, ISheet, ISheet> OnSheetChanged;
        
        /// <summary>
        /// シートを登録する
        /// </summary>
        /// <param name="sheet">登録するシート</param>
        /// <param name="groupName">所属するグループ名</param>
        public void RegisterSheet(ISheet sheet, string groupName)
        {
            if (sheet == null)
            {
                Debug.LogError("[SheetManager] 登録するSheetがnullです");
                return;
            }
            
            if (string.IsNullOrEmpty(groupName))
            {
                Debug.LogError("[SheetManager] グループ名が指定されていません");
                return;
            }
            
            // グループが存在しない場合は作成
            if (!sheetGroups.ContainsKey(groupName))
            {
                sheetGroups[groupName] = new List<ISheet>();
            }
            
            // 既に登録されているかチェック
            if (sheetGroups[groupName].Contains(sheet))
            {
                Debug.LogWarning($"[SheetManager] Sheet '{sheet.SheetId}' は既にグループ '{groupName}' に登録されています");
                return;
            }
            
            sheetGroups[groupName].Add(sheet);
            Debug.Log($"[SheetManager] Sheet '{sheet.SheetId}' をグループ '{groupName}' に登録しました");
        }
        
        /// <summary>
        /// シートの登録を解除する
        /// </summary>
        /// <param name="sheet">解除するシート</param>
        /// <param name="groupName">所属するグループ名</param>
        public void UnregisterSheet(ISheet sheet, string groupName)
        {
            if (sheet == null) return;
            
            if (sheetGroups.TryGetValue(groupName, out var sheets))
            {
                sheets.Remove(sheet);
                
                // アクティブなシートだった場合は解除
                if (activeSheets.TryGetValue(groupName, out var activeSheet) && activeSheet == sheet)
                {
                    activeSheets.Remove(groupName);
                }
                
                Debug.Log($"[SheetManager] Sheet '{sheet.SheetId}' のグループ '{groupName}' からの登録を解除しました");
            }
        }
        
        /// <summary>
        /// 指定したシートをアクティブにする
        /// </summary>
        /// <param name="sheetId">アクティブにするシートID</param>
        /// <param name="groupName">所属するグループ名</param>
        public void ActivateSheet(string sheetId, string groupName)
        {
            var sheet = GetSheet(sheetId, groupName);
            if (sheet == null) return;
            
            var previousSheet = activeSheets.TryGetValue(groupName, out var prev) ? prev : null;
            
            // 前のシートを非アクティブに
            if (previousSheet != null)
            {
                previousSheet.Deactivate();
            }
            
            // 新しいシートをアクティブに
            sheet.Activate();
            activeSheets[groupName] = sheet;
            
            OnSheetChanged?.Invoke(groupName, sheet, previousSheet);
            Debug.Log($"[SheetManager] グループ '{groupName}' で Sheet '{sheetId}' をアクティブにしました");
        }
        
        /// <summary>
        /// グループ内の次のシートをアクティブにする
        /// </summary>
        /// <param name="groupName">グループ名</param>
        public void ActivateNextSheet(string groupName)
        {
            if (!sheetGroups.TryGetValue(groupName, out var sheets) || sheets.Count == 0)
            {
                Debug.LogWarning($"[SheetManager] グループ '{groupName}' にシートが登録されていません");
                return;
            }
            
            var currentSheet = activeSheets.TryGetValue(groupName, out var active) ? active : null;
            var currentIndex = currentSheet != null ? sheets.IndexOf(currentSheet) : -1;
            var nextIndex = (currentIndex + 1) % sheets.Count;
            
            var nextSheet = sheets[nextIndex];
            ActivateSheet(nextSheet.SheetId, groupName);
        }
        
        /// <summary>
        /// グループ内の前のシートをアクティブにする
        /// </summary>
        /// <param name="groupName">グループ名</param>
        public void ActivatePreviousSheet(string groupName)
        {
            if (!sheetGroups.TryGetValue(groupName, out var sheets) || sheets.Count == 0)
            {
                Debug.LogWarning($"[SheetManager] グループ '{groupName}' にシートが登録されていません");
                return;
            }
            
            var currentSheet = activeSheets.TryGetValue(groupName, out var active) ? active : null;
            var currentIndex = currentSheet != null ? sheets.IndexOf(currentSheet) : -1;
            var prevIndex = currentIndex <= 0 ? sheets.Count - 1 : currentIndex - 1;
            
            var prevSheet = sheets[prevIndex];
            ActivateSheet(prevSheet.SheetId, groupName);
        }
        
        /// <summary>
        /// 指定グループの現在アクティブなシートを取得
        /// </summary>
        /// <param name="groupName">グループ名</param>
        /// <returns>アクティブなシート</returns>
        public ISheet GetActiveSheet(string groupName)
        {
            activeSheets.TryGetValue(groupName, out var sheet);
            return sheet;
        }
        
        /// <summary>
        /// 指定グループのシート一覧を取得
        /// </summary>
        /// <param name="groupName">グループ名</param>
        /// <returns>シート一覧</returns>
        public IReadOnlyList<ISheet> GetSheets(string groupName)
        {
            return sheetGroups.TryGetValue(groupName, out var sheets) ? sheets : new List<ISheet>();
        }
        
        /// <summary>
        /// 指定グループのシートをすべて非アクティブにする
        /// </summary>
        /// <param name="groupName">グループ名</param>
        public void DeactivateAllSheets(string groupName)
        {
            if (activeSheets.TryGetValue(groupName, out var activeSheet))
            {
                activeSheet.Deactivate();
                activeSheets.Remove(groupName);
                Debug.Log($"[SheetManager] グループ '{groupName}' の全シートを非アクティブにしました");
            }
        }
        
        /// <summary>
        /// 登録されているシートを取得する
        /// </summary>
        /// <param name="sheetId">シートID</param>
        /// <param name="groupName">グループ名</param>
        /// <returns>シートインスタンス</returns>
        private ISheet GetSheet(string sheetId, string groupName)
        {
            if (!sheetGroups.TryGetValue(groupName, out var sheets))
            {
                Debug.LogError($"[SheetManager] グループ '{groupName}' が見つかりません");
                return null;
            }
            
            var sheet = sheets.Find(s => s.SheetId == sheetId);
            if (sheet == null)
            {
                Debug.LogError($"[SheetManager] グループ '{groupName}' にSheet '{sheetId}' が見つかりません");
            }
            
            return sheet;
        }
        
        /// <summary>
        /// シート状態をログ出力
        /// </summary>
        [ContextMenu("Debug Sheet State")]
        public void DebugSheetState()
        {
            Debug.Log($"[SheetManager] Sheet Groups: {sheetGroups.Count}");
            foreach (var group in sheetGroups)
            {
                var activeSheet = GetActiveSheet(group.Key);
                var activeSheetId = activeSheet?.SheetId ?? "None";
                Debug.Log($"  Group '{group.Key}': {group.Value.Count} sheets, Active: {activeSheetId}");
            }
        }
        
        /// <summary>
        /// シートグループの詳細情報をログ出力
        /// </summary>
        [ContextMenu("Debug Sheet Groups")]
        public void DebugSheetGroups()
        {
            Debug.Log($"[SheetManager] === シートグループ詳細情報 ===");
            Debug.Log($"総グループ数: {sheetGroups.Count}");
            Debug.Log($"アクティブシート数: {activeSheets.Count}");
            
            foreach (var group in sheetGroups)
            {
                var groupName = group.Key;
                var sheets = group.Value;
                var activeSheet = GetActiveSheet(groupName);
                
                Debug.Log($"グループ '{groupName}':");
                Debug.Log($"  - 登録シート数: {sheets.Count}");
                Debug.Log($"  - アクティブシート: {activeSheet?.SheetId ?? "None"}");
                
                for (int i = 0; i < sheets.Count; i++)
                {
                    var sheet = sheets[i];
                    var isActive = sheet == activeSheet;
                    Debug.Log($"    [{i}] {sheet.SheetId} {(isActive ? "(Active)" : "")}");
                }
            }
        }
        
        /// <summary>
        /// 全グループのシートを非アクティブにする
        /// </summary>
        public void DeactivateAllSheets()
        {
            foreach (var groupName in activeSheets.Keys.ToList())
            {
                DeactivateAllSheets(groupName);
            }
            Debug.Log("[SheetManager] 全グループの全シートを非アクティブにしました");
        }
        
        /// <summary>
        /// 全グループの登録を解除
        /// </summary>
        public void UnregisterAllSheets()
        {
            sheetGroups.Clear();
            activeSheets.Clear();
            Debug.Log("[SheetManager] 全てのシートの登録を解除しました");
        }
    }
} 