using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellReagentsCurrencyEntry
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public int CurrencyTypesID;
        public int CurrencyCount;
        public int OverrideRecraftCurrencyCount;
        public byte OrderSource;
    }
}
