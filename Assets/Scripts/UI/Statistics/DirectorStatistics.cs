using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 导演模式统计 - 管理导演相关的所有统计数据
    /// </summary>
    public class DirectorStatistics
    {
        private float realTimeHeat;
        private float viewership;
        private float viewershipChange;
        private int danmakuCount;
        private float danmakuChange;
        private long topicViews;
        private float douyinHeat;
        private float rating;
        private int revenue;
        private int actionPoints;
        private int maxActionPoints;
        private float heatTrend;
        
        private List<DirectorStatsRecord> heatHistory;
        private List<DirectorStatsRecord> viewershipHistory;
        private List<DirectorStatsRecord> danmakuHistory;
        private List<DirectorStatsRecord> ratingHistory;
        
        private Dictionary<string, GuestPopularity> guestPopularityDict;
        
        public float RealTimeHeat => realTimeHeat;
        public float Viewership => viewership;
        public float ViewershipChange => viewershipChange;
        public int DanmakuCount => danmakuCount;
        public float DanmakuChange => danmakuChange;
        public long TopicViews => topicViews;
        public float DouyinHeat => douyinHeat;
        public float Rating => rating;
        public int Revenue => revenue;
        public int ActionPoints => actionPoints;
        public int MaxActionPoints => maxActionPoints;
        public float HeatTrend => heatTrend;
        public float ActionPointConsumptionRate => maxActionPoints > 0 ? (float)(maxActionPoints - actionPoints) / maxActionPoints * 100f : 0f;
        
        public DirectorStatistics()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            realTimeHeat = 50f;
            viewership = 0.5f;
            viewershipChange = 0f;
            danmakuCount = 0;
            danmakuChange = 0f;
            topicViews = 0;
            douyinHeat = 0f;
            rating = 7.0f;
            revenue = 0;
            actionPoints = 100;
            maxActionPoints = 100;
            heatTrend = 0f;
            
            heatHistory = new List<DirectorStatsRecord>();
            viewershipHistory = new List<DirectorStatsRecord>();
            danmakuHistory = new List<DirectorStatsRecord>();
            ratingHistory = new List<DirectorStatsRecord>();
            
            guestPopularityDict = new Dictionary<string, GuestPopularity>();
        }
        
        /// <summary>
        /// 初始化嘉宾热度
        /// </summary>
        public void InitializeGuestPopularity(string guestId, string guestName)
        {
            if (!guestPopularityDict.ContainsKey(guestId))
            {
                guestPopularityDict[guestId] = new GuestPopularity
                {
                    guestId = guestId,
                    guestName = guestName,
                    popularity = 50f,
                    trend = TrendDirection.Stable
                };
            }
        }
        
        /// <summary>
        /// 获取嘉宾热度
        /// </summary>
        public GuestPopularity GetGuestPopularity(string guestId)
        {
            if (guestPopularityDict.ContainsKey(guestId))
            {
                return guestPopularityDict[guestId];
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有嘉宾热度排名
        /// </summary>
        public List<GuestPopularity> GetGuestPopularityRanking()
        {
            List<GuestPopularity> rankings = new List<GuestPopularity>();
            foreach (var kvp in guestPopularityDict)
            {
                rankings.Add(kvp.Value);
            }
            rankings.Sort((a, b) => b.popularity.CompareTo(a.popularity));
            
            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].rank = i + 1;
            }
            
            return rankings;
        }
        
        /// <summary>
        /// 更新实时热度
        /// </summary>
        public void UpdateRealTimeHeat(float delta)
        {
            float previousHeat = realTimeHeat;
            realTimeHeat = Mathf.Clamp(realTimeHeat + delta, 0f, 100f);
            heatTrend = realTimeHeat - previousHeat;
            
            AddHeatRecord();
        }
        
        /// <summary>
        /// 设置实时热度
        /// </summary>
        public void SetRealTimeHeat(float value)
        {
            float previousHeat = realTimeHeat;
            realTimeHeat = Mathf.Clamp(value, 0f, 100f);
            heatTrend = realTimeHeat - previousHeat;
        }
        
        /// <summary>
        /// 增加弹幕数
        /// </summary>
        public void AddDanmaku(int count)
        {
            danmakuCount += count;
            realTimeHeat += count * 0.001f;
            AddDanmakuRecord();
        }
        
        /// <summary>
        /// 更新收视率
        /// </summary>
        public void UpdateViewership(float delta)
        {
            float previousViewership = viewership;
            viewership = Mathf.Clamp(viewership + delta, 0f, 10f);
            viewershipChange = viewership - previousViewership;
            
            AddViewershipRecord();
        }
        
        /// <summary>
        /// 设置收视率
        /// </summary>
        public void SetViewership(float value)
        {
            float previousViewership = viewership;
            viewership = Mathf.Clamp(value, 0f, 10f);
            viewershipChange = viewership - previousViewership;
        }
        
        /// <summary>
        /// 更新话题阅读量
        /// </summary>
        public void AddTopicViews(long views)
        {
            topicViews += views;
        }
        
        /// <summary>
        /// 设置话题阅读量
        /// </summary>
        public void SetTopicViews(long views)
        {
            topicViews = views;
        }
        
        /// <summary>
        /// 更新抖音热度
        /// </summary>
        public void UpdateDouyinHeat(float delta)
        {
            douyinHeat = Mathf.Clamp(douyinHeat + delta, 0f, 100f);
        }
        
        /// <summary>
        /// 设置抖音热度
        /// </summary>
        public void SetDouyinHeat(float value)
        {
            douyinHeat = Mathf.Clamp(value, 0f, 100f);
        }
        
        /// <summary>
        /// 更新口碑评分
        /// </summary>
        public void UpdateRating(float delta)
        {
            rating = Mathf.Clamp(rating + delta, 0f, 10f);
            AddRatingRecord();
        }
        
        /// <summary>
        /// 设置口碑评分
        /// </summary>
        public void SetRating(float value)
        {
            rating = Mathf.Clamp(value, 0f, 10f);
        }
        
        /// <summary>
        /// 增加收益
        /// </summary>
        public void AddRevenue(int amount)
        {
            revenue += amount;
        }
        
        /// <summary>
        /// 设置收益
        /// </summary>
        public void SetRevenue(int value)
        {
            revenue = value;
        }
        
        /// <summary>
        /// 使用行动点
        /// </summary>
        public bool UseActionPoint(int amount)
        {
            if (actionPoints >= amount)
            {
                actionPoints -= amount;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 恢复行动点
        /// </summary>
        public void RestoreActionPoint(int amount)
        {
            actionPoints = Mathf.Min(actionPoints + amount, maxActionPoints);
        }
        
        /// <summary>
        /// 设置行动点上限
        /// </summary>
        public void SetMaxActionPoints(int max)
        {
            maxActionPoints = max;
            actionPoints = Mathf.Min(actionPoints, maxActionPoints);
        }
        
        /// <summary>
        /// 更新嘉宾热度
        /// </summary>
        public void UpdateGuestPopularity(string guestId, float delta)
        {
            if (guestPopularityDict.ContainsKey(guestId))
            {
                GuestPopularity popularity = guestPopularityDict[guestId];
                float previousPopularity = popularity.popularity;
                popularity.popularity = Mathf.Clamp(popularity.popularity + delta, 0f, 100f);
                popularity.trend = CalculateTrendDirection(popularity.popularity - previousPopularity);
            }
        }
        
        /// <summary>
        /// 设置嘉宾热度
        /// </summary>
        public void SetGuestPopularity(string guestId, float value)
        {
            if (guestPopularityDict.ContainsKey(guestId))
            {
                GuestPopularity popularity = guestPopularityDict[guestId];
                float previousPopularity = popularity.popularity;
                popularity.popularity = Mathf.Clamp(value, 0f, 100f);
                popularity.trend = CalculateTrendDirection(popularity.popularity - previousPopularity);
            }
        }
        
        private TrendDirection CalculateTrendDirection(float delta)
        {
            if (delta > 2f)
                return TrendDirection.Rising;
            else if (delta < -2f)
                return TrendDirection.Falling;
            else
                return TrendDirection.Stable;
        }
        
        private void AddHeatRecord()
        {
            DirectorStatsRecord record = new DirectorStatsRecord
            {
                value = realTimeHeat,
                timestamp = DateTime.Now
            };
            heatHistory.Add(record);
            
            if (heatHistory.Count > 1000)
            {
                heatHistory.RemoveAt(0);
            }
        }
        
        private void AddViewershipRecord()
        {
            DirectorStatsRecord record = new DirectorStatsRecord
            {
                value = viewership,
                timestamp = DateTime.Now
            };
            viewershipHistory.Add(record);
            
            if (viewershipHistory.Count > 1000)
            {
                viewershipHistory.RemoveAt(0);
            }
        }
        
        private void AddDanmakuRecord()
        {
            DirectorStatsRecord record = new DirectorStatsRecord
            {
                value = danmakuCount,
                timestamp = DateTime.Now
            };
            danmakuHistory.Add(record);
            
            if (danmakuHistory.Count > 1000)
            {
                danmakuHistory.RemoveAt(0);
            }
        }
        
        private void AddRatingRecord()
        {
            DirectorStatsRecord record = new DirectorStatsRecord
            {
                value = rating,
                timestamp = DateTime.Now
            };
            ratingHistory.Add(record);
            
            if (ratingHistory.Count > 1000)
            {
                ratingHistory.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// 获取热度历史
        /// </summary>
        public List<DirectorStatsRecord> GetHeatHistory(int count = -1)
        {
            if (count < 0 || count >= heatHistory.Count)
                return new List<DirectorStatsRecord>(heatHistory);
            return new List<DirectorStatsRecord>(heatHistory.GetRange(heatHistory.Count - count, count));
        }
        
        /// <summary>
        /// 获取收视率历史
        /// </summary>
        public List<DirectorStatsRecord> GetViewershipHistory(int count = -1)
        {
            if (count < 0 || count >= viewershipHistory.Count)
                return new List<DirectorStatsRecord>(viewershipHistory);
            return new List<DirectorStatsRecord>(viewershipHistory.GetRange(viewershipHistory.Count - count, count));
        }
        
        /// <summary>
        /// 更新所有数据
        /// </summary>
        public void Update()
        {
            danmakuChange = 0f;
            foreach (var record in danmakuHistory)
            {
                if ((DateTime.Now - record.timestamp).TotalSeconds < 60)
                {
                    danmakuChange++;
                }
            }
        }
        
        /// <summary>
        /// 重置所有数据
        /// </summary>
        public void Reset()
        {
            Initialize();
            guestPopularityDict.Clear();
        }
        
        /// <summary>
        /// 生成保存数据
        /// </summary>
        public DirectorStatsSaveData GetSaveData()
        {
            DirectorStatsSaveData saveData = new DirectorStatsSaveData
            {
                realTimeHeat = realTimeHeat,
                viewership = viewership,
                danmakuCount = danmakuCount,
                topicViews = topicViews,
                douyinHeat = douyinHeat,
                rating = rating,
                revenue = revenue,
                actionPoints = actionPoints,
                maxActionPoints = maxActionPoints
            };
            
            foreach (var kvp in guestPopularityDict)
            {
                saveData.guestPopularityList.Add(kvp.Value);
            }
            
            return saveData;
        }
        
        /// <summary>
        /// 从保存数据加载
        /// </summary>
        public void LoadFromSaveData(DirectorStatsSaveData saveData)
        {
            if (saveData == null)
                return;
            
            realTimeHeat = saveData.realTimeHeat;
            viewership = saveData.viewership;
            danmakuCount = saveData.danmakuCount;
            topicViews = saveData.topicViews;
            douyinHeat = saveData.douyinHeat;
            rating = saveData.rating;
            revenue = saveData.revenue;
            actionPoints = saveData.actionPoints;
            maxActionPoints = saveData.maxActionPoints;
            
            guestPopularityDict.Clear();
            foreach (var popularity in saveData.guestPopularityList)
            {
                guestPopularityDict[popularity.guestId] = popularity;
            }
        }
        
        /// <summary>
        /// 生成统计报告
        /// </summary>
        public DirectorStatisticsReport GenerateReport()
        {
            DirectorStatisticsReport report = new DirectorStatisticsReport
            {
                realTimeHeat = realTimeHeat,
                viewership = viewership,
                danmakuCount = danmakuCount,
                rating = rating,
                revenue = revenue
            };
            
            return report;
        }
    }
    
    /// <summary>
    /// 导演统计数据记录
    /// </summary>
    [Serializable]
    public class DirectorStatsRecord
    {
        public float value;
        public DateTime timestamp;
    }
    
    /// <summary>
    /// 嘉宾热度数据
    /// </summary>
    [Serializable]
    public class GuestPopularity
    {
        public string guestId;
        public string guestName;
        public float popularity;
        public TrendDirection trend;
        public int rank;
        public List<DirectorStatsRecord> history;
        
        public GuestPopularity()
        {
            history = new List<DirectorStatsRecord>();
            trend = TrendDirection.Stable;
        }
    }
    
    /// <summary>
    /// 导演统计保存数据
    /// </summary>
    [Serializable]
    public class DirectorStatsSaveData
    {
        public float realTimeHeat;
        public float viewership;
        public int danmakuCount;
        public long topicViews;
        public float douyinHeat;
        public float rating;
        public int revenue;
        public int actionPoints;
        public int maxActionPoints;
        public List<GuestPopularity> guestPopularityList;
        
        public DirectorStatsSaveData()
        {
            guestPopularityList = new List<GuestPopularity>();
        }
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
}
