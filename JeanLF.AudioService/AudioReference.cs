using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JeanLF.AudioService
{
    [System.Serializable]
    public struct AudioReference
    {
#if UNITY_EDITOR
        internal const string EntryMemberPath = nameof(_entryId);
        internal const string GroupMemberPath = nameof(_groupId);
#endif

        [SerializeField]
        private string _entryId;
        [SerializeField]
        private string _groupId;

        public string EntryId => _entryId;

        public string GroupId => _groupId;

        public AudioReference(string entryId, string groupId)
        {
            _entryId = entryId;
            _groupId = groupId;
        }

        public bool Equals(string entryId, string groupId)
        {
            return _entryId == entryId && _groupId == groupId;
        }

        public bool Equals(AudioReference other)
        {
            return _entryId == other._entryId
                && _groupId == other._groupId;
        }

        public override bool Equals(object obj)
        {
            return obj is AudioReference other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_entryId != null ? _entryId.GetHashCode() : 0) * 397) ^ (_groupId != null ? _groupId.GetHashCode() : 0);
            }
        }

        public override string ToString() {
            return $"{_entryId}";
        }
    }
}
