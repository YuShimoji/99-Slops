using GlitchWorker.Story;
using UnityEngine;

namespace GlitchWorker.Systems
{
    public class OverworldDirector : MonoBehaviour
    {
        [SerializeField] private ChapterCatalog _chapterCatalog;
        [SerializeField] private bool _enableDecisionLogs = true;

        private readonly ChapterVariantResolver _resolver = new ChapterVariantResolver();
        private MetaFlagService _metaFlags;

        public StoryId CurrentChapterId { get; private set; }
        public string CurrentVariantId { get; private set; } = "default";
        public bool HasAppliedChapter => CurrentChapterId.IsDefined;

        public void Initialize(MetaFlagService metaFlags = null)
        {
            _metaFlags = metaFlags ?? new MetaFlagService();
        }

        public void SetCatalog(ChapterCatalog catalog)
        {
            _chapterCatalog = catalog;
        }

        public bool TryApplyChapter(StoryId chapterId)
        {
            EnsureInitialized();

            if (_chapterCatalog == null)
            {
                LogDecision(chapterId, "default", true, "missing_catalog");
                return false;
            }

            if (!_chapterCatalog.TryGetChapter(chapterId, out ChapterDefinition chapter))
            {
                LogDecision(chapterId, "default", true, "chapter_not_found");
                return false;
            }

            ChapterVariantResolver.ResolveResult result = _resolver.Resolve(chapter, _metaFlags.GetAllSetFlags());
            CurrentChapterId = chapterId;
            CurrentVariantId = result.VariantId;

            LogDecision(chapterId, result.VariantId, result.IsFallback, result.Reason);
            return true;
        }

        public void SetMetaFlag(StoryId flagId, bool value = true)
        {
            EnsureInitialized();
            _metaFlags.Set(flagId, value);
        }

        public void ResetMetaFlag(StoryId flagId)
        {
            EnsureInitialized();
            _metaFlags.Reset(flagId);
        }

        private void EnsureInitialized()
        {
            if (_metaFlags == null)
            {
                _metaFlags = new MetaFlagService();
            }
        }

        private void LogDecision(StoryId chapterId, string variantId, bool isFallback, string reason)
        {
            if (!_enableDecisionLogs)
            {
                return;
            }

            Debug.Log($"[OverworldDirector] chapter={chapterId} variant={variantId} fallback={isFallback} reason={reason}");
        }
    }
}
