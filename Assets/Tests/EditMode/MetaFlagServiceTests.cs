using GlitchWorker.Story;
using GlitchWorker.Systems;
using NUnit.Framework;

namespace GlitchWorker.Tests.EditMode
{
    public class MetaFlagServiceTests
    {
        [Test]
        public void SetAndResetFlag_UpdatesState()
        {
            MetaFlagService service = new MetaFlagService();
            StoryId seenIntro = new StoryId("flag.seen_intro");

            Assert.That(service.IsSet(seenIntro), Is.False);

            service.Set(seenIntro);
            Assert.That(service.IsSet(seenIntro), Is.True);

            bool removed = service.Reset(seenIntro);

            Assert.That(removed, Is.True);
            Assert.That(service.IsSet(seenIntro), Is.False);
        }

        [Test]
        public void UndefinedFlag_IsIgnoredAndReturnsFalse()
        {
            MetaFlagService service = new MetaFlagService();
            StoryId undefined = default;

            service.Set(undefined);
            bool isSet = service.IsSet(undefined);
            bool removed = service.Reset(undefined);

            Assert.That(isSet, Is.False);
            Assert.That(removed, Is.False);
            Assert.That(service.GetAllSetFlags().Count, Is.EqualTo(0));
        }
    }
}
