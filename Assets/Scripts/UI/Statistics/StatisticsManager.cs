using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 统计管理器 - 统一管理所有统计数据
    /// </summary>
    public class StatisticsManager : MonoBehaviour
    {
        private static StatisticsManager instance;
        public static StatisticsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("StatisticsManager");
                    instance = go.AddComponent<StatisticsManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        
        private Dictionary<string, StatisticsData> statisticsDataDict;
        private GuestStatistics guestStatistics;
        private DirectorStatistics directorStatistics;
        private AchievementManager achievementManager;
        
        private float updateInterval = 0.5f;
        private float lastUpdateTime = 0f;
        
        public GuestStatistics GuestStats => guestStatistics;
        public DirectorStatistics DirectorStats => directorStatistics;
        public AchievementManager AchievementMgr => achievementManager;
        
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
        
        void Update()
        {
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateStatistics();
                lastUpdateTime = Time.time;
            }
        }
        
        private void Initialize()
        {
            statisticsDataDict = new Dictionary<string, StatisticsData>();
            guestStatistics = new GuestStatistics();
            directorStatistics = new DirectorStatistics();
            achievementManager = gameObject.GetComponent<AchievementManager>();
            
            if (achievementManager == null)
            {
                achievementManager = gameObject.AddComponent<AchievementManager>();
            }
            
            InitializeDefaultStatistics();
        }
        
        private void InitializeDefaultStatistics()
        {
            AddStatistics(new StatisticsData("total_play_time", "总游戏时长", 86400, StatisticsType.Time));
            AddStatistics(new StatisticsData("total_events", "总事件数", 1000));
            AddStatistics(new StatisticsData("completed_events", "已完成事件", 500));
            AddStatistics(new StatisticsData("achievement_count", "成就数量", 100));
        }
        
        /// <summary>
        /// 添加统计数据
        /// </summary>
        public void AddStatistics(StatisticsData data)
        {
            if (data == null || string.IsNullOrEmpty(data.id))
                return;
            
            if (!statisticsDataDict.ContainsKey(data.id))
            {
                statisticsDataDict[data.id] = data;
            }
        }
        
        /// <summary>
        /// 获取统计数据
        /// </summary>
        public StatisticsData GetStatistics(string id)
        {
            if (statisticsDataDict.ContainsKey(id))
            {
                return statisticsDataDict[id];
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有统计数据
        /// </summary>
        public Dictionary<string, StatisticsData> GetAllStatistics()
        {
            return new Dictionary<string, StatisticsData>(statisticsDataDict);
        }
        
        /// <summary>
        /// 设置统计数据值
        /// </summary>
        public void SetStatisticsValue(string id, float value)
        {
            StatisticsData data = GetStatistics(id);
            if (data != null)
            {
                data.SetValue(value);
            }
        }
        
        /// <summary>
        /// 增加统计数据值
        /// </summary>
        public void AddStatisticsValue(string id, float amount)
        {
            StatisticsData data = GetStatistics(id);
            if (data != null)
            {
                data.AddValue(amount);
            }
        }
        
        /// <summary>
        /// 更新统计数据的回调
        /// </summary>
        public event Action<string, StatisticsData> OnStatisticsUpdated;
        
        /// <summary>
        /// 触发统计更新事件
        /// </summary>
        protected virtual void OnStatisticsUpdate(string id, StatisticsData data)
        {
            OnStatisticsUpdated?.Invoke(id, data);
        }
        
        /// <summary>
        /// 更新所有统计数据
        /// </summary>
        private void UpdateStatistics()
        {
            foreach (var kvp in statisticsDataDict)
            {
                if (kvp.Value.realTimeUpdate)
                {
                    OnStatisticsUpdate(kvp.Key, kvp.Value);
                }
            }
            
            if (guestStatistics != null)
            {
                guestStatistics.Update();
            }
            
            if (directorStatistics != null)
            {
                directorStatistics.Update();
            }
        }
        
        /// <summary>
        /// 重置所有统计数据
        /// </summary>
        public void ResetAllStatistics()
        {
            foreach (var kvp in statisticsDataDict)
            {
                kvp.Value.Reset();
            }
            
            guestStatistics?.Reset();
            directorStatistics?.Reset();
        }
        
        /// <summary>
        /// 保存统计数据
        /// </summary>
        public void SaveStatistics(string filePath)
        {
            SaveData saveData = new SaveData
            {
                timestamp = DateTime.Now,
                statisticsDataDict = statisticsDataDict,
                guestStatsData = guestStatistics?.GetSaveData(),
                directorStatsData = directorStatistics?.GetSaveData()
            };
            
            string json = JsonUtility.ToJson(saveData, true);
            System.IO.File.WriteAllText(filePath, json);
        }
        
        /// <summary>
        /// 加载统计数据
        /// </summary>
        public void LoadStatistics(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                if (saveData != null)
                {
                    if (saveData.statisticsDataDict != null)
                    {
                        statisticsDataDict = saveData.statisticsDataDict;
                    }
                    
                    if (saveData.guestStatsData != null && guestStatistics != null)
                    {
                        guestStatistics.LoadFromSaveData(saveData.guestStatsData);
                    }
                    
                    if (saveData.directorStatsData != null && directorStatistics != null)
                    {
                        directorStatistics.LoadFromSaveData(saveData.directorStatsData);
                    }
                }
            }
        }
        
        /// <summary>
        /// 导出统计数据报告
        /// </summary>
        public StatisticsReport GenerateReport()
        {
            StatisticsReport report = new StatisticsReport
            {
                generateTime = DateTime.Now,
                totalPlayTime = GetStatistics("total_play_time")?.currentValue ?? 0,
                totalEvents = GetStatistics("total_events")?.currentValue ?? 0,
                completedEvents = GetStatistics("completed_events")?.currentValue ?? 0,
                achievementCount = GetStatistics("achievement_count")?.currentValue ?? 0
            };
            
            if (guestStatistics != null)
            {
                report.guestReport = guestStatistics.GenerateReport();
            }
            
            if (directorStatistics != null)
            {
                report.directorReport = directorStatistics.GenerateReport();
            }
            
            return report;
        }
        
        /// <summary>
        /// 获取统计数据摘要
        /// </summary>
        public StatisticsSummary GetSummary()
        {
            StatisticsSummary summary = new StatisticsSummary();
            
            foreach (var kvp in statisticsDataDict)
            {
                summary.AddStatistic(kvp.Key, kvp.Value.name, kvp.Value.currentValue, kvp.Value.maxValue);
            }
            
            return summary;
        }
    }
    
    /// <summary>
    /// 保存数据结构
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public DateTime timestamp;
        public Dictionary<string, StatisticsData> statisticsDataDict;
        public GuestStatsSaveData guestStatsData;
        public DirectorStatsSaveData directorStatsData;
    }
    
    /// <summary>
    /// 统计数据报告
    /// </summary>
    [Serializable]
    public class StatisticsReport
    {
        public DateTime generateTime;
        public float totalPlayTime;
        public float totalEvents;
        public float completedEvents;
        public float achievementCount;
        public GuestStatisticsReport guestReport;
        public DirectorStatisticsReport directorReport;
    }
    
    /// <summary>
    /// 嘉宾统计报告
    /// </summary>
    [Serializable]
    public class GuestStatisticsReport
    {
        public float averageAffection;
        public float maxAffection;
        public string targetGuestId;
        public int totalConversations;
        public int totalDates;
    }
    
    /// <summary>
    /// 导演统计报告
    /// </summary>
    [Serializable]
    public class DirectorStatisticsReport
    {
        public float realTimeHeat;
        public float viewership;
        public int danmakuCount;
        public float rating;
        public int revenue;
    }
    
    /// <summary>
    /// 统计数据摘要
    /// </summary>
    [Serializable]
    public class StatisticsSummary
    {
        public List<StatisticItem> items;
        
        public StatisticsSummary()
        {
            items = new List<StatisticItem>();
        }
        
        public void AddStatistic(string id, string name, float value, float maxValue)
        {
            items.Add(new StatisticItem
            {
                id = id,
                name = name,
                value = value,
                maxValue = maxValue
            });
        }
    }
    
    /// <summary>
    /// 统计项
    /// </summary>
    [Serializable]
    public class StatisticItem
    {
        public string id;
        public string name;
        public float value;
        public float maxValue;
        public float percentage => maxValue > 0 ? (value / maxValue) * 100f : 0f;
    }
}
