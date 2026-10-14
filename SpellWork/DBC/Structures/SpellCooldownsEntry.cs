using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellCooldownsEntry
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public int CategoryRecoveryTime;
        public int RecoveryTime;
        public int StartRecoveryTime;
        public int AuraSpellID;
        public int SpellID;
    }
}
