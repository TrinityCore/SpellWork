using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class ItemEffectEntry
    {
        [Index(true)]
        public uint ID;
        public byte LegacySlotIndex;
        public byte TriggerType;
        public short Charges;
        public int CoolDownMSec;
        public int CategoryCoolDownMSec;
        public ushort SpellCategoryID;
        public int SpellID;
        public ushort ChrSpecializationID;
        public int PlayerConditionID;

        // Helper
        public ItemSparseEntry Item { get; set; }
        public int ItemID { get; set; }
    }
}
