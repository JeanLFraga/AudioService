using System;
using UnityEngine;
using UnityEngine.Audio;

namespace JeanLF.AudioService
{
    [Serializable]
    internal struct AudioGroup
    {
        #if UNITY_EDITOR
        internal static readonly string IdPropertyPath = nameof(_id);
        #endif

        [Delayed][SerializeField] private string _id;
        [SerializeField] private AudioMixerGroup _mixerGroup;
        private GroupId _cachedId;

        public string Id => _id;
        public GroupId ConvertedId
        {
            get
            {
                if (_cachedId == GroupId.Invalid)
                {
                    if (!GroupId.TryParse(_id, out _cachedId))
                    {
                        _cachedId = GroupId.Invalid;
                    }
                }

                return _cachedId;
            }
        }
        public AudioMixerGroup MixerGroup => _mixerGroup;
    }
}
