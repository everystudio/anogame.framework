// BuffUtils.cs（共通処理として）
using System.Collections.Generic;

namespace anogame.framework
{
    public static class BuffUtils
    {
        public static void ApplyBuffs(IBuffSource source, CharacterStatus target)
        {
            foreach (var buffDef in source.GetBuffs())
            {
                if (buffDef == null) continue;
                target.AddBuff(buffDef); // ← MergePolicyに従って処理
            }
        }
    }
}
