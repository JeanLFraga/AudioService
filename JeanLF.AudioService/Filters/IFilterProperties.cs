using System;
using UnityEngine;

namespace JeanLF.AudioService.Filters
{
    public interface IFilterProperty
    {
        Type FilterType { get; }

        void SetupFilter(ref Component component);
    }
}
