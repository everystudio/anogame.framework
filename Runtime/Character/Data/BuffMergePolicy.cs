namespace anogame.framework
{
    public enum BuffMergePolicy
    {
        Override,   // 効果と残りターンを上書き
        Extend,     // 残りターンを延長（効果は変えない）
        Ignore      // 同一IDが既にある場合は何もしない
    }
}
