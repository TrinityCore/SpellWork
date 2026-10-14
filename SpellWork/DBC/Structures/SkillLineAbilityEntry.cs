using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SkillLineAbilityEntry
    {
        public string AbilityVerb;
        public string AbilityAllVerb;
        [Index(false)]
        public uint ID;
        public ushort SkillLine;
        public int Spell;
        public short MinSkillLineRank;
        public int ClassMask;
        public int SupercedesSpell;
        public int AcquireMethod;
        public short TrivialSkillLineRankHigh;
        public short TrivialSkillLineRankLow;
        public int Flags;
        public sbyte NumSkillUps;
        public short UniqueBit;
        public short TradeSkillCategoryID;
        public short SkillupSkillLineID;
        [Cardinality(2)]
        public int[] RaceMask = new int[2];
    }
}
