using UnityEngine;

namespace HeartHook.Game.Ending.Director.Achievements
{
    public class RatingMythAchievement : DirectorAchievement
    {
        [Header("收视神话专属")]
        [SerializeField] private int minHeatForUnlock = 95;
        [SerializeField] private int minReputationForUnlock = 90;

        protected override void Start()
        {
            AchievementId = "RatingMyth";
            AchievementName = "收视神话";
            Description = "热度达到95以上，口碑达到90以上";
            IsSecret = false;

            minHeat = minHeatForUnlock;
            minReputation = minReputationForUnlock;
            base.Start();
        }

        public override bool CheckUnlock(DirectorEndingResult result)
        {
            if (result == null)
                return false;

            return result.FinalHeat >= minHeatForUnlock &&
                   result.FinalReputation >= minReputationForUnlock &&
                   !result.HasCrisis;
        }
    }
}
