using System.Collections;
using GlitchWorker.Story;
using GlitchWorker.Systems;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GlitchWorker.Tests.PlayMode
{
    [TestFixture]
    public class OverworldDirectorSmokeTests
    {
        private GameObject _host;
        private OverworldDirector _director;
        private ChapterCatalog _catalog;
        private ChapterDefinition _chapter;

        [SetUp]
        public void SetUp()
        {
            _host = new GameObject("OverworldDirectorHost");
            _director = _host.AddComponent<OverworldDirector>();
            _director.Initialize();

            _chapter = ScriptableObject.CreateInstance<ChapterDefinition>();
            _chapter.Configure(new StoryId("chapter.intro"), "Intro", "variant.default");

            ChapterVariantRule alertRule = new ChapterVariantRule();
            alertRule.Configure("variant.alert", new[] { new StoryId("flag.alert") });
            _chapter.ReplaceVariantRules(new[] { alertRule });

            _catalog = ScriptableObject.CreateInstance<ChapterCatalog>();
            _catalog.ReplaceChapters(new[] { _chapter });
            _director.SetCatalog(_catalog);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_catalog);
            Object.DestroyImmediate(_chapter);
            Object.DestroyImmediate(_host);
        }

        [UnityTest]
        public IEnumerator TryApplyChapter_UsesRuleVariant_WhenFlagIsSet()
        {
            _director.SetMetaFlag(new StoryId("flag.alert"), true);
            yield return null;

            bool applied = _director.TryApplyChapter(new StoryId("chapter.intro"));

            Assert.That(applied, Is.True);
            Assert.That(_director.CurrentVariantId, Is.EqualTo("variant.alert"));
        }

        [UnityTest]
        public IEnumerator TryApplyChapter_UsesFallbackVariant_WhenRuleDoesNotMatch()
        {
            yield return null;

            bool applied = _director.TryApplyChapter(new StoryId("chapter.intro"));

            Assert.That(applied, Is.True);
            Assert.That(_director.CurrentVariantId, Is.EqualTo("variant.default"));
        }
    }
}
