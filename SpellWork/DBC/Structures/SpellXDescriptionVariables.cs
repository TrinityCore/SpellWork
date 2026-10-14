using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellXDescriptionVariablesEntry
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public int SpellDescriptionVariablesID;
    }
}
