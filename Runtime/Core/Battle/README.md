# 戦闘システム (Battle System)

このドキュメントでは、anogameフレームワークの戦闘システムについて説明します。

## 概要

戦闘システムは、ターン制戦闘を実現するための包括的なシステムです。既存のエフェクトシステムと完全に統合され、スキル、アイテム、通常攻撃などの様々な行動を統一的に処理できます。

## 主要な機能

- ✅ **ターン制戦闘システム** - 行動順序決定と順次実行
- ✅ **柔軟な行動システム** - 攻撃、スキル、アイテム、防御、逃走
- ✅ **直感的なUI** - HP/MPバー、行動選択、対象選択
- ✅ **AIシステム** - 敵キャラクター用の行動決定AI
- ✅ **イベントシステム** - 戦闘の各段階でイベント通知
- ✅ **エフェクト統合** - 既存のエフェクトシステムとの完全統合

## アーキテクチャ

### コアクラス

| クラス名               | 説明                                     |
| ---------------------- | ---------------------------------------- |
| `BattleManager`        | 戦闘全体を統括するメインクラス           |
| `BattleParticipant`    | 戦闘参加者（プレイヤー・敵）を表すクラス |
| `BattleAction`         | 戦闘での行動を定義するクラス             |
| `ActionQueue`          | 行動順序を管理するクラス                 |
| `BattleActionExecutor` | 行動を実行する静的クラス                 |

### UI関連

| クラス名    | 説明                                |
| ----------- | ----------------------------------- |
| `BattleUI`  | 戦闘UIのメインクラス                |
| `HPMPBar`   | HP/MPバーを表示するUIコンポーネント |
| `IBattleUI` | 戦闘UI用インターフェース            |

### その他

| クラス名              | 説明                     |
| --------------------- | ------------------------ |
| `BattleSystemExample` | 使用例とテスト用クラス   |
| `SimpleAI`            | 簡単な敵AI実装例         |
| `IBattleAI`           | 戦闘AI用インターフェース |

## 使用方法

### 1. 基本的なセットアップ

```csharp
// BattleManagerをシーンに配置
BattleManager battleManager = FindObjectOfType<BattleManager>();

// 戦闘参加者を作成
var players = new List<BattleParticipant>();
var enemies = new List<BattleParticipant>();

// プレイヤーキャラクターを作成
var playerStatus = new CharacterStatus
{
    BaseMaxHP = 100,
    BaseAttack = 15,
    BaseDefense = 8,
    BaseSpeed = 10
};
var player = new BattleParticipant(ParticipantType.Player, playerStatus, "プレイヤー");

// 敵キャラクターを作成
var enemyStatus = new CharacterStatus
{
    BaseMaxHP = 80,
    BaseAttack = 12,
    BaseDefense = 5,
    BaseSpeed = 8
};
var enemy = new BattleParticipant(ParticipantType.Enemy, enemyStatus, "敵")
{
    AI = new SimpleAI() // AIを設定
};

players.Add(player);
enemies.Add(enemy);

// 戦闘開始
battleManager.StartBattle(players, enemies);
```

### 2. スキルの追加

```csharp
// スキルを作成（ScriptableObject）
SkillDefinition healSkill = CreateInstance<SkillDefinition>();
healSkill.SkillName = "ヒール";
healSkill.Effects = new IEffect[] { new HealEffect { amount = 30 } };

// プレイヤーにスキルを追加
player.AddSkill(healSkill);
```

### 3. カスタムAIの作成

```csharp
public class CustomAI : IBattleAI
{
    public BattleAction DecideAction(BattleParticipant self, 
        List<BattleParticipant> allies, 
        List<BattleParticipant> enemies)
    {
        // カスタムロジック
        if (self.CharacterStatus.CurrentHP < self.CharacterStatus.MaxHP * 0.3f)
        {
            // HP低下時は防御
            return new BattleAction(BattleActionType.Guard, self);
        }
        
        // それ以外は攻撃
        var target = enemies.Find(e => e.IsAlive);
        return new BattleAction(BattleActionType.Attack, self, target);
    }
}
```

### 4. 戦闘イベントの監視

```csharp
battleManager.OnStateChanged += (state) => 
{
    Debug.Log($"戦闘状態: {state}");
};

battleManager.OnActionExecuted += (action) => 
{
    Debug.Log($"{action.Actor.Name} が {action.ActionType} を実行");
};

battleManager.OnBattleEnded += (result) => 
{
    Debug.Log($"戦闘結果: {result}");
};
```

## UI設定

### 必要なUI要素

戦闘UIを正しく動作させるには、以下のUI要素を設定する必要があります：

1. **行動選択パネル**
   - 攻撃ボタン
   - スキルボタン
   - アイテムボタン
   - 防御ボタン

2. **対象選択パネル**
   - 対象ボタンの親オブジェクト
   - 対象ボタンのプレハブ

3. **ステータス表示**
   - プレイヤーステータスの親オブジェクト
   - 敵ステータスの親オブジェクト
   - HP/MPバーのプレハブ

4. **情報表示**
   - ターン表示テキスト
   - 状態表示テキスト
   - ログ表示テキスト

### HP/MPバーの設定

```csharp
// HP/MPバーをキャラクターに関連付け
HPMPBar statusBar = GetComponent<HPMPBar>();
statusBar.SetTarget(character.CharacterStatus);

// エフェクト表示
statusBar.ShowDamageEffect(); // ダメージ時
statusBar.ShowHealEffect();   // 回復時
```

## 拡張方法

### 新しい行動タイプの追加

```csharp
// BattleActionType列挙型に新しい値を追加
public enum BattleActionType
{
    Attack,
    Skill,
    Item,
    Guard,
    Escape,
    NewActionType // 新しい行動タイプ
}

// BattleActionExecutorに処理を追加
case BattleActionType.NewActionType:
    ExecuteNewAction(action);
    break;
```

### カスタムエフェクトとの統合

戦闘システムは既存のエフェクトシステムと完全に統合されているため、新しいエフェクトを作成するだけで戦闘で使用できます。

```csharp
public class CustomBattleEffect : IEffect
{
    public void Apply(EffectContext context)
    {
        // カスタム効果の実装
    }
}
```

## テスト・デバッグ

### BattleSystemExampleの使用

`BattleSystemExample`クラスを使用して、戦闘システムを簡単にテストできます：

1. シーンに`BattleSystemExample`を配置
2. `Start Battle On Start`をチェック
3. テスト用スキルを設定
4. プレイして戦闘をテスト

### デバッグ情報

戦闘中の詳細な情報はConsoleウィンドウで確認できます：

- 戦闘状態の変化
- 各行動の実行
- ダメージ・回復の詳細
- 戦闘結果

## 既知の制限事項

- アイテム使用機能は基本実装のみ
- 複雑なスキル対象選択（範囲攻撃など）は今後実装予定
- 戦闘アニメーションシステムは別途実装が必要

## トラブルシューティング

### よくある問題

1. **UIが表示されない**
   - BattleUIコンポーネントの設定を確認
   - 必要なUI要素が正しく参照されているか確認

2. **行動が実行されない**
   - BattleActionExecutorのswitch文を確認
   - 対象が有効かどうか確認

3. **AIが動作しない**
   - IBattleAIが正しく実装されているか確認
   - BattleParticipant.AIが設定されているか確認

## 今後の拡張予定

- MP消費システムの実装
- 戦闘アニメーション統合
- より高度なAIシステム
- 複数対象スキルの対象選択UI
- 戦闘リプレイ機能 