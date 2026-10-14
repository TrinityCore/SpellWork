using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellInterruptsEntry
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public int InterruptFlags;
        [Cardinality(2)]
        public int[] AuraInterruptFlags = new int[2];
        [Cardinality(2)]
        public int[] ChannelInterruptFlags = new int[2];
        public int SpellID;
    }
}
