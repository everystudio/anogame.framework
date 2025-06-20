# UI管理システム

anogameフレームワークの階層的UI管理システムです。Page/Sheet/Modalの3つの概念で構成されています。

## 概要

### UI要素の種類

- **Page**: 画面遷移の基本単位。戻る機能があり、スタック形式で管理されます。
- **Sheet**: Page内やModal内でタブ切り替えなどに使用される単位。
- **Modal**: ポップアップ表示される要素。重ね表示が可能で、LIFO（後入れ先出し）で閉じられます。

### 管理システム

- **UIManager**: 全体を統合管理するメインマネージャー
- **PageManager**: ページ遷移を管理（スタック形式）
- **SheetManager**: シート表示を管理（グループ単位）
- **ModalManager**: モーダル表示を管理（スタック形式）

## 基本的な使用方法

### 1. UIManagerの初期化

```csharp
// UIManagerは自動的にシングルトンとして初期化されます
var uiManager = UIManager.Instance;

// 初期化完了イベントの購読
uiManager.OnUIInitialized += () => {
    Debug.Log("UI system initialized!");
};
```

### 2. Pageの作成と使用

```csharp
// PageBaseを継承してカスタムページを作成
public class MainMenuPage : PageBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("メインメニューに入りました");
    }
    
    public override void OnExit()
    {
        Debug.Log("メインメニューから出ます");
        base.OnExit();
    }
}

// ページの登録と遷移
PageManager.Instance.RegisterPage(mainMenuPage);
PageManager.Instance.NavigateToPage("MainMenu");
```

### 3. Modalの作成と使用

```csharp
// ModalBaseを継承してカスタムモーダルを作成
public class SettingsModal : ModalBase
{
    public override void OnOpen()
    {
        base.OnOpen();
        Debug.Log("設定モーダルを開きました");
    }
    
    public override void OnClose()
    {
        Debug.Log("設定モーダルを閉じました");
        base.OnClose();
    }
}

// モーダルの登録と開閉
ModalManager.Instance.RegisterModal(settingsModal);
ModalManager.Instance.OpenModal("Settings");
ModalManager.Instance.CloseModal("Settings");
```

### 4. Sheetの作成と使用

```csharp
// SheetBaseを継承してカスタムシートを作成
public class InventorySheet : SheetBase
{
    public override void OnActivate()
    {
        base.OnActivate();
        Debug.Log("インベントリシートがアクティブになりました");
    }
    
    public override void OnDeactivate()
    {
        Debug.Log("インベントリシートが非アクティブになりました");
        base.OnDeactivate();
    }
}

// シートの登録とアクティブ化
SheetManager.Instance.RegisterSheet("MainTabs", inventorySheet);
SheetManager.Instance.ActivateSheet("MainTabs", "Inventory");
```

## 高度な使用方法

### ページ間の遷移パターン

```csharp
// スタックをクリアして新しいページに遷移
PageManager.Instance.NavigateToPage("MainMenu");

// 現在のページの上に新しいページを重ねる
PageManager.Instance.PushPage("Settings");

// 前のページに戻る
PageManager.Instance.PopPage();
```

### モーダルの重ね表示

```csharp
// モーダルを順次開く（スタック形式）
ModalManager.Instance.OpenModal("FirstModal");
ModalManager.Instance.OpenModal("SecondModal");
ModalManager.Instance.OpenModal("ThirdModal");

// 最上位のモーダルを閉じる（ThirdModal）
ModalManager.Instance.CloseTopModal();

// 指定したモーダルとその上のモーダルを全て閉じる
ModalManager.Instance.CloseModal("FirstModal"); // First, Second, Third全て閉じる
```

### シートのグループ管理

```csharp
// 複数のシートを同じグループに登録
SheetManager.Instance.RegisterSheet("MainTabs", inventorySheet);
SheetManager.Instance.RegisterSheet("MainTabs", equipmentSheet);
SheetManager.Instance.RegisterSheet("MainTabs", statusSheet);

// グループ内でアクティブシートを切り替え
SheetManager.Instance.ActivateSheet("MainTabs", "Inventory");
SheetManager.Instance.ActivateSheet("MainTabs", "Equipment");

// タブ形式の切り替え
SheetManager.Instance.ActivateNextSheet("MainTabs");     // 次のシート
SheetManager.Instance.ActivatePreviousSheet("MainTabs"); // 前のシート
```

## イベント処理

### ページ遷移イベント

```csharp
PageManager.Instance.OnPageChanged += (prevPage, currentPage) => {
    Debug.Log($"ページ変更: {prevPage?.PageId} -> {currentPage?.PageId}");
};
```

### モーダル開閉イベント

```csharp
ModalManager.Instance.OnModalStateChanged += (modal, isOpen) => {
    Debug.Log($"モーダル {modal.ModalId} が{(isOpen ? "開" : "閉")}じました");
};
```

### シート切り替えイベント

```csharp
SheetManager.Instance.OnSheetChanged += (groupId, prevSheet, currentSheet) => {
    Debug.Log($"[{groupId}] シート変更: {prevSheet?.SheetId} -> {currentSheet?.SheetId}");
};
```

## 入力処理

UIManagerは自動的に以下の入力を処理します：

- **ESCキー**: モーダル > ページの順で戻る処理を実行
- **Androidの戻るボタン**: ESCキーと同じ処理

```csharp
// 設定で無効化することも可能
UIManager.Instance.SetBackButtonEnabled(false);
UIManager.Instance.SetEscapeKeyEnabled(false);
```

## デバッグ機能

各マネージャーにはデバッグ用の機能が用意されています：

```csharp
// UI全体の状態を出力
UIManager.Instance.DebugUIState();

// 個別のデバッグ
PageManager.Instance.DebugPageStack();
ModalManager.Instance.DebugModalStack();
SheetManager.Instance.DebugSheetGroups();
```

## 使用例

実際の使用例は `Runtime/UI/Examples/` フォルダにあります：

- `ExamplePage.cs`: ページの実装例
- `ExampleModal.cs`: モーダルの実装例
- `ExampleSheet.cs`: シートの実装例

## 注意事項

1. **初期化順序**: UIManagerが他のマネージャーより先に初期化されるようにしてください。
2. **メモリ管理**: UI要素の登録解除を忘れずに行ってください。
3. **イベント購読**: イベントの購読解除も適切に行ってください。

## カスタマイズ

各基底クラス（`PageBase`, `ModalBase`, `SheetBase`）を継承して、独自の動作を実装できます。

```csharp
public class CustomPage : PageBase
{
    public override bool OnBackPressed()
    {
        // カスタムの戻る処理
        if (HasUnsavedChanges())
        {
            ShowSaveConfirmationDialog();
            return false; // 戻る処理をキャンセル
        }
        return true; // 通常の戻る処理を実行
    }
} 