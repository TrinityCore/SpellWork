using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class ExpectedStatModEntry
    {
        [Index(true)]
        public uint ID;
        public float CreatureHealthMod;
        public float PlayerHealthMod;
        public float CreatureAutoAttackDPSMod;
        public float CreatureArmorMod;
        public float PlayerManaMod;
        public float PlayerPrimaryStatMod;
        public float PlayerSecondaryStatMod;
        public float ArmorConstantMod;
        public float CreatureSpellDamageMod;
    }
}
