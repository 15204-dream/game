using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 玩家角色能力数据
    /// </summary>
    [System.Serializable]
    public class PlayerAbilityData
    {
        [Header("基础属性")]
        public int charm = 50;
        public int intelligence = 50;
        public int athleticAbility = 50;
        public int cookingSkill = 50;
        public int artisticSkill = 50;
        public int musicalSkill = 50;
        public int socialSkill = 50;
        public int romanticSkill = 50;

        [Header("属性上限")]
        public int maxCharm = 100;
        public int maxIntelligence = 100;
        public int maxAthleticAbility = 100;
        public int maxCookingSkill = 100;
        public int maxArtisticSkill = 100;
        public int maxMusicalSkill = 100;
        public int maxSocialSkill = 100;
        public int maxRomanticSkill = 100;

        [Header("属性经验值")]
        public Dictionary<string, int> skillExperience = new Dictionary<string, int>();
        public Dictionary<string, int> skillLevel = new Dictionary<string, int>();
    }

    /// <summary>
    /// 玩家特殊能力
    /// </summary>
    [System.Serializable]
    public class PlayerSpecialAbility
    {
        public string abilityId;
        public string abilityName;
        public string description;
        public int level;
        public int maxLevel;
        public bool isUnlocked;
        public int unlockRequirement;
        public float cooldownTime;
        public float currentCooldown;
        public List<string> compatibleScenes = new List<string>();
    }

    /// <summary>
    /// 成就记录
    /// </summary>
    [System.Serializable]
    public class AchievementRecord
    {
        public string achievementId;
        public string achievementName;
        public string description;
        public bool isUnlocked;
        public DateTime? unlockTime;
        public int rewardCoins;
        public string rewardItemId;
    }

    /// <summary>
    /// 玩家角色数据 - 嘉宾模式使用
    /// </summary>
    [System.Serializable]
    public class GuestPlayerData
    {
        [Header("基础信息")]
        public string playerId;
        public string playerName;
        public CharacterGender gender;
        public int age;
        public string occupation;
        public string personality;
        public string avatar;

        [Header("角色设定")]
        public string backstory;
        public List<string> strengths = new List<string>();
        public List<string> weaknesses = new List<string>();
        public List<string> interests = new List<string>();
        public List<string> dealBreakers = new List<string>();

        [Header("状态数据")]
        public int health = 100;
        public int maxHealth = 100;
        public int energy = 100;
        public int maxEnergy = 100;
        public int mood = 70;
        public int maxMood = 100;
        public int stress = 0;
        public int hunger = 100;
        public int socialBattery = 100;

        [Header("资源")]
        public int coins = 1000;
        public int gems = 0;
        public int popularity = 0;
        public int fans = 0;

        [Header("能力数据")]
        public PlayerAbilityData abilities = new PlayerAbilityData();

        [Header("特殊能力")]
        public List<PlayerSpecialAbility> specialAbilities = new List<PlayerSpecialAbility>();

        [Header("角色标签")]
        public List<string> characterTags = new List<string>();
        public List<string> unlockedTraits = new List<string>();

        [Header("进度数据")]
        public int currentEpisode;
        public int currentDay;
        public int currentWeek;
        public GamePhase currentPhase;
        public List<string> completedTutorials = new List<string>();

        [Header("社交数据")]
        public string currentCrush;
        public List<string> previousCrushes = new List<string>();
        public List<string> blacklistedCharacters = new List<string>();

        [Header("约会历史")]
        public int totalDates;
        public int successfulDates;
        public int failedDates;
        public int confessionsAttempted;
        public int confessionsSuccessful;
        public List<DateHistoryEntry> dateHistory = new List<DateHistoryEntry>();

        [Header("成就")]
        public List<AchievementRecord> achievements = new List<AchievementRecord>();
        public int totalAchievementPoints;

        [Header("游戏统计")]
        public int totalPlayTime;
        public int totalInteractions;
        public int totalGiftsGiven;
        public int totalGiftsReceived;
        public Dictionary<string, int> choiceHistory = new Dictionary<string, int>();
    }

    /// <summary>
    /// 游戏阶段
    /// </summary>
    public enum GamePhase
    {
        Tutorial,
        Introduction,
        GroupPhase,
        DateSelection,
        IndividualDate,
        Elimination,
        SemiFinal,
        Final,
        Ending
    }

    /// <summary>
    /// 约会历史记录
    /// </summary>
    [System.Serializable]
    public class DateHistoryEntry
    {
        public string entryId;
        public DateTime dateTime;
        public string partnerId;
        public string sceneId;
        public DateQuality quality;
        public int moodChange;
        public int relationshipChange;
        public string memorableMoment;
        public List<string> choices = new List<string>();
    }

    /// <summary>
    /// 玩家数据验证器
    /// </summary>
    public static class GuestPlayerDataValidator
    {
        public static bool ValidatePlayerData(GuestPlayerData data, out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(data.playerId))
            {
                errors.Add("玩家ID不能为空");
            }

            if (string.IsNullOrEmpty(data.playerName))
            {
                errors.Add("玩家名称不能为空");
            }

            if (data.age < 18 || data.age > 60)
            {
                errors.Add("年龄需要在18-60之间");
            }

            if (data.health < 0 || data.health > data.maxHealth)
            {
                errors.Add("健康值超出范围");
            }

            if (data.energy < 0 || data.energy > data.maxEnergy)
            {
                errors.Add("精力值超出范围");
            }

            return errors.Count == 0;
        }

        public static void ClampPlayerStats(GuestPlayerData data)
        {
            data.health = Mathf.Clamp(data.health, 0, data.maxHealth);
            data.energy = Mathf.Clamp(data.energy, 0, data.maxEnergy);
            data.mood = Mathf.Clamp(data.mood, 0, data.maxMood);
            data.stress = Mathf.Max(0, data.stress);
            data.hunger = Mathf.Clamp(data.hunger, 0, 100);
            data.socialBattery = Mathf.Clamp(data.socialBattery, 0, 100);
            data.coins = Mathf.Max(0, data.coins);
            data.gems = Mathf.Max(0, data.gems);
            data.popularity = Mathf.Max(0, data.popularity);
        }
    }

    /// <summary>
    /// 玩家能力提升计算器
    /// </summary>
    public static class PlayerAbilityCalculator
    {
        public static int CalculateAbilityLevel(int experience)
        {
            return Mathf.FloorToInt(Mathf.Sqrt(experience / 10f));
        }

        public static int CalculateExperienceForNextLevel(int currentLevel)
        {
            return 10 * (currentLevel + 1) * (currentLevel + 1);
        }

        public static float CalculateCharmBonus(int charm)
        {
            return 1f + (charm / 200f);
        }

        public static float CalculateSocialSkillMultiplier(int socialSkill)
        {
            return 1f + (socialSkill / 100f);
        }

        public static int CalculateRomanceSuccessChance(int romanticSkill, int partnerMood, int atmosphereBonus)
        {
            int baseChance = 50;
            int skillBonus = romanticSkill / 2;
            int moodBonus = partnerMood / 4;
            return Mathf.Clamp(baseChance + skillBonus + moodBonus + atmosphereBonus, 0, 100);
        }
    }
}
