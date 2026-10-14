using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellTotemsEntry
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        [Cardinality(2)]
        public ushort[] RequiredTotemCategoryID = new ushort[2];
        [Cardinality(2)]
        public int[] Totem = new int[2];
    }
}
