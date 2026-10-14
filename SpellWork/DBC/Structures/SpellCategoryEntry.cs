using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellCategoryEntry
    {
        [Index(true)]
        public uint ID;
        public string Name;
        public int Flags;
        public int UsesPerWeek;
        public int MaxCharges;
        public int ChargeRecoveryTime;
        public int TypeMask;
    };
}
