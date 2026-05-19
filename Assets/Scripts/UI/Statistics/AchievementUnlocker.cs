using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 成就解锁器 - 自动检查并解锁成就
    /// </summary>
    public class AchievementUnlocker : MonoBehaviour
    {
        [Header("监听的统计数据")]
        [SerializeField] private bool monitorConversations = true;
        [SerializeField] private bool monitorDates = true;
        [SerializeField] private bool monitorHearts = true;
        [SerializeField] private bool monitorAffection = true;
        [SerializeField] private bool monitorStory = true;
        
        [Header("检查间隔")]
        [SerializeField] private float checkInterval = 1f;
        
        [Header("调试模式")]
        [SerializeField] private bool debugMode = false;
        
        private float lastCheckTime;
        private StatisticsManager statisticsManager;
        private AchievementManager achievementManager;
        private GuestStatistics guestStatistics;
        private Dictionary<string, bool> unlockedAchievements;
        
        public event Action<Achievement> OnAchievementUnlocked;
        
        void Start()
        {
            Initialize();
        }
        
        void Update()
        {
            if (Time.time - lastCheckTime >= checkInterval)
            {
                CheckUnlock();
                lastCheckTime = Time.time;
            }
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            statisticsManager = StatisticsManager.Instance;
            achievementManager = AchievementManager.Instance;
            
            if (statisticsManager != null)
            {
                guestStatistics = statisticsManager.GuestStats;
            }
            
            unlockedAchievements = new Dictionary<string, bool>();
            
            if (achievementManager != null)
            {
                achievementManager.RegisterUnlocker(this);
                achievementManager.OnAchievementUnlocked += OnAchievementUnlockedCallback;
            }
        }
        
        void OnDestroy()
        {
            if (achievementManager != null)
            {
                achievementManager.UnregisterUnlocker(this);
                achievementManager.OnAchievementUnlocked -= OnAchievementUnlockedCallback;
            }
        }
        
        /// <summary>
        /// 检查成就解锁
        /// </summary>
        public void CheckUnlock()
        {
            if (achievementManager == null || guestStatistics == null)
                return;
            
            if (monitorConversations)
            {
                CheckConversationAchievements();
            }
            
            if (monitorDates)
            {
                CheckDateAchievements();
            }
            
            if (monitorHearts)
            {
                CheckHeartAchievements();
            }
            
            if (monitorAffection)
            {
                CheckAffectionAchievements();
            }
            
            if (monitorStory)
            {
                CheckStoryAchievements();
            }
        }
        
        /// <summary>
        /// 检查对话相关成就
        /// </summary>
        private void CheckConversationAchievements()
        {
            int totalConversations = guestStatistics.TotalConversations;
            
            CheckAndUnlock("first_conversation", totalConversations >= 1);
            CheckAndUnlock("conversation_master", totalConversations >= 100);
        }
        
        /// <summary>
        /// 检查约会相关成就
        /// </summary>
        private void CheckDateAchievements()
        {
            int totalDates = guestStatistics.TotalDates;
            
            CheckAndUnlock("first_date", totalDates >= 1);
            CheckAndUnlock("date_master", totalDates >= 50);
            
            var allGuestStats = guestStatistics.GetAllGuestStats();
            if (allGuestStats.Count > 0)
            {
                bool allDated = true;
                foreach (var stats in allGuestStats.Values)
                {
                    if (stats.dateCount == 0)
                    {
                        allDated = false;
                        break;
                    }
                }
                CheckAndUnlock("all_guests_dated", allDated);
            }
        }
        
        /// <summary>
        /// 检查心动相关成就
        /// </summary>
        private void CheckHeartAchievements()
        {
            var allGuestStats = guestStatistics.GetAllGuestStats();
            
            int totalHearts = 0;
            foreach (var stats in allGuestStats.Values)
            {
                totalHearts += stats.heartCount;
            }
            
            CheckAndUnlock("first_heart", totalHearts >= 1);
            CheckAndUnlock("heart_collector", totalHearts >= 100);
        }
        
        /// <summary>
        /// 检查好感度相关成就
        /// </summary>
        private void CheckAffectionAchievements()
        {
            var allGuestStats = guestStatistics.GetAllGuestStats();
            
            foreach (var stats in allGuestStats.Values)
            {
                if (stats.affectionLevel >= 100f)
                {
                    CheckAndUnlock("affection_master", true);
                    break;
                }
            }
        }
        
        /// <summary>
        /// 检查剧情相关成就
        /// </summary>
        private void CheckStoryAchievements()
        {
            float storyProgress = guestStatistics.EventProgress;
            
            CheckAndUnlock("story_completer", storyProgress >= 100f);
        }
        
        /// <summary>
        /// 检查并解锁成就
        /// </summary>
        private void CheckAndUnlock(string achievementId, bool condition)
        {
            if (!condition)
                return;
            
            if (unlockedAchievements.ContainsKey(achievementId) && unlockedAchievements[achievementId])
                return;
            
            Achievement achievement = achievementManager.GetAchievement(achievementId);
            if (achievement == null || achievement.IsUnlocked)
                return;
            
            achievementManager.UnlockAchievement(achievementId);
            unlockedAchievements[achievementId] = true;
            
            if (debugMode)
            {
                Debug.Log($"[AchievementUnlocker] 解锁成就: {achievement.Name}");
            }
        }
        
        /// <summary>
        /// 成就解锁回调
        /// </summary>
        private void OnAchievementUnlockedCallback(Achievement achievement)
        {
            unlockedAchievements[achievement.Id] = true;
            OnAchievementUnlocked?.Invoke(achievement);
        }
        
        /// <summary>
        /// 手动触发成就解锁检查
        /// </summary>
        public void ForceCheck()
        {
            lastCheckTime = 0;
            CheckUnlock();
        }
        
        /// <summary>
        /// 重置已解锁记录
        /// </summary>
        public void ResetUnlockedRecord()
        {
            unlockedAchievements.Clear();
        }
    }
    
    /// <summary>
    /// 成就解锁条件
    /// </summary>
    [Serializable]
    public class UnlockCondition
    {
        public string achievementId;
        public ConditionType conditionType;
        public float targetValue;
        public ComparisonOperator comparison;
        
        public enum ComparisonOperator
        {
            Equal,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual
        }
    }
    
    /// <summary>
    /// 条件类型
    /// </summary>
    public enum ConditionType
    {
        Conversation,
        Date,
        Heart,
        Affection,
        Progress,
        Time
    }
    
    /// <summary>
    /// 解锁器配置
    /// </summary>
    [Serializable]
    public class UnlockerConfig
    {
        public bool monitorConversations = true;
        public bool monitorDates = true;
        public bool monitorHearts = true;
        public bool monitorAffection = true;
        public bool monitorStory = true;
        public float checkInterval = 1f;
        public List<UnlockCondition> customConditions;
        
        public static UnlockerConfig CreateDefault()
        {
            return new UnlockerConfig
            {
                monitorConversations = true,
                monitorDates = true,
                monitorHearts = true,
                monitorAffection = true,
                monitorStory = true,
                checkInterval = 1f,
                customConditions = new List<UnlockCondition>()
            };
        }
    }
}
