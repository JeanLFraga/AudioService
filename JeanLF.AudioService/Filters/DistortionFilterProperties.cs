using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct DistortionFilterProperties : IFilterProperty
    {
        [Range(0,1)] [SerializeField] private float _distortionLevel;
        
        public Type FilterType => typeof(AudioDistortionFilter);
        public float DistortionLevel => _distortionLevel;
    }
}