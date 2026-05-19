using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending.Director
{
    [CreateAssetMenu(fileName = "DirectorEndingData", menuName = "HeartHook/导演结局数据")]
    public class DirectorEndingData : ScriptableObject
    {
        [Header("等级结局配置")]
        public RankEndingConfig[] rankEndings;

        [Header("成就配置")]
        public AchievementConfig[] achievements;

        [Header("评级阈值")]
        public RankThreshold[] rankThresholds;

        [Header("全局配置")]
        public int MaxHeat = 100;
        public int MaxReputation = 100;
        public int MaxAudience = 1000000;

        public RankEndingConfig GetRankEnding(string rankId)
        {
            if (rankEndings == null)
                return null;

            foreach (var config in rankEndings)
            {
                if (config != null && config.RankId == rankId)
                    return config;
            }
            return null;
        }

        public AchievementConfig GetAchievement(string achievementId)
        {
            if (achievements == null)
                return null;

            foreach (var config in achievements)
            {
                if (config != null && config.AchievementId == achievementId)
                    return config;
            }
            return null;
        }

        public RankThreshold GetThreshold(string rankId)
        {
            if (rankThresholds == null)
                return null;

            foreach (var threshold in rankThresholds)
            {
                if (threshold != null && threshold.RankId == rankId)
                    return threshold;
            }
            return null;
        }

        public string CalculateRank(int heat, int reputation, bool hasCrisis)
        {
            if (heat >= 95 && reputation >= 90 && !hasCrisis)
                return "S";
            if (heat >= 80 && reputation >= 75)
                return "A";
            if (heat >= 60 && reputation >= 60)
                return "B";
            if (heat >= 40 && reputation >= 40)
                return "C";
            if (reputation >= 20)
                return "D";
            return "E";
        }
    }

    [Serializable]
    public class RankEndingConfig
    {
        public string RankId;
        public string RankName;
        public string Description;
        public Sprite Background;
        public Sprite Badge;
        public AudioClip BGM;
        public Color RankColor = Color.white;
        public EndingDisplayConfig DisplayConfig;
        public EndingRewardConfig RewardConfig;
        public int DiamondReward;
        public string UnlockHint;
    }

    [Serializable]
    public class AchievementConfig
    {
        public string AchievementId;
        public string AchievementName;
        public string Description;
        public Sprite Icon;
        public bool IsSecret;
        public string UnlockHint;
        public int DiamondReward;
    }

    [Serializable]
    public class RankThreshold
    {
        public string RankId;
        public int MinHeat;
        public int MinReputation;
        public int MinAudience;
        public bool RequireNoCrisis;
    }

    [Serializable]
    public class EndingDisplayConfig
    {
        public float FadeInDuration = 1f;
        public float TitleDisplayDuration = 2f;
        public float StatsDisplayDuration = 3f;
        public float AchievementDisplayDelay = 1f;
        public bool ShowDetailedStats = true;
    }

    [Serializable]
    public class EndingRewardConfig
    {
        public int Diamonds;
        public int Experience;
        public string UnlockItemId;
        public string[] UnlockAchievementIds;
    }
}
