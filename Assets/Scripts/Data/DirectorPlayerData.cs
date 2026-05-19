using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 导演能力类型
    /// </summary>
    public enum DirectorAbilityType
    {
        ScriptWriting,
        SceneDirection,
        TalentManagement,
        AudienceAnalysis,
        CrisisControl,
        Marketing,
        Budgeting,
        CreativeStorytelling
    }

    /// <summary>
    /// 导演能力数据
    /// </summary>
    [System.Serializable]
    public class DirectorAbility
    {
        public DirectorAbilityType abilityType;
        public string abilityName;
        public string description;
        public int level;
        public int maxLevel;
        public int experience;
        public int experienceToNextLevel;
        public bool isUnlocked;
        public List<string> relatedAchievements = new List<string>();
    }

    /// <summary>
    /// 导演模式成就
    /// </summary>
    [System.Serializable]
    public class DirectorAchievement
    {
        public string achievementId;
        public string achievementName;
        public string description;
        public bool isUnlocked;
        public DateTime? unlockTime;
        public int rewardCoins;
        public int rewardFans;
        public string prerequisiteAchievementId;
    }

    /// <summary>
    /// 节目配置
    /// </summary>
    [System.Serializable]
    public class ShowConfiguration
    {
        public string showName;
        public string showTheme;
        public int totalEpisodes;
        public int currentEpisode;
        public ShowFormat format;
        public ShowRating targetRating;
        public int targetAudience;
        public int productionBudget;
    }

    /// <summary>
    /// 节目格式
    /// </summary>
    public enum ShowFormat
    {
        Traditional,
        RealityCompetition,
        DatingGame,
        Documentary,
        Interactive
    }

    /// <summary>
    /// 节目评级
    /// </summary>
    public enum ShowRating
    {
        G,
        PG,
        PG13,
        TV14,
        TVMA
    }

    /// <summary>
    /// 嘉宾管理数据
    /// </summary>
    [System.Serializable]
    public class GuestManagementData
    {
        public string guestId;
        public GuestManagementStatus status;
        public int screenTime;
        public int popularity;
        public int dramaPotential;
        public int audienceFavorability;
        public List<string> storylines = new List<string>();
        public List<string> scheduledAppearances = new List<string>();
        public bool isEliminated;
        public int eliminationEpisode;
    }

    /// <summary>
    /// 嘉宾管理状态
    /// </summary>
    public enum GuestManagementStatus
    {
        Pending,
        Active,
        Inactive,
        Eliminated,
        Winner
    }

    /// <summary>
    /// 剧本数据
    /// </summary>
    [System.Serializable]
    public class ScriptData
    {
        public string scriptId;
        public string scriptName;
        public int episodeNumber;
        public List<ScriptScene> scenes = new List<ScriptScene>();
        public ScriptStatus status;
        public int approvalRating;
        public List<string> improvisationAllowed = new List<string>();
    }

    /// <summary>
    /// 剧本状态
    /// </summary>
    public enum ScriptStatus
    {
        Draft,
        InReview,
        Approved,
        Rejected,
        Revised
    }

    /// <summary>
    /// 剧本场景
    /// </summary>
    [System.Serializable]
    public class ScriptScene
    {
        public string sceneId;
        public string sceneName;
        public int duration;
        public SceneType sceneType;
        public List<ScriptEvent> events = new List<ScriptEvent>();
        public List<string> requiredGuestIds = new List<string>();
        public List<string> optionalGuestIds = new List<string>();
        public bool isCompleted;
    }

    /// <summary>
    /// 剧本事件
    /// </summary>
    [System.Serializable]
    public class ScriptEvent
    {
        public string eventId;
        public string eventName;
        public string description;
        public EventType eventType;
        public int dramaValue;
        public int entertainmentValue;
        public int audienceAppeal;
    }

    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EventType
    {
        Conversation,
        Challenge,
        Date,
        Elimination,
        Revelation,
        Conflict,
        Romance,
        Surprise,
        Competition
    }

    /// <summary>
    /// 导演玩家数据
    /// </summary>
    [System.Serializable]
    public class DirectorPlayerData
    {
        [Header("导演信息")]
        public string directorId;
        public string directorName;
        public int directorLevel;
        public int experience;
        public int experienceToNextLevel;

        [Header("能力")]
        public List<DirectorAbility> abilities = new List<DirectorAbility>();
        public int totalAbilityPoints;
        public int availableAbilityPoints;

        [Header("资源")]
        public int coins = 10000;
        public int gems = 0;
        public int productionBudget = 100000;
        public int usedBudget;

        [Header("节目数据")]
        public ShowConfiguration showConfig = new ShowConfiguration();
        public List<ScriptData> scripts = new List<ScriptData>();
        public ScriptData currentScript;

        [Header("嘉宾管理")]
        public List<GuestManagementData> guestManagementList = new List<GuestManagementData>();
        public Dictionary<string, GuestManagementData> guestManagementDict = new Dictionary<string, GuestManagementData>();

        [Header("导演成就")]
        public List<DirectorAchievement> achievements = new List<DirectorAchievement>();
        public int totalAchievementPoints;
        public int showsProduced;
        public int successfulShows;
        public int failedShows;

        [Header("统计")]
        public int totalEpisodesDirected;
        public int peakAudienceRating;
        public int averageAudienceRating;
        public int totalFans;
        public int totalCoinsEarned;
        public int totalCoinsSpent;

        [Header("技能树")]
        public List<string> unlockedSkills = new List<string>();
        public List<string> availableSkills = new List<string>();
        public Dictionary<string, int> skillLevels = new Dictionary<string, int>();

        [Header("历史记录")]
        public List<string> completedShowIds = new List<string>();
        public List<string> failedShowIds = new List<string>();
        public List<string> createdTemplates = new List<string>();
    }

    /// <summary>
    /// 导演数据工具类
    /// </summary>
    public static class DirectorPlayerHelper
    {
        public static int CalculateDirectorLevel(int experience)
        {
            return Mathf.FloorToInt(Mathf.Sqrt(experience / 100f));
        }

        public static int CalculateExperienceForNextLevel(int currentLevel)
        {
            return 100 * (currentLevel + 1) * (currentLevel + 1);
        }

        public static float CalculateBudgetEfficiency(int usedBudget, int totalBudget)
        {
            if (totalBudget == 0) return 0f;
            return 1f - ((float)usedBudget / totalBudget);
        }

        public static int CalculateDramaValue(List<ScriptEvent> events)
        {
            int totalDrama = 0;
            foreach (var evt in events)
            {
                totalDrama += evt.dramaValue;
            }
            return totalDrama;
        }

        public static float CalculateAudienceAppeal(List<ScriptScene> scenes)
        {
            if (scenes.Count == 0) return 0f;

            int totalAppeal = 0;
            foreach (var scene in scenes)
            {
                foreach (var evt in scene.events)
                {
                    totalAppeal += evt.audienceAppeal;
                }
            }

            return (float)totalAppeal / scenes.Count;
        }

        public static bool CanUpgradeAbility(DirectorAbility ability, int availablePoints)
        {
            return ability.isUnlocked &&
                   ability.level < ability.maxLevel &&
                   availablePoints > 0;
        }
    }
}
