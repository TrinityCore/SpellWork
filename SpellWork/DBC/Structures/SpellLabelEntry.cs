using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellLabelEntry
    {
        [Index(true)]
        public uint ID;
        public uint LabelID;
        public int SpellID;
    }
}
