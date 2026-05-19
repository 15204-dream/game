using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 成就管理器 - 管理所有成就的解锁和进度
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        private static AchievementManager instance;
        public static AchievementManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("AchievementManager");
                    instance = go.AddComponent<AchievementManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        
        private Dictionary<string, Achievement> achievements;
        private List<AchievementUnlocker> unlockers;
        
        public event Action<Achievement> OnAchievementUnlocked;
        public event Action<Achievement> OnAchievementProgressChanged;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Initialize()
        {
            achievements = new Dictionary<string, Achievement>();
            unlockers = new List<AchievementUnlocker>();
            
            CreateDefaultAchievements();
        }
        
        /// <summary>
        /// 创建默认成就
        /// </summary>
        private void CreateDefaultAchievements()
        {
            AddAchievement(new Achievement("first_conversation", "初次见面", "与任意嘉宾进行第一次对话", AchievementRarity.Common));
            AddAchievement(new Achievement("first_date", "约会开始", "与任意嘉宾进行第一次约会", AchievementRarity.Common));
            AddAchievement(new Achievement("first_heart", "心动时刻", "向任意嘉宾送出第一个心动", AchievementRarity.Common));
            
            AddAchievement(new Achievement("conversation_master", "健谈达人", "对话次数达到100次", AchievementRarity.Rare));
            AddAchievement(new Achievement("date_master", "约会高手", "约会次数达到50次", AchievementRarity.Rare));
            AddAchievement(new Achievement("heart_collector", "心动收集者", "送出100个心动", AchievementRarity.Rare));
            
            AddAchievement(new Achievement("affection_master", "感情大师", "某个嘉宾好感度达到100%", AchievementRarity.Epic));
            AddAchievement(new Achievement("all_guests_dated", "人人平等", "与所有嘉宾都约会过", AchievementRarity.Epic));
            AddAchievement(new Achievement("story_completer", "故事完结", "完成所有剧情事件", AchievementRarity.Epic));
            
            AddAchievement(new Achievement("legendary_lover", "传奇恋人", "达成完美结局", AchievementRarity.Legendary));
            AddAchievement(new Achievement("true_love", "真爱永恒", "好感度持续保持最高", AchievementRarity.Legendary));
            AddAchievement(new Achievement("game_master", "游戏大师", "解锁所有成就", AchievementRarity.Legendary));
        }
        
        /// <summary>
        /// 添加成就
        /// </summary>
        public void AddAchievement(Achievement achievement)
        {
            if (achievement == null || string.IsNullOrEmpty(achievement.Id))
                return;
            
            if (!achievements.ContainsKey(achievement.Id))
            {
                achievements[achievement.Id] = achievement;
            }
        }
        
        /// <summary>
        /// 获取成就
        /// </summary>
        public Achievement GetAchievement(string id)
        {
            if (achievements.ContainsKey(id))
            {
                return achievements[id];
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有成就
        /// </summary>
        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(achievements.Values);
        }
        
        /// <summary>
        /// 获取已解锁成就
        /// </summary>
        public List<Achievement> GetUnlockedAchievements()
        {
            List<Achievement> unlocked = new List<Achievement>();
            foreach (var achievement in achievements.Values)
            {
                if (achievement.IsUnlocked)
                {
                    unlocked.Add(achievement);
                }
            }
            return unlocked;
        }
        
        /// <summary>
        /// 获取未解锁成就
        /// </summary>
        public List<Achievement> GetLockedAchievements()
        {
            List<Achievement> locked = new List<Achievement>();
            foreach (var achievement in achievements.Values)
            {
                if (!achievement.IsUnlocked)
                {
                    locked.Add(achievement);
                }
            }
            return locked;
        }
        
        /// <summary>
        /// 获取已解锁成就数量
        /// </summary>
        public int GetUnlockedCount()
        {
            int count = 0;
            foreach (var achievement in achievements.Values)
            {
                if (achievement.IsUnlocked)
                {
                    count++;
                }
            }
            return count;
        }
        
        /// <summary>
        /// 获取总成就数量
        /// </summary>
        public int GetTotalCount()
        {
            return achievements.Count;
        }
        
        /// <summary>
        /// 更新成就进度
        /// </summary>
        public void UpdateProgress(string achievementId, int progress)
        {
            Achievement achievement = GetAchievement(achievementId);
            if (achievement == null || achievement.IsUnlocked)
                return;
            
            int previousProgress = achievement.CurrentProgress;
            achievement.SetProgress(progress);
            
            OnAchievementProgressChanged?.Invoke(achievement);
            
            if (achievement.CurrentProgress != previousProgress && achievement.IsUnlocked)
            {
                OnAchievementUnlocked?.Invoke(achievement);
            }
        }
        
        /// <summary>
        /// 增加成就进度
        /// </summary>
        public void AddProgress(string achievementId, int amount = 1)
        {
            Achievement achievement = GetAchievement(achievementId);
            if (achievement == null || achievement.IsUnlocked)
                return;
            
            int previousProgress = achievement.CurrentProgress;
            achievement.AddProgress(amount);
            
            OnAchievementProgressChanged?.Invoke(achievement);
            
            if (achievement.CurrentProgress != previousProgress && achievement.IsUnlocked)
            {
                OnAchievementUnlocked?.Invoke(achievement);
            }
        }
        
        /// <summary>
        /// 解锁成就
        /// </summary>
        public void UnlockAchievement(string achievementId)
        {
            Achievement achievement = GetAchievement(achievementId);
            if (achievement == null || achievement.IsUnlocked)
                return;
            
            achievement.Unlock();
            OnAchievementUnlocked?.Invoke(achievement);
        }
        
        /// <summary>
        /// 注册解锁器
        /// </summary>
        public void RegisterUnlocker(AchievementUnlocker unlocker)
        {
            if (unlocker != null && !unlockers.Contains(unlocker))
            {
                unlockers.Add(unlocker);
            }
        }
        
        /// <summary>
        /// 注销解锁器
        /// </summary>
        public void UnregisterUnlocker(AchievementUnlocker unlocker)
        {
            if (unlocker != null)
            {
                unlockers.Remove(unlocker);
            }
        }
        
        /// <summary>
        /// 检查所有解锁器
        /// </summary>
        public void CheckAllUnlockers()
        {
            foreach (var unlocker in unlockers)
            {
                if (unlocker != null)
                {
                    unlocker.CheckUnlock();
                }
            }
        }
        
        /// <summary>
        /// 获取成就统计
        /// </summary>
        public AchievementStats GetStats()
        {
            AchievementStats stats = new AchievementStats
            {
                totalAchievements = achievements.Count,
                unlockedAchievements = GetUnlockedCount()
            };
            
            foreach (var achievement in achievements.Values)
            {
                if (achievement.IsUnlocked)
                {
                    switch (achievement.Rarity)
                    {
                        case AchievementRarity.Common: stats.commonUnlocked++; break;
                        case AchievementRarity.Rare: stats.rareUnlocked++; break;
                        case AchievementRarity.Epic: stats.epicUnlocked++; break;
                        case AchievementRarity.Legendary: stats.legendaryUnlocked++; break;
                    }
                    
                    if (!stats.firstUnlockTime.HasValue)
                    {
                        stats.firstUnlockTime = achievement.UnlockTime.Value;
                    }
                    stats.lastUnlockTime = achievement.UnlockTime.Value;
                }
            }
            
            return stats;
        }
        
        /// <summary>
        /// 重置所有成就
        /// </summary>
        public void ResetAllAchievements()
        {
            foreach (var achievement in achievements.Values)
            {
                achievement.Reset();
            }
        }
        
        /// <summary>
        /// 保存成就数据
        /// </summary>
        public void SaveAchievements(string filePath)
        {
            List<AchievementSaveData> saveData = new List<AchievementSaveData>();
            
            foreach (var achievement in achievements.Values)
            {
                saveData.Add(new AchievementSaveData
                {
                    id = achievement.Id,
                    currentProgress = achievement.CurrentProgress,
                    isUnlocked = achievement.IsUnlocked,
                    unlockTime = achievement.UnlockTime
                });
            }
            
            string json = JsonUtility.ToJson(new AchievementSaveWrapper { achievements = saveData }, true);
            System.IO.File.WriteAllText(filePath, json);
        }
        
        /// <summary>
        /// 加载成就数据
        /// </summary>
        public void LoadAchievements(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                AchievementSaveWrapper wrapper = JsonUtility.FromJson<AchievementSaveWrapper>(json);
                
                if (wrapper != null && wrapper.achievements != null)
                {
                    foreach (var saveData in wrapper.achievements)
                    {
                        Achievement achievement = GetAchievement(saveData.id);
                        if (achievement != null)
                        {
                            achievement.SetProgress(saveData.currentProgress);
                            if (saveData.isUnlocked && !achievement.IsUnlocked)
                            {
                                achievement.Unlock();
                            }
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 成就保存数据
    /// </summary>
    [Serializable]
    public class AchievementSaveData
    {
        public string id;
        public int currentProgress;
        public bool isUnlocked;
        public DateTime? unlockTime;
    }
    
    /// <summary>
    /// 成就保存数据包装器
    /// </summary>
    [Serializable]
    public class AchievementSaveWrapper
    {
        public List<AchievementSaveData> achievements;
    }
}
