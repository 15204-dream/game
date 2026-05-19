using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 嘉宾模式统计 - 管理嘉宾相关的所有统计数据
    /// </summary>
    public class GuestStatistics
    {
        private Dictionary<string, GuestStatsData> guestStatsDict;
        private string currentTargetGuestId;
        private int currentDay;
        private int totalDays;
        private int completedEvents;
        private int totalEvents;
        private int achievedEndings;
        private int totalEndings;
        private int totalConversations;
        private int totalDates;
        private int totalTasks;
        private int completedTasks;
        
        public string CurrentTargetGuestId => currentTargetGuestId;
        public int CurrentDay => currentDay;
        public int TotalDays => totalDays;
        public float DayProgress => totalDays > 0 ? (float)currentDay / totalDays * 100f : 0f;
        public int CompletedEvents => completedEvents;
        public int TotalEvents => totalEvents;
        public float EventProgress => totalEvents > 0 ? (float)completedEvents / totalEvents * 100f : 0f;
        public int AchievedEndings => achievedEndings;
        public int TotalEndings => totalEndings;
        public float EndingRate => totalEndings > 0 ? (float)achievedEndings / totalEndings * 100f : 0f;
        public int TotalConversations => totalConversations;
        public int TotalDates => totalDates;
        public int TotalTasks => totalTasks;
        public int CompletedTasks => completedTasks;
        public float TaskCompletionRate => totalTasks > 0 ? (float)completedTasks / totalTasks * 100f : 0f;
        
        public GuestStatistics()
        {
            guestStatsDict = new Dictionary<string, GuestStatsData>();
            currentDay = 1;
            totalDays = 21;
            totalEvents = 100;
            totalEndings = 5;
        }
        
        /// <summary>
        /// 初始化嘉宾统计数据
        /// </summary>
        public void InitializeGuest(string guestId, string guestName, GuestStatsData config)
        {
            if (!guestStatsDict.ContainsKey(guestId))
            {
                GuestStatsData stats = new GuestStatsData
                {
                    guestId = guestId,
                    guestName = guestName,
                    heartIndex = 0f,
                    focusRate = 0f,
                    socialActivity = 0f,
                    affectionLevel = 0f,
                    conversationCount = 0,
                    dateCount = 0,
                    heartCount = 0,
                    lastInteractionTime = DateTime.Now
                };
                
                if (config != null)
                {
                    stats.maxHeartIndex = config.maxHeartIndex;
                    stats.focusDecayRate = config.focusDecayRate;
                    stats.socialMultiplier = config.socialMultiplier;
                }
                
                guestStatsDict[guestId] = stats;
            }
        }
        
        /// <summary>
        /// 获取嘉宾统计数据
        /// </summary>
        public GuestStatsData GetGuestStats(string guestId)
        {
            if (guestStatsDict.ContainsKey(guestId))
            {
                return guestStatsDict[guestId];
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有嘉宾统计数据
        /// </summary>
        public Dictionary<string, GuestStatsData> GetAllGuestStats()
        {
            return new Dictionary<string, GuestStatsData>(guestStatsDict);
        }
        
        /// <summary>
        /// 设置当前攻略目标
        /// </summary>
        public void SetCurrentTarget(string guestId)
        {
            currentTargetGuestId = guestId;
        }
        
        /// <summary>
        /// 更新当前天数
        /// </summary>
        public void UpdateDay(int day)
        {
            currentDay = day;
        }
        
        /// <summary>
        /// 增加对话次数
        /// </summary>
        public void AddConversation(string guestId)
        {
            if (guestStatsDict.ContainsKey(guestId))
            {
                guestStatsDict[guestId].conversationCount++;
                guestStatsDict[guestId].lastInteractionTime = DateTime.Now;
                totalConversations++;
                
                UpdateHeartIndex(guestId);
            }
        }
        
        /// <summary>
        /// 增加约会次数
        /// </summary>
        public void AddDate(string guestId, float rating)
        {
            if (guestStatsDict.ContainsKey(guestId))
            {
                guestStatsDict[guestId].dateCount++;
                guestStatsDict[guestId].lastInteractionTime = DateTime.Now;
                totalDates++;
                
                guestStatsDict[guestId].affectionLevel += rating * 0.1f;
                guestStatsDict[guestId].affectionLevel = Mathf.Clamp(guestStatsDict[guestId].affectionLevel, 0f, 100f);
                
                UpdateHeartIndex(guestId);
            }
        }
        
        /// <summary>
        /// 增加心动次数
        /// </summary>
        public void AddHeart(string guestId)
        {
            if (guestStatsDict.ContainsKey(guestId))
            {
                guestStatsDict[guestId].heartCount++;
                UpdateHeartIndex(guestId);
            }
        }
        
        /// <summary>
        /// 更新心动指数
        /// </summary>
        private void UpdateHeartIndex(string guestId)
        {
            if (guestStatsDict.ContainsKey(guestId))
            {
                GuestStatsData stats = guestStatsDict[guestId];
                stats.heartIndex = StatisticsCalculator.CalculateHeartIndex(new GuestStatisticsData(stats.guestId, stats.guestName)
                {
                    affectionLevel = stats.affectionLevel,
                    conversationCount = stats.conversationCount,
                    dateCount = stats.dateCount,
                    heartCount = stats.heartCount
                });
            }
        }
        
        /// <summary>
        /// 更新专注度
        /// </summary>
        public void UpdateFocusRate(string guestId, int totalInteractions, int focusInteractions)
        {
            if (guestStatsDict.ContainsKey(guestId))
            {
                guestStatsDict[guestId].focusRate = StatisticsCalculator.CalculateFocusRate(totalInteractions, focusInteractions);
            }
        }
        
        /// <summary>
        /// 更新社交活跃度
        /// </summary>
        public void UpdateSocialActivity(string guestId)
        {
            if (guestStatsDict.ContainsKey(guestId))
            {
                List<GuestStatisticsData> allGuests = new List<GuestStatisticsData>();
                foreach (var kvp in guestStatsDict)
                {
                    allGuests.Add(new GuestStatisticsData(kvp.Value.guestId, kvp.Value.guestName)
                    {
                        conversationCount = kvp.Value.conversationCount,
                        dateCount = kvp.Value.dateCount
                    });
                }
                
                guestStatsDict[guestId].socialActivity = StatisticsCalculator.CalculateSocialActivity(allGuests, guestId);
            }
        }
        
        /// <summary>
        /// 增加完成事件数
        /// </summary>
        public void AddCompletedEvent()
        {
            completedEvents++;
        }
        
        /// <summary>
        /// 设置总事件数
        /// </summary>
        public void SetTotalEvents(int count)
        {
            totalEvents = count;
        }
        
        /// <summary>
        /// 增加达成结局数
        /// </summary>
        public void AddAchievedEnding()
        {
            achievedEndings++;
        }
        
        /// <summary>
        /// 设置总结局数
        /// </summary>
        public void SetTotalEndings(int count)
        {
            totalEndings = count;
        }
        
        /// <summary>
        /// 增加完成任务数
        /// </summary>
        public void AddCompletedTask()
        {
            completedTasks++;
        }
        
        /// <summary>
        /// 设置总任务数
        /// </summary>
        public void SetTotalTasks(int count)
        {
            totalTasks = count;
        }
        
        /// <summary>
        /// 获取目标嘉宾的统计数据
        /// </summary>
        public GuestStatsData GetTargetGuestStats()
        {
            if (!string.IsNullOrEmpty(currentTargetGuestId) && guestStatsDict.ContainsKey(currentTargetGuestId))
            {
                return guestStatsDict[currentTargetGuestId];
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有嘉宾的好感度排名
        /// </summary>
        public List<GuestRanking> GetAffectionRanking()
        {
            List<GuestStatisticsData> guests = new List<GuestStatisticsData>();
            foreach (var kvp in guestStatsDict)
            {
                guests.Add(new GuestStatisticsData(kvp.Value.guestId, kvp.Value.guestName)
                {
                    affectionLevel = kvp.Value.affectionLevel,
                    conversationCount = kvp.Value.conversationCount,
                    dateCount = kvp.Value.dateCount,
                    heartCount = kvp.Value.heartCount
                });
            }
            
            return StatisticsCalculator.CalculateRanking(guests);
        }
        
        /// <summary>
        /// 更新所有嘉宾的统计数据
        /// </summary>
        public void Update()
        {
            foreach (var kvp in guestStatsDict)
            {
                UpdateHeartIndex(kvp.Key);
                UpdateSocialActivity(kvp.Key);
            }
        }
        
        /// <summary>
        /// 重置所有统计数据
        /// </summary>
        public void Reset()
        {
            guestStatsDict.Clear();
            currentTargetGuestId = "";
            currentDay = 1;
            completedEvents = 0;
            achievedEndings = 0;
            totalConversations = 0;
            totalDates = 0;
            completedTasks = 0;
        }
        
        /// <summary>
        /// 生成保存数据
        /// </summary>
        public GuestStatsSaveData GetSaveData()
        {
            GuestStatsSaveData saveData = new GuestStatsSaveData
            {
                currentTargetGuestId = currentTargetGuestId,
                currentDay = currentDay,
                totalDays = totalDays,
                completedEvents = completedEvents,
                totalEvents = totalEvents,
                achievedEndings = achievedEndings,
                totalEndings = totalEndings,
                totalConversations = totalConversations,
                totalDates = totalDates,
                completedTasks = completedTasks,
                totalTasks = totalTasks
            };
            
            foreach (var kvp in guestStatsDict)
            {
                saveData.guestStatsList.Add(kvp.Value);
            }
            
            return saveData;
        }
        
        /// <summary>
        /// 从保存数据加载
        /// </summary>
        public void LoadFromSaveData(GuestStatsSaveData saveData)
        {
            if (saveData == null)
                return;
            
            currentTargetGuestId = saveData.currentTargetGuestId;
            currentDay = saveData.currentDay;
            totalDays = saveData.totalDays;
            completedEvents = saveData.completedEvents;
            totalEvents = saveData.totalEvents;
            achievedEndings = saveData.achievedEndings;
            totalEndings = saveData.totalEndings;
            totalConversations = saveData.totalConversations;
            totalDates = saveData.totalDates;
            completedTasks = saveData.completedTasks;
            totalTasks = saveData.totalTasks;
            
            guestStatsDict.Clear();
            foreach (var stats in saveData.guestStatsList)
            {
                guestStatsDict[stats.guestId] = stats;
            }
        }
        
        /// <summary>
        /// 生成统计报告
        /// </summary>
        public GuestStatisticsReport GenerateReport()
        {
            GuestStatisticsReport report = new GuestStatisticsReport();
            
            float totalAffection = 0f;
            float maxAffection = 0f;
            
            foreach (var kvp in guestStatsDict)
            {
                totalAffection += kvp.Value.affectionLevel;
                if (kvp.Value.affectionLevel > maxAffection)
                {
                    maxAffection = kvp.Value.affectionLevel;
                }
            }
            
            report.averageAffection = guestStatsDict.Count > 0 ? totalAffection / guestStatsDict.Count : 0f;
            report.maxAffection = maxAffection;
            report.targetGuestId = currentTargetGuestId;
            report.totalConversations = totalConversations;
            report.totalDates = totalDates;
            
            return report;
        }
    }
    
    /// <summary>
    /// 嘉宾统计数据
    /// </summary>
    [Serializable]
    public class GuestStatsData
    {
        public string guestId;
        public string guestName;
        public float heartIndex;
        public float maxHeartIndex = 100f;
        public float focusRate;
        public float focusDecayRate = 0.1f;
        public float socialActivity;
        public float socialMultiplier = 1f;
        public float affectionLevel;
        public int conversationCount;
        public int dateCount;
        public int heartCount;
        public DateTime lastInteractionTime;
        public List<StatisticsRecord> affectionHistory;
        
        public GuestStatsData()
        {
            affectionHistory = new List<StatisticsRecord>();
            lastInteractionTime = DateTime.Now;
        }
    }
    
    /// <summary>
    /// 嘉宾统计保存数据
    /// </summary>
    [Serializable]
    public class GuestStatsSaveData
    {
        public string currentTargetGuestId;
        public int currentDay;
        public int totalDays;
        public int completedEvents;
        public int totalEvents;
        public int achievedEndings;
        public int totalEndings;
        public int totalConversations;
        public int totalDates;
        public int completedTasks;
        public int totalTasks;
        public List<GuestStatsData> guestStatsList;
        
        public GuestStatsSaveData()
        {
            guestStatsList = new List<GuestStatsData>();
        }
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
}
