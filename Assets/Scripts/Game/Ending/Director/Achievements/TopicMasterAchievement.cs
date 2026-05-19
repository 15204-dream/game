using UnityEngine;

namespace HeartHook.Game.Ending.Director.Achievements
{
    public class TopicMasterAchievement : DirectorAchievement
    {
        [Header("话题鬼才专属")]
        [SerializeField] private int requiredTopicCountForUnlock = 10;

        protected override void Start()
        {
            AchievementId = "TopicMaster";
            AchievementName = "话题鬼才";
            Description = "制造10个热门话题";
            IsSecret = false;

            requiredTopicCount = requiredTopicCountForUnlock;
            base.Start();
        }

        public override bool CheckUnlock(DirectorEndingResult result)
        {
            if (result == null)
                return false;

            return result.TopicCount >= requiredTopicCountForUnlock;
        }
    }
}
