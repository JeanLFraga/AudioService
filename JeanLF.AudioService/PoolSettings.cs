using System;
using UnityEngine;

namespace JeanLF.AudioService
{
    [Serializable]
    internal struct PoolSettings
    {
        public const string PlayerPoolCountName = nameof(_playerPoolCount);
        public const string FilterPlayerCountName = nameof(_filterPlayerPoolCount);
        public const string ExpandCountName = nameof(_expandCount);
        public const string ShrinkCountName = nameof(_shrinkCount);

        public PoolSettings(int playerPoolCount, int filterPlayerPoolCount, int expandCount, int shrinkCount)
        {
            _playerPoolCount = playerPoolCount;
            _filterPlayerPoolCount = filterPlayerPoolCount;
            _expandCount = expandCount;
            _shrinkCount = shrinkCount;
        }

        [SerializeField]
        private int _playerPoolCount;
        [SerializeField]
        private int _filterPlayerPoolCount;
        [SerializeField]
        private int _expandCount;
        [SerializeField]
        private int _shrinkCount;

        public int PlayerPoolCount => _playerPoolCount;
        public int FilterPlayerPoolCount => _filterPlayerPoolCount;
        public int ExpandCount => _expandCount;
        public int ShrinkCount => _shrinkCount;
    }
}
