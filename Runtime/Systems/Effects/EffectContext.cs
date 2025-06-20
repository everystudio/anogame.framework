namespace anogame.framework
{
    /// <summary>
    /// エフェクト実行時のコンテキスト情報
    /// CharacterStatusへの直接依存を避け、将来的な拡張性を確保
    /// </summary>
    public class EffectContext
    {
        /// <summary>エフェクトの対象</summary>
        public CharacterStatus Target { get; }
        
        /// <summary>エフェクトの発動者（必要に応じて）</summary>
        public CharacterStatus Source { get; }
        
        /// <summary>エフェクトの強度倍率（スキルレベルやアイテム品質による補正）</summary>
        public float Multiplier { get; }
        
        public EffectContext(CharacterStatus target, CharacterStatus source = null, float multiplier = 1.0f)
        {
            Target = target;
            Source = source;
            Multiplier = multiplier;
        }
    }
} 