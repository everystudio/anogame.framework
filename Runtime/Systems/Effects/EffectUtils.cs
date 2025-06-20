namespace anogame.framework
{
    /// <summary>
    /// エフェクト適用のためのユーティリティクラス
    /// </summary>
    public static class EffectUtils
    {
        /// <summary>
        /// エフェクトソースから全てのエフェクトを適用
        /// </summary>
        /// <param name="source">エフェクトソース</param>
        /// <param name="context">適用コンテキスト</param>
        public static void ApplyEffects(IEffectSource source, EffectContext context)
        {
            if (source == null || context == null) return;
            
            var effects = source.GetEffects();
            if (effects == null) return;
            
            foreach (var effect in effects)
            {
                effect?.Apply(context);
            }
        }
        
        /// <summary>
        /// 単一のエフェクトを適用
        /// </summary>
        /// <param name="effect">エフェクト</param>
        /// <param name="target">対象</param>
        /// <param name="source">発動者</param>
        /// <param name="multiplier">倍率</param>
        public static void ApplyEffect(IEffect effect, CharacterStatus target, CharacterStatus source = null, float multiplier = 1.0f)
        {
            if (effect == null || target == null) return;
            
            var context = new EffectContext(target, source, multiplier);
            effect.Apply(context);
        }
        
        /// <summary>
        /// 複数のエフェクトを適用
        /// </summary>
        /// <param name="effects">エフェクト配列</param>
        /// <param name="target">対象</param>
        /// <param name="source">発動者</param>
        /// <param name="multiplier">倍率</param>
        public static void ApplyEffects(IEffect[] effects, CharacterStatus target, CharacterStatus source = null, float multiplier = 1.0f)
        {
            if (effects == null || target == null) return;
            
            var context = new EffectContext(target, source, multiplier);
            foreach (var effect in effects)
            {
                effect?.Apply(context);
            }
        }
    }
} 