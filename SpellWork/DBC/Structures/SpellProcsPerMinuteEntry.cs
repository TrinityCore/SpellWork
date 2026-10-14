using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellProcsPerMinuteEntry
    {
        [Index(true)]
        public uint ID;
        public float BaseProcRate;
        public int Flags;
    }
}
