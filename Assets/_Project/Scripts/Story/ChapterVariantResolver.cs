using System.Collections.Generic;

namespace GlitchWorker.Story
{
    public sealed class ChapterVariantResolver
    {
        public readonly struct ResolveResult
        {
            public ResolveResult(string variantId, bool isFallback, string reason)
            {
                VariantId = variantId;
                IsFallback = isFallback;
                Reason = reason;
            }

            public string VariantId { get; }
            public bool IsFallback { get; }
            public string Reason { get; }
        }

        public ResolveResult Resolve(ChapterDefinition chapter, IReadOnlyCollection<StoryId> activeFlags)
        {
            if (chapter == null || !chapter.IsValid)
            {
                return new ResolveResult("default", true, "invalid_chapter");
            }

            IReadOnlyList<ChapterVariantRule> rules = chapter.VariantRules;
            for (int i = 0; i < rules.Count; i++)
            {
                ChapterVariantRule rule = rules[i];
                if (rule == null)
                {
                    continue;
                }

                if (rule.IsMatch(activeFlags))
                {
                    string variantId = string.IsNullOrWhiteSpace(rule.VariantId) ? chapter.FallbackVariantId : rule.VariantId;
                    return new ResolveResult(variantId, false, $"matched_rule_{i}");
                }
            }

            return new ResolveResult(chapter.FallbackVariantId, true, "fallback_no_match");
        }
    }
}
