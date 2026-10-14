using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class ContentTuningXExpectedEntry
    {
        [Index(true)]
        public uint ID;
        public int ExpectedStatModID;
        public int MinMythicPlusSeasonID;
        public int MaxMythicPlusSeasonID;
        public uint ContentTuningID;
    }
}
