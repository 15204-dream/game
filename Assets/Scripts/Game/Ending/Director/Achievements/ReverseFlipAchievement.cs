using UnityEngine;

namespace HeartHook.Game.Ending.Director.Achievements
{
    public class ReverseFlipAchievement : DirectorAchievement
    {
        [Header("逆向翻盘专属")]
        [SerializeField] private int minFinalHeat = 60;
        [SerializeField] private int minFinalReputation = 50;
        [SerializeField] private int minCrisisCount = 2;

        protected override void Start()
        {
            AchievementId = "ReverseFlip";
            AchievementName = "逆向翻盘";
            Description = "在经历多次危机后成功翻盘";
            IsSecret = true;

            requireNoCrisis = false;
            base.Start();
        }

        public override bool CheckUnlock(DirectorEndingResult result)
        {
            if (result == null)
                return false;

            return result.CrisisCount >= minCrisisCount &&
                   result.FinalHeat >= minFinalHeat &&
                   result.FinalReputation >= minFinalReputation;
        }
    }
}
