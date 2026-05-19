using UnityEngine;

namespace HeartHook.Game.Ending.Director.Achievements
{
    public class CrisisMasterAchievement : DirectorAchievement
    {
        [Header("危机终结者专属")]
        [SerializeField] private int minHeatForUnlock = 70;
        [SerializeField] private int minReputationForUnlock = 70;

        protected override void Start()
        {
            AchievementId = "CrisisMaster";
            AchievementName = "危机终结者";
            Description = "在发生危机的情况下仍保持高热度高口碑";
            IsSecret = true;
            requireNoCrisis = false;

            minHeat = minHeatForUnlock;
            minReputation = minReputationForUnlock;
            base.Start();
        }

        public override bool CheckUnlock(DirectorEndingResult result)
        {
            if (result == null)
                return false;

            return result.HasCrisis &&
                   result.FinalHeat >= minHeatForUnlock &&
                   result.FinalReputation >= minReputationForUnlock;
        }
    }
}
