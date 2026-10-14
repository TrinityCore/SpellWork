using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellDescriptionVariablesEntry
    {
        [Index(true)]
        public uint ID;
        public string Variables;
    }
}
