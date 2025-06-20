namespace anogame.framework
{
    /// <summary>
    /// 戦闘の状態を表す列挙型
    /// </summary>
    public enum BattleState
    {
        /// <summary>戦闘開始前</summary>
        None,
        
        /// <summary>戦闘開始</summary>
        Starting,
        
        /// <summary>行動順決定中</summary>
        DeterminingOrder,
        
        /// <summary>プレイヤーの行動選択待ち</summary>
        WaitingForPlayerAction,
        
        /// <summary>敵の行動決定中</summary>
        EnemyActionDecision,
        
        /// <summary>行動実行中</summary>
        ExecutingAction,
        
        /// <summary>ターン終了処理中</summary>
        TurnEnd,
        
        /// <summary>戦闘勝利</summary>
        Victory,
        
        /// <summary>戦闘敗北</summary>
        Defeat,
        
        /// <summary>戦闘終了</summary>
        Ended
    }
} 