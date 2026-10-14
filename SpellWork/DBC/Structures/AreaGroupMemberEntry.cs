using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class AreaGroupMemberEntry
    {
        [Index(true)]
        public uint ID;
        public ushort AreaID;
        public int AreaGroupID;
    }
}
