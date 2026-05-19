using UnityEngine;

namespace HeartHook.Game.Ending.Director.Achievements
{
    public class StarDirectorAchievement : DirectorAchievement
    {
        [Header("造星导演专属")]
        [SerializeField] private int requiredStarCountForUnlock = 3;

        protected override void Start()
        {
            AchievementId = "StarDirector";
            AchievementName = "造星导演";
            Description = "成功打造3位明星嘉宾";
            IsSecret = false;

            requiredStarCount = requiredStarCountForUnlock;
            base.Start();
        }

        public override bool CheckUnlock(DirectorEndingResult result)
        {
            if (result == null)
                return false;

            return result.StarCount >= requiredStarCountForUnlock;
        }
    }
}
