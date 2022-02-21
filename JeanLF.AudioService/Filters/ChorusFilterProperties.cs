using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    public struct ChorusFilterProperties : IFilterProperty
    {
        
        public Type FilterType => typeof(AudioChorusFilter);
    }
}