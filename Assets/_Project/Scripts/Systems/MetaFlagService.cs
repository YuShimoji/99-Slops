using System.Collections.Generic;
using GlitchWorker.Story;
using UnityEngine;

namespace GlitchWorker.Systems
{
    public sealed class MetaFlagService
    {
        private readonly HashSet<StoryId> _flags = new HashSet<StoryId>();

        public bool IsSet(StoryId flagId)
        {
            if (!flagId.IsDefined)
            {
                Debug.LogWarning("MetaFlagService: undefined flag ID read.");
                return false;
            }

            return _flags.Contains(flagId);
        }

        public void Set(StoryId flagId, bool value = true)
        {
            if (!flagId.IsDefined)
            {
                Debug.LogWarning("MetaFlagService: undefined flag ID write.");
                return;
            }

            bool changed = value ? _flags.Add(flagId) : _flags.Remove(flagId);

            if (changed)
            {
                Debug.Log($"MetaFlagService: '{flagId}' set={value}.");
            }
        }

        public bool Reset(StoryId flagId)
        {
            if (!flagId.IsDefined)
            {
                Debug.LogWarning("MetaFlagService: undefined flag ID reset.");
                return false;
            }

            bool removed = _flags.Remove(flagId);

            if (removed)
            {
                Debug.Log($"MetaFlagService: '{flagId}' reset.");
            }

            return removed;
        }

        public void ResetAll()
        {
            _flags.Clear();
            Debug.Log("MetaFlagService: all flags reset.");
        }

        public IReadOnlyCollection<StoryId> GetAllSetFlags()
        {
            return _flags;
        }
    }
}
