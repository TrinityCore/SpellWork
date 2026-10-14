using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellEntry
    {
        [Index(true)]
        public uint ID;
        public string NameSubtext;
        public string Description;
        public string AuraDescription;
    }
}
