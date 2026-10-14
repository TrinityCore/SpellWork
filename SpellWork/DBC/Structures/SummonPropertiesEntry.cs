using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SummonPropertiesEntry
    {
        [Index(true)]
        public uint ID;
        public int Control;
        public int Faction;
        public int Title;
        public int Slot;
        [Cardinality(2)]
        public int[] Flags;
    }
}
