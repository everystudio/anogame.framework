namespace anogame.framework
{
    /// <summary>
    /// エフェクトを持つ要素の共通インターフェース
    /// スキル、アイテム、バフ定義などが実装
    /// </summary>
    public interface IEffectSource
    {
        /// <summary>
        /// この要素が持つエフェクトを取得
        /// </summary>
        /// <returns>エフェクトの配列</returns>
        IEffect[] GetEffects();
    }
} 