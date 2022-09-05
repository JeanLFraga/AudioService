using System;

namespace JeanLF.AudioService
{
    [Serializable]
    internal struct PoolSettings
    {
        public PoolSettings(int playerPoolCount, int filterPlayerPoolCount, int expandCount, int shrinkCount)
        {
            PlayerPoolCount = playerPoolCount;
            FilterPlayerPoolCount = filterPlayerPoolCount;
            ExpandCount = expandCount;
            ShrinkCount = shrinkCount;
        }

        public readonly int PlayerPoolCount;
        public readonly int FilterPlayerPoolCount;
        public readonly int ExpandCount;
        public readonly int ShrinkCount;
    }
}
