using UnityEngine;

namespace GlitchWorker.Story
{
    [CreateAssetMenu(fileName = "ChapterDefinition", menuName = "GlitchWorker/Story/Chapter Definition")]
    public class ChapterDefinition : ScriptableObject
    {
        [SerializeField] private StoryId _chapterId;
        [SerializeField] private string _displayName;

        public StoryId ChapterId => _chapterId;
        public string DisplayName => _displayName;
        public bool IsValid => _chapterId.IsDefined;

        public void Configure(StoryId chapterId, string displayName)
        {
            _chapterId = chapterId;
            _displayName = displayName;
        }
    }
}
