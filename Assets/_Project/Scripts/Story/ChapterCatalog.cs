using System.Collections.Generic;
using UnityEngine;

namespace GlitchWorker.Story
{
    [CreateAssetMenu(fileName = "ChapterCatalog", menuName = "GlitchWorker/Story/Chapter Catalog")]
    public class ChapterCatalog : ScriptableObject
    {
        [SerializeField] private List<ChapterDefinition> _chapters = new List<ChapterDefinition>();

        private readonly Dictionary<StoryId, ChapterDefinition> _chapterById = new Dictionary<StoryId, ChapterDefinition>();
        private bool _isIndexed;

        public IReadOnlyList<ChapterDefinition> Chapters => _chapters;

        public bool TryGetChapter(StoryId chapterId, out ChapterDefinition chapter)
        {
            chapter = null;

            if (!chapterId.IsDefined)
            {
                Debug.LogWarning("ChapterCatalog: undefined chapter ID query.");
                return false;
            }

            EnsureIndexed();
            return _chapterById.TryGetValue(chapterId, out chapter);
        }

        public void ReplaceChapters(IEnumerable<ChapterDefinition> chapters)
        {
            _chapters.Clear();

            if (chapters != null)
            {
                _chapters.AddRange(chapters);
            }

            RebuildIndex();
        }

        private void OnEnable()
        {
            RebuildIndex();
        }

        private void OnValidate()
        {
            RebuildIndex();
        }

        private void EnsureIndexed()
        {
            if (_isIndexed)
            {
                return;
            }

            RebuildIndex();
        }

        private void RebuildIndex()
        {
            _chapterById.Clear();

            for (int i = 0; i < _chapters.Count; i++)
            {
                ChapterDefinition chapter = _chapters[i];

                if (chapter == null)
                {
                    continue;
                }

                if (!chapter.IsValid)
                {
                    Debug.LogWarning($"ChapterCatalog: chapter at index {i} has undefined ID.");
                    continue;
                }

                if (_chapterById.ContainsKey(chapter.ChapterId))
                {
                    Debug.LogWarning($"ChapterCatalog: duplicate chapter ID '{chapter.ChapterId}'.");
                    continue;
                }

                _chapterById.Add(chapter.ChapterId, chapter);
            }

            _isIndexed = true;
        }
    }
}
