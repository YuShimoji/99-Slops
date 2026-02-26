using GlitchWorker.Story;
using NUnit.Framework;
using UnityEngine;

namespace GlitchWorker.Tests.EditMode
{
    public class ChapterCatalogTests
    {
        [Test]
        public void TryGetChapter_ReturnsRegisteredChapter()
        {
            ChapterDefinition chapterA = ScriptableObject.CreateInstance<ChapterDefinition>();
            ChapterDefinition chapterB = ScriptableObject.CreateInstance<ChapterDefinition>();
            chapterA.Configure(new StoryId("chapter.intro"), "Intro");
            chapterB.Configure(new StoryId("chapter.escape"), "Escape");

            ChapterCatalog catalog = ScriptableObject.CreateInstance<ChapterCatalog>();
            catalog.ReplaceChapters(new[] { chapterA, chapterB });

            bool found = catalog.TryGetChapter(new StoryId("chapter.escape"), out ChapterDefinition resolved);

            Assert.That(found, Is.True);
            Assert.That(resolved, Is.SameAs(chapterB));
        }

        [Test]
        public void TryGetChapter_UndefinedId_ReturnsFalse()
        {
            ChapterCatalog catalog = ScriptableObject.CreateInstance<ChapterCatalog>();

            bool found = catalog.TryGetChapter(default, out ChapterDefinition resolved);

            Assert.That(found, Is.False);
            Assert.That(resolved, Is.Null);
        }
    }
}
