using DBFileReaderLib.Attributes;
using System;

namespace SpellWork.DBC.Structures
{
    public class MapDifficultyEntry : IComparable
    {
        public string Message;
        [Index(false)]
        public uint ID;
        public int DifficultyID;
        public int LockID;
        public byte ResetInterval;
        public int MaxPlayers;
        public byte ItemContext;
        public int ItemContextPickerID;
        public int Flags;
        public int ContentTuningID;
        public int WorldStateExpressionID;
        [NonInlineRelation(typeof(uint))]
        public int MapID;
        public int CompareTo(object obj)
        {
            return obj is MapDifficultyEntry m ? ID.CompareTo(m.ID) : 1;
        }
    }
}
