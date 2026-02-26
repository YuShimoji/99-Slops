using GlitchWorker.Story;
using GlitchWorker.Systems;
using NUnit.Framework;
using UnityEngine;

namespace GlitchWorker.Tests.EditMode
{
    public class OverworldDirectorTests
    {
        private GameObject _host;
        private OverworldDirector _director;

        [SetUp]
        public void SetUp()
        {
            _host = new GameObject("OverworldDirectorTestHost");
            _director = _host.AddComponent<OverworldDirector>();
            _director.Initialize(new MetaFlagService());
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_host);
        }

        [Test]
        public void TryApplyChapter_ReturnsTrue_WhenCatalogContainsChapter()
        {
            ChapterDefinition chapter = ScriptableObject.CreateInstance<ChapterDefinition>();
            chapter.Configure(new StoryId("chapter.intro"), "Intro");

            ChapterCatalog catalog = ScriptableObject.CreateInstance<ChapterCatalog>();
            catalog.ReplaceChapters(new[] { chapter });

            _director.SetCatalog(catalog);
            bool applied = _director.TryApplyChapter(new StoryId("chapter.intro"));

            Assert.That(applied, Is.True);
            Assert.That(_director.CurrentVariantId, Is.EqualTo("default"));
            Assert.That(_director.HasAppliedChapter, Is.True);
        }

        [Test]
        public void TryApplyChapter_ReturnsFalse_WhenChapterIsMissing()
        {
            ChapterCatalog catalog = ScriptableObject.CreateInstance<ChapterCatalog>();
            _director.SetCatalog(catalog);

            bool applied = _director.TryApplyChapter(new StoryId("chapter.missing"));

            Assert.That(applied, Is.False);
            Assert.That(_director.HasAppliedChapter, Is.False);
        }
    }
}
