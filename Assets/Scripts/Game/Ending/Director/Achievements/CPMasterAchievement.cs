using UnityEngine;

namespace HeartHook.Game.Ending.Director.Achievements
{
    public class CPMasterAchievement : DirectorAchievement
    {
        [Header("CP制造机专属")]
        [SerializeField] private int requiredCPCountForUnlock = 5;

        protected override void Start()
        {
            AchievementId = "CPMaster";
            AchievementName = "CP制造机";
            Description = "成功配对5对CP";
            IsSecret = false;

            requiredCPCount = requiredCPCountForUnlock;
            base.Start();
        }

        public override bool CheckUnlock(DirectorEndingResult result)
        {
            if (result == null)
                return false;

            return result.CPCount >= requiredCPCountForUnlock;
        }
    }
}
