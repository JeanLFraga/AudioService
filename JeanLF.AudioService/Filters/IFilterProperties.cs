using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    public interface IFilterProperty
    {
        IFilterProperty DefaultValue { get; }

        Type FilterType { get; }

        void SetupFilter(ref Component component);
    }
}
