using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    [Serializable]
    public struct DistortionFilterProperties : IFilterProperty
    {
        public static readonly DistortionFilterProperties DefaultValues = new DistortionFilterProperties(0.5f);

        [Range(0,1)] [SerializeField] private float _distortionLevel;

        public DistortionFilterProperties(float distortionLevel = 0.5f)
        {
            _distortionLevel = distortionLevel;
        }

        public Type FilterType => typeof(AudioDistortionFilter);

        public void SetupFilter(ref Component component)
        {
            AudioDistortionFilter filter = (AudioDistortionFilter)component;
            filter.distortionLevel = _distortionLevel;
        }

        public float DistortionLevel => _distortionLevel;
    }
}
