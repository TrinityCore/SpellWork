using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellTargetRestrictionsEntry
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public float ConeDegrees;
        public byte MaxTargets;
        public uint MaxTargetLevel;
        public short TargetCreatureType;
        public int Targets;
        public float Width;
        public int SpellID;
    }
}
