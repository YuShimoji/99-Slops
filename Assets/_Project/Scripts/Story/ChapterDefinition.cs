using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlitchWorker.Story
{
    [Serializable]
    public sealed class ChapterVariantRule
    {
        [SerializeField] private string _variantId = "default";
        [SerializeField] private List<StoryId> _requiredFlags = new List<StoryId>();

        public string VariantId => _variantId;
        public IReadOnlyList<StoryId> RequiredFlags => _requiredFlags;

        public bool IsMatch(IReadOnlyCollection<StoryId> activeFlags)
        {
            if (activeFlags == null)
            {
                return _requiredFlags.Count == 0;
            }

            for (int i = 0; i < _requiredFlags.Count; i++)
            {
                StoryId flagId = _requiredFlags[i];
                if (!flagId.IsDefined || !ContainsFlag(activeFlags, flagId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ContainsFlag(IReadOnlyCollection<StoryId> activeFlags, StoryId target)
        {
            foreach (StoryId active in activeFlags)
            {
                if (active == target)
                {
                    return true;
                }
            }

            return false;
        }

        public void Configure(string variantId, IEnumerable<StoryId> requiredFlags)
        {
            _variantId = string.IsNullOrWhiteSpace(variantId) ? "default" : variantId.Trim();
            _requiredFlags.Clear();
            if (requiredFlags != null)
            {
                _requiredFlags.AddRange(requiredFlags);
            }
        }
    }

    [CreateAssetMenu(fileName = "ChapterDefinition", menuName = "GlitchWorker/Story/Chapter Definition")]
    public class ChapterDefinition : ScriptableObject
    {
        [SerializeField] private StoryId _chapterId;
        [SerializeField] private string _displayName;
        [SerializeField] private string _fallbackVariantId = "default";
        [SerializeField] private List<ChapterVariantRule> _variantRules = new List<ChapterVariantRule>();

        public StoryId ChapterId => _chapterId;
        public string DisplayName => _displayName;
        public string FallbackVariantId => _fallbackVariantId;
        public IReadOnlyList<ChapterVariantRule> VariantRules => _variantRules;
        public bool IsValid => _chapterId.IsDefined;

        public void Configure(StoryId chapterId, string displayName, string fallbackVariantId = "default")
        {
            _chapterId = chapterId;
            _displayName = displayName;
            _fallbackVariantId = string.IsNullOrWhiteSpace(fallbackVariantId) ? "default" : fallbackVariantId.Trim();
        }

        public void ReplaceVariantRules(IEnumerable<ChapterVariantRule> rules)
        {
            _variantRules.Clear();
            if (rules != null)
            {
                _variantRules.AddRange(rules);
            }
        }
    }
}
