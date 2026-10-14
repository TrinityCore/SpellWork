using DBCD.IO.Attributes;

namespace SpellWork.DBC.Structures
{
    public class ExpectedStatEntry
    {
        [Index(true)]
        public uint ID;
        public int ExpansionID;
        public float CreatureHealth;
        public float PlayerHealth;
        public float CreatureAutoAttackDps;
        public float CreatureArmor;
        public float PlayerMana;
        public float PlayerPrimaryStat;
        public float PlayerSecondaryStat;
        public float ArmorConstant;
        public float CreatureSpellDamage;
        public int ContentSetID;
        public uint Lvl;
    }
}
