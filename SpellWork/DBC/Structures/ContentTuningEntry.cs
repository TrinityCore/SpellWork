using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class ContentTuningEntry
    {
        [Index(false)]
        public uint ID;
        public int Flags;
        public int ExpansionID;
        public int HealthItemLevelCurveID;
        public int DamageItemLevelCurveID;
        public int HealthPrimaryStatCurveID;
        public int DamagePrimaryStatCurveID;
        public int PrimaryStatScalingModPlayerDataElementCharacterID;
        public float PrimaryStatScalingModPlayerDataElementCharacterMultiplier;
        public int MinLevel;
        public int MaxLevel;
        public int MinLevelType;
        public int MaxLevelType;
        public int TargetLevelDelta;
        public int TargetLevelMaxDelta;
        public int TargetLevelMin;
        public int TargetLevelMax;
        public int MinItemLevel;
        public float QuestXpMultiplier;
    }
}
