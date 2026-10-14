using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellLevelsEntry
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public short MaxLevel;
        public byte MaxPassiveAuraLevel;
        public int BaseLevel;
        public int SpellLevel;
        public int SpellID;
    }
}
