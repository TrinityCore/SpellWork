using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellCategoriesEntry
    {
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public short Category;
        public sbyte DefenseType;
        public int DiminishType;
        public sbyte DispelType;
        public sbyte Mechanic;
        public int PreventionType;
        public short StartRecoveryCategory;
        public short ChargeCategory;
        public int SpellID;
    }
}
