using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellAuraRestrictionsEntry
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public int CasterAuraState;
        public int TargetAuraState;
        public int ExcludeCasterAuraState;
        public int ExcludeTargetAuraState;
        public int CasterAuraSpell;
        public int TargetAuraSpell;
        public int ExcludeCasterAuraSpell;
        public int ExcludeTargetAuraSpell;
        public short CasterAuraType;
        public short TargetAuraType;
        public short ExcludeCasterAuraType;
        public short ExcludeTargetAuraType;
        public int SpellID;
    }
}
