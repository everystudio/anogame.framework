namespace anogame.framework
{
    public class Buff
    {
        public string Id { get; }
        public StatusType TargetStat { get; }
        public int Amount { get; private set; }
        public int RemainingTurns { get; private set; }

        public static Buff FromDefinition(BuffDefinition def)
        {
            return new Buff(def.Id, def.TargetStat, def.Amount, def.DurationTurns);
        }

        public Buff(string id, StatusType stat, int amount, int duration)
        {
            Id = id;
            TargetStat = stat;
            Amount = amount;
            RemainingTurns = duration;
        }

        public void Tick()
        {
            RemainingTurns--;
        }

        public bool IsExpired => RemainingTurns <= 0;

        public void Refresh(int newAmount, int newDuration)
        {
            Amount = newAmount;
            RemainingTurns = System.Math.Max(RemainingTurns, newDuration);
        }

        public void MergeWith(Buff newBuff, BuffMergePolicy policy)
        {
            switch (policy)
            {
                case BuffMergePolicy.Override:
                    Amount = newBuff.Amount;
                    RemainingTurns = newBuff.RemainingTurns;
                    break;
                case BuffMergePolicy.Extend:
                    RemainingTurns = System.Math.Max(RemainingTurns, newBuff.RemainingTurns);
                    break;
                case BuffMergePolicy.Ignore:
                    // 何もしない
                    break;
            }
        }

    }
} 