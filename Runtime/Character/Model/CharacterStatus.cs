using System.Collections.Generic;
using anogame.framework;

namespace anogame.framework
{
    public class CharacterStatus
    {
        // 固定ステータス（基本値）
        public int BaseMaxHP { get; set; } = 100;
        public int BaseMaxMP { get; set; } = 30;
        public int BaseAttack { get; set; } = 10;
        public int BaseDefense { get; set; } = 5;
        public int BaseSpeed { get; set; } = 3;

        // 現在のHP/MP（ゲーム中に変化）
        public int CurrentHP { get; set; }
        public int CurrentMP { get; set; }

        // 装備品の参照
        public EquipmentModel Equipment { get; set; }
        private readonly List<Buff> activeBuffs = new();
        public IReadOnlyList<Buff> ActiveBuffs => activeBuffs;

        // バフ・デバフ（ステータス別に補正値を持つ）
        private readonly Dictionary<StatusType, int> statusModifiers = new();

        public CharacterStatus()
        {
            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
        }

        // 合計ステータス（装備 + バフ）
        public int MaxHP => BaseMaxHP + GetModifier(StatusType.HP);
        public int MaxMP => BaseMaxMP + GetModifier(StatusType.MP);
        public int TotalAttack => BaseAttack + EquipmentBonus(e => e.AttackBonus) + GetModifier(StatusType.Attack);
        public int TotalDefense => BaseDefense + EquipmentBonus(e => e.DefenseBonus) + GetModifier(StatusType.Defense);
        public int TotalSpeed => BaseSpeed + EquipmentBonus(e => e.SpeedBonus) + GetModifier(StatusType.Speed);

        private int EquipmentBonus(System.Func<EquippableItem, int> selector)
        {
            if (Equipment == null)
            {
                return 0;
            }

            int sum = 0;
            foreach (var item in Equipment.Equipped.Values)
            {
                if (item != null)
                {
                    sum += selector(item);
                }
            }
            return sum;
        }

        // 合計ステータスにバフ効果を加える
        private int GetModifier(StatusType type)
        {
            int sum = 0;
            foreach (var buff in activeBuffs)
            {
                if (buff.TargetStat == type)
                {
                    sum += buff.Amount;
                }
            }
            return sum;
        }

        public void AddBuff(Buff newBuff, BuffMergePolicy mergePolicy)
        {
            for (int i = 0; i < activeBuffs.Count; i++)
            {
                var existing = activeBuffs[i];
                if (existing.Id == newBuff.Id)
                {
                    activeBuffs[i].MergeWith(newBuff, mergePolicy);
                    return;
                }
            }

            // なければ新規追加
            activeBuffs.Add(newBuff);
        }

        public void AddBuff(BuffDefinition def)
        {
            var buff = Buff.FromDefinition(def);
            AddBuff(buff, def.MergePolicy);
        }

        public void AddModifier(StatusType type, int value)
        {
            if (statusModifiers.ContainsKey(type))
            {
                statusModifiers[type] += value;
            }
            else
            {
                statusModifiers[type] = value;
            }
        }

        public void ClearModifiers()
        {
            statusModifiers.Clear();
        }

        // バフの経過処理（ターン終了時などに呼ぶ）
        public void TickBuffs()
        {
            foreach (var buff in activeBuffs)
            {
                buff.Tick();
            }

            activeBuffs.RemoveAll(b => b.IsExpired);
        }


    }
}
