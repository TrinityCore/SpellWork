using DBCD.IO.Attributes;
using System;

namespace SpellWork.DBC.Structures
{
    public class MapDifficultyEntry : IComparable
    {
        public string Message;
        [Index(true)]
        public uint ID;
        public short DifficultyID;
        public int LockID;
        public byte ResetInterval;
        public int MaxPlayers;
        public byte ItemContext;
        public int ItemContextPickerID;
        public int Flags;
        public int ContentTuningID;
        public int WorldStateExpressionID;
        public int MapID;

        public int CompareTo(object obj)
        {
            return obj is MapDifficultyEntry m ? ID.CompareTo(m.ID) : 1;
        }
    }
}
