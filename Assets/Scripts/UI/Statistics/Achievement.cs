using System;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 成就数据类 - 定义成就的各种属性
    /// </summary>
    [Serializable]
    public class Achievement
    {
        [Header("基础信息")]
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string icon;
        
        [Header("稀有度")]
        [SerializeField] private AchievementRarity rarity;
        
        [Header("进度配置")]
        [SerializeField] private int currentProgress;
        [SerializeField] private int maxProgress;
        [SerializeField] private bool incremental;
        
        [Header("奖励")]
        [SerializeField] private string reward;
        
        [Header("解锁状态")]
        [SerializeField] private bool isUnlocked;
        [SerializeField] private DateTime? unlockTime;
        
        [Header("解锁条件")]
        [SerializeField] private AchievementCondition condition;
        
        public string Id => id;
        public string Name => name;
        public string Description => description;
        public string Icon => icon;
        public AchievementRarity Rarity => rarity;
        public int CurrentProgress => currentProgress;
        public int MaxProgress => maxProgress;
        public bool IsUnlocked => isUnlocked;
        public DateTime? UnlockTime => unlockTime;
        public string Reward => reward;
        public AchievementCondition Condition => condition;
        
        public float ProgressPercentage => maxProgress > 0 ? (float)currentProgress / maxProgress * 100f : 0f;
        
        public Achievement()
        {
            id = "";
            name = "";
            description = "";
            icon = "🏆";
            rarity = AchievementRarity.Common;
            currentProgress = 0;
            maxProgress = 1;
            incremental = true;
            reward = "";
            isUnlocked = false;
            unlockTime = null;
            condition = new AchievementCondition();
        }
        
        public Achievement(string id, string name, string description, AchievementRarity rarity = AchievementRarity.Common)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.icon = "🏆";
            this.rarity = rarity;
            this.currentProgress = 0;
            this.maxProgress = 1;
            this.incremental = true;
            this.reward = "";
            this.isUnlocked = false;
            this.unlockTime = null;
            this.condition = new AchievementCondition();
        }
        
        /// <summary>
        /// 增加进度
        /// </summary>
        public void AddProgress(int amount = 1)
        {
            if (isUnlocked)
                return;
            
            if (incremental)
            {
                currentProgress = Mathf.Min(currentProgress + amount, maxProgress);
            }
            else
            {
                currentProgress = amount;
            }
            
            CheckUnlock();
        }
        
        /// <summary>
        /// 设置进度
        /// </summary>
        public void SetProgress(int progress)
        {
            if (isUnlocked)
                return;
            
            currentProgress = Mathf.Clamp(progress, 0, maxProgress);
            CheckUnlock();
        }
        
        /// <summary>
        /// 检查是否满足解锁条件
        /// </summary>
        public void CheckUnlock()
        {
            if (isUnlocked)
                return;
            
            if (currentProgress >= maxProgress)
            {
                Unlock();
            }
        }
        
        /// <summary>
        /// 解锁成就
        /// </summary>
        public void Unlock()
        {
            if (isUnlocked)
                return;
            
            isUnlocked = true;
            unlockTime = DateTime.Now;
            currentProgress = maxProgress;
        }
        
        /// <summary>
        /// 重置成就进度
        /// </summary>
        public void Reset()
        {
            currentProgress = 0;
            isUnlocked = false;
            unlockTime = null;
        }
        
        /// <summary>
        /// 复制成就
        /// </summary>
        public Achievement Clone()
        {
            Achievement clone = new Achievement
            {
                id = this.id,
                name = this.name,
                description = this.description,
                icon = this.icon,
                rarity = this.rarity,
                currentProgress = this.currentProgress,
                maxProgress = this.maxProgress,
                incremental = this.incremental,
                reward = this.reward,
                isUnlocked = this.isUnlocked,
                unlockTime = this.unlockTime,
                condition = this.condition.Clone()
            };
            
            return clone;
        }
    }
    
    /// <summary>
    /// 成就稀有度
    /// </summary>
    public enum AchievementRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }
    
    /// <summary>
    /// 成就条件
    /// </summary>
    [Serializable]
    public class AchievementCondition
    {
        [Header("类型条件")]
        public ConditionType type;
        
        [Header("数值条件")]
        public int targetValue;
        public int minValue;
        public int maxValue;
        
        [Header("字符串条件")]
        public string targetString;
        public string[] requiredStrings;
        
        [Header("时间条件")]
        public float minTime;
        public float maxTime;
        
        public AchievementCondition()
        {
            type = ConditionType.None;
            targetValue = 1;
            minValue = 0;
            maxValue = int.MaxValue;
        }
        
        public AchievementCondition Clone()
        {
            return new AchievementCondition
            {
                type = this.type,
                targetValue = this.targetValue,
                minValue = this.minValue,
                maxValue = this.maxValue,
                targetString = this.targetString,
                requiredStrings = this.requiredStrings != null ? (string[])this.requiredStrings.Clone() : null,
                minTime = this.minTime,
                maxTime = this.maxTime
            };
        }
    }
    
    /// <summary>
    /// 条件类型
    /// </summary>
    public enum ConditionType
    {
        None,
        ConversationCount,
        DateCount,
        AffectionLevel,
        HeartCount,
        StoryProgress,
        EndingReached,
        TimePlayed,
        DayReached,
        GuestCount,
        AchievementCount
    }
    
    /// <summary>
    /// 成就统计数据
    /// </summary>
    [Serializable]
    public class AchievementStats
    {
        public int totalAchievements;
        public int unlockedAchievements;
        public int commonUnlocked;
        public int rareUnlocked;
        public int epicUnlocked;
        public int legendaryUnlocked;
        public DateTime firstUnlockTime;
        public DateTime lastUnlockTime;
        
        public float UnlockRate => totalAchievements > 0 ? (float)unlockedAchievements / totalAchievements * 100f : 0f;
    }
}
