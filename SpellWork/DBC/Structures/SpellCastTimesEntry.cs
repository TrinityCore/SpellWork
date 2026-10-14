using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellCastTimesEntry
    {
        [Index(true)]
        public uint ID;
        public int Base;
        public int Minimum;
    }
}
