using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    public struct ReverbFilterProperties : IFilterProperty
    {
        public Type FilterType => typeof(AudioReverbFilter);
    }
}