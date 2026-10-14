using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class OverrideSpellDataEntry
    {
        [Index(true)]
        public uint ID;
        [Cardinality(10)]
        public int[] Spells = new int[10];
        public int PlayerActionbarFileDataID;
        public int Flags;
    }
}
