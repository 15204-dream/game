using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 导演统计数据配置 - 用于配置导演的初始统计参数
    /// </summary>
    [CreateAssetMenu(fileName = "DirectorStatsConfig", menuName = "DatingShow/DirectorStatsConfig")]
    public class DirectorStatsDataConfig : ScriptableObject
    {
        [Header("初始数据配置")]
        [SerializeField] private float initialHeat = 50f;
        [SerializeField] private float initialViewership = 0.5f;
        [SerializeField] private int initialDanmaku = 0;
        [SerializeField] private long initialTopicViews = 0;
        [SerializeField] private float initialDouyinHeat = 0f;
        [SerializeField] private float initialRating = 7.0f;
        [SerializeField] private int initialRevenue = 0;
        [SerializeField] private int initialActionPoints = 100;
        
        [Header("变化范围配置")]
        [SerializeField] private float heatChangeRange = 10f;
        [SerializeField] private float viewershipChangeRange = 0.1f;
        [SerializeField] private float douyinHeatChangeRange = 5f;
        [SerializeField] private float ratingChangeRange = 0.5f;
        
        [Header("衰减配置")]
        [SerializeField] private float heatDecayRate = 0.05f;
        [SerializeField] private float viewershipDecayRate = 0.02f;
        [SerializeField] private float douyinHeatDecayRate = 0.1f;
        
        [Header("事件影响配置")]
        [SerializeField] private float kissEventHeatBonus = 20f;
        [SerializeField] private float confessionEventHeatBonus = 30f;
        [SerializeField] private float conflictEventHeatBonus = 15f;
        [SerializeField] private float dateEventViewershipBonus = 0.5f;
        [SerializeField] private float dramaEventRatingBonus = 0.3f;
        
        [Header("行动点配置")]
        [SerializeField] private int maxActionPoints = 100;
        [SerializeField] private int actionPointRecoveryRate = 10;
        [SerializeField] private float actionPointConsumptionMultiplier = 1f;
        
        public float InitialHeat => initialHeat;
        public float InitialViewership => initialViewership;
        public int InitialDanmaku => initialDanmaku;
        public long InitialTopicViews => initialTopicViews;
        public float InitialDouyinHeat => initialDouyinHeat;
        public float InitialRating => initialRating;
        public int InitialRevenue => initialRevenue;
        public int InitialActionPoints => initialActionPoints;
        public float HeatChangeRange => heatChangeRange;
        public float ViewershipChangeRange => viewershipChangeRange;
        public float DouyinHeatChangeRange => douyinHeatChangeRange;
        public float RatingChangeRange => ratingChangeRange;
        public float HeatDecayRate => heatDecayRate;
        public float ViewershipDecayRate => viewershipDecayRate;
        public float DouyinHeatDecayRate => douyinHeatDecayRate;
        public float KissEventHeatBonus => kissEventHeatBonus;
        public float ConfessionEventHeatBonus => confessionEventHeatBonus;
        public float ConflictEventHeatBonus => conflictEventHeatBonus;
        public float DateEventViewershipBonus => dateEventViewershipBonus;
        public float DramaEventRatingBonus => dramaEventRatingBonus;
        public int MaxActionPoints => maxActionPoints;
        public int ActionPointRecoveryRate => actionPointRecoveryRate;
        public float ActionPointConsumptionMultiplier => actionPointConsumptionMultiplier;
        
        /// <summary>
        /// 根据配置创建导演统计数据
        /// </summary>
        public DirectorStatsData CreateStatsData()
        {
            DirectorStatsData data = new DirectorStatsData
            {
                realTimeHeat = initialHeat,
                viewership = initialViewership,
                danmakuCount = initialDanmaku,
                topicViews = initialTopicViews,
                douyinHeat = initialDouyinHeat,
                rating = initialRating,
                revenue = initialRevenue,
                actionPoints = initialActionPoints
            };
            
            return data;
        }
        
        /// <summary>
        /// 计算事件对热度的影响
        /// </summary>
        public float CalculateEventHeatImpact(DirectorEventType eventType)
        {
            switch (eventType)
            {
                case DirectorEventType.Kiss:
                    return kissEventHeatBonus;
                case DirectorEventType.Confession:
                    return confessionEventHeatBonus;
                case DirectorEventType.Conflict:
                    return conflictEventHeatBonus;
                case DirectorEventType.Date:
                    return dateEventViewershipBonus * 10f;
                case DirectorEventType.Drama:
                    return dramaEventRatingBonus * 10f;
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// 计算事件对视收率的影响
        /// </summary>
        public float CalculateEventViewershipImpact(DirectorEventType eventType)
        {
            switch (eventType)
            {
                case DirectorEventType.Date:
                    return dateEventViewershipBonus;
                case DirectorEventType.Drama:
                    return dramaEventRatingBonus;
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// 计算衰减后的值
        /// </summary>
        public float CalculateDecayedValue(float currentValue, DecayConfigType type, float timeDelta)
        {
            float decayRate = 0f;
            
            switch (type)
            {
                case DecayConfigType.Heat:
                    decayRate = heatDecayRate;
                    break;
                case DecayConfigType.Viewership:
                    decayRate = viewershipDecayRate;
                    break;
                case DecayConfigType.DouyinHeat:
                    decayRate = douyinHeatDecayRate;
                    break;
            }
            
            float decayAmount = decayRate * timeDelta;
            return Mathf.Max(0f, currentValue - decayAmount);
        }
        
        /// <summary>
        /// 计算行动点消耗
        /// </summary>
        public int CalculateActionPointConsumption(int baseCost)
        {
            return Mathf.CeilToInt(baseCost * actionPointConsumptionMultiplier);
        }
    }
    
    /// <summary>
    /// 导演事件类型
    /// </summary>
    public enum DirectorEventType
    {
        Kiss,
        Confession,
        Conflict,
        Date,
        Drama,
        Elimination,
        NewEntry
    }
    
    /// <summary>
    /// 衰减配置类型
    /// </summary>
    public enum DecayConfigType
    {
        Heat,
        Viewership,
        DouyinHeat
    }
    
    /// <summary>
    /// 导演统计数据配置数据类
    /// </summary>
    [Serializable]
    public class DirectorStatsConfigData
    {
        public float initialHeat;
        public float initialViewership;
        public float heatChangeRange;
        public float viewershipChangeRange;
        public float heatDecayRate;
        public float viewershipDecayRate;
        public float kissEventHeatBonus;
        public float confessionEventHeatBonus;
        public float conflictEventHeatBonus;
        public float dateEventViewershipBonus;
        public float dramaEventRatingBonus;
        public int maxActionPoints;
        public int actionPointRecoveryRate;
        
        public static DirectorStatsConfigData CreateDefault()
        {
            return new DirectorStatsConfigData
            {
                initialHeat = 50f,
                initialViewership = 0.5f,
                heatChangeRange = 10f,
                viewershipChangeRange = 0.1f,
                heatDecayRate = 0.05f,
                viewershipDecayRate = 0.02f,
                kissEventHeatBonus = 20f,
                confessionEventHeatBonus = 30f,
                conflictEventHeatBonus = 15f,
                dateEventViewershipBonus = 0.5f,
                dramaEventRatingBonus = 0.3f,
                maxActionPoints = 100,
                actionPointRecoveryRate = 10
            };
        }
    }
    
    /// <summary>
    /// 导演分析数据
    /// </summary>
    [Serializable]
    public class DirectorAnalyticsData
    {
        public float averageHeat;
        public float peakHeat;
        public float averageViewership;
        public float totalRevenue;
        public float averageRating;
        public List<EpisodeData> episodeDataList;
        public List<EventImpactData> eventImpacts;
        
        public DirectorAnalyticsData()
        {
            episodeDataList = new List<EpisodeData>();
            eventImpacts = new List<EventImpactData>();
        }
    }
    
    /// <summary>
    /// 节目数据
    /// </summary>
    [Serializable]
    public class EpisodeData
    {
        public int episodeNumber;
        public float heat;
        public float viewership;
        public int danmakuCount;
        public float rating;
        public int revenue;
    }
    
    /// <summary>
    /// 事件影响数据
    /// </summary>
    [Serializable]
    public class EventImpactData
    {
        public DirectorEventType eventType;
        public float heatImpact;
        public float viewershipImpact;
        public float ratingImpact;
        public DateTime timestamp;
    }
    
    /// <summary>
    /// 导演对比数据
    /// </summary>
    [Serializable]
    public class DirectorComparisonData
    {
        public string showName1;
        public string showName2;
        public float heatDifference;
        public float viewershipDifference;
        public float ratingDifference;
        public float revenueDifference;
    }
}
