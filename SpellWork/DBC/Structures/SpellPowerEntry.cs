using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellPowerEntry
    {
        [Index(false)]
        public uint ID;
        public byte OrderIndex;
        public int ManaCost;
        public int ManaCostPerLevel;
        public int ManaPerSecond;
        public uint PowerDisplayID;
        public int AltPowerBarID;
        public float PowerCostPct;
        public float PowerCostMaxPct;
        public float PowerPctPerSecond;
        public sbyte PowerType;
        public int RequiredAuraSpellID;
        public uint OptionalCost;
        [NonInlineRelation(typeof(uint))]
        public int SpellID;
    }
}
