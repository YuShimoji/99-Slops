using GlitchWorker.Story;
using NUnit.Framework;
using UnityEngine;

namespace GlitchWorker.Tests.EditMode
{
    public class ChapterVariantResolverTests
    {
        [Test]
        public void Resolve_ReturnsMatchedVariant_WhenRequiredFlagsAreSet()
        {
            ChapterDefinition chapter = ScriptableObject.CreateInstance<ChapterDefinition>();
            chapter.Configure(new StoryId("chapter.intro"), "Intro", "fallback");

            ChapterVariantRule rule = new ChapterVariantRule();
            rule.Configure("variant.alert", new[] { new StoryId("flag.alert") });
            chapter.ReplaceVariantRules(new[] { rule });

            MetaFlagService metaFlags = new MetaFlagService();
            metaFlags.Set(new StoryId("flag.alert"));

            ChapterVariantResolver resolver = new ChapterVariantResolver();
            ChapterVariantResolver.ResolveResult result = resolver.Resolve(chapter, metaFlags.GetAllSetFlags());

            Assert.That(result.IsFallback, Is.False);
            Assert.That(result.VariantId, Is.EqualTo("variant.alert"));
        }

        [Test]
        public void Resolve_ReturnsFallback_WhenNoRuleMatches()
        {
            ChapterDefinition chapter = ScriptableObject.CreateInstance<ChapterDefinition>();
            chapter.Configure(new StoryId("chapter.intro"), "Intro", "variant.default");

            ChapterVariantRule rule = new ChapterVariantRule();
            rule.Configure("variant.alert", new[] { new StoryId("flag.alert") });
            chapter.ReplaceVariantRules(new[] { rule });

            ChapterVariantResolver resolver = new ChapterVariantResolver();
            ChapterVariantResolver.ResolveResult result = resolver.Resolve(chapter, new StoryId[0]);

            Assert.That(result.IsFallback, Is.True);
            Assert.That(result.VariantId, Is.EqualTo("variant.default"));
        }
    }
}
