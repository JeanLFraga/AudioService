using System;
using UnityEngine;

namespace JeanLF.AudioService
{
    [Serializable]
    public struct AudioDescription
    {
#if UNITY_EDITOR
        internal static readonly string TempoPath = nameof(_tempo);
        internal static readonly string SignaturePath = nameof(_timeSignature);
#endif

        [SerializeField] private int _tempo;
        [SerializeField] private Vector2Int _timeSignature;

        public AudioDescription(int tempo, Vector2Int timeSignature)
        {
            _tempo = tempo;
            _timeSignature = timeSignature;
        }

        public int Tempo => _tempo;
        public int UpperSignature => _timeSignature.x;
        public int LowerSignature => _timeSignature.y;
        public bool IsValid => _tempo > 0 || _timeSignature.x > 0 || _timeSignature.y > 0;

        public double BeatPerSecond => 60d / _tempo;

        public double BarPerSecond => 60d / _tempo * _timeSignature.x * ((double)_timeSignature.x / _timeSignature.y);
    }
}
