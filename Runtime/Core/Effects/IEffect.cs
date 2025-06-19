namespace anogame.framework
{
    /// <summary>
    /// 汎用エフェクトインターフェース
    /// スキル、アイテム、バフなど様々なシステムから使用可能
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// エフェクトを適用する
        /// </summary>
        /// <param name="context">エフェクト実行コンテキスト</param>
        void Apply(EffectContext context);
    }
} 