using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellNameEntry
    {
        [Index(true)]
        public uint ID;
        public string Name;
    }
}
