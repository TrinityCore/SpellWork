using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellScalingEntry
    {
        [Index(true)]
        public uint ID;
        public int SpellID;
        public uint MinScalingLevel;
        public uint MaxScalingLevel;
    }
}
