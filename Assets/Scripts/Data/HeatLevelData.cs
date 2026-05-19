using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 热度等级
    /// </summary>
    public enum HeatLevel
    {
        Cold,
        Lukewarm,
        Warm,
        Hot,
        VeryHot,
        OnFire,
        Viral
    }

    /// <summary>
    /// 平台类型
    /// </summary>
    public enum PlatformType
    {
        Weibo,
        Douyin,
        Xiaohongshu,
        Bilibili,
        WeChat,
        YouTube,
        TikTok
    }

    /// <summary>
    /// 内容类型
    /// </summary>
    public enum ContentType
    {
        Clip,
        Highlight,
        BehindTheScenes,
        Interview,
        LiveStream,
        Poll,
        Challenge
    }

    /// <summary>
    /// 热度数据
    /// </summary>
    [System.Serializable]
    public class HeatLevelData
    {
        [Header("总体热度")]
        public int totalHeat;
        public HeatLevel currentHeatLevel;
        public int heatRank;
        public int peakHeat;
        public int averageHeat;

        [Header("热度变化")]
        public int heatChange;
        public int dailyHeatGain;
        public int dailyHeatLoss;
        public List<HeatChangeRecord> heatHistory = new List<HeatChangeRecord>();

        [Header("平台热度")]
        public Dictionary<PlatformType, PlatformHeat> platformHeatData = new Dictionary<PlatformType, PlatformHeat>();

        [Header("热度峰值记录")]
        public List<HeatPeakRecord> heatPeaks = new List<HeatPeakRecord>();

        [Header("热门内容")]
        public List<TrendingContent> trendingContent = new List<TrendingContent>();
        public List<string> viralMoments = new List<string>();

        [Header("热搜")]
        public List<HotSearchEntry> hotSearchEntries = new List<HotSearchEntry>();
        public int totalHotSearchDays;

        [Header("观众反馈")]
        public AudienceFeedbackData audienceFeedback = new AudienceFeedbackData();

        [Header("热度事件")]
        public List<HeatEvent> heatEvents = new List<HeatEvent>();

        [Header("热度计算配置")]
        public HeatConfig config = new HeatConfig();
    }

    /// <summary>
    /// 平台热度数据
    /// </summary>
    [System.Serializable]
    public class PlatformHeat
    {
        public PlatformType platform;
        public string platformName;
        public int followers;
        public int totalViews;
        public int engagement;
        public int shares;
        public int comments;
        public int likes;
        public float engagementRate;
        public int dailyNewFollowers;
        public List<PlatformPost> recentPosts = new List<PlatformPost>();
    }

    /// <summary>
    /// 平台帖子数据
    /// </summary>
    [System.Serializable]
    public class PlatformPost
    {
        public string postId;
        public DateTime postTime;
        public ContentType contentType;
        public string content;
        public int views;
        public int likes;
        public int comments;
        public int shares;
        public int heatGenerated;
        public bool isPromoted;
        public int promotionCost;
    }

    /// <summary>
    /// 热度变化记录
    /// </summary>
    [System.Serializable]
    public class HeatChangeRecord
    {
        public string recordId;
        public DateTime timestamp;
        public int previousHeat;
        public int currentHeat;
        public int changeAmount;
        public HeatChangeReason reason;
        public string description;
        public string relatedEventId;
    }

    /// <summary>
    /// 热度变化原因
    /// </summary>
    public enum HeatChangeReason
    {
        EpisodeAir,
        ViralClip,
        GuestConflict,
        RomanceDevelopment,
        Elimination,
        Challenge,
        SpecialEvent,
        FanPromotion,
        NegativePR,
        PlatformAlgorithm,
        Collaboration,
        Advertisement
    }

    /// <summary>
    /// 热度峰值记录
    /// </summary>
    [System.Serializable]
    public class HeatPeakRecord
    {
        public string recordId;
        public DateTime peakTime;
        public int peakValue;
        public HeatLevel peakLevel;
        public string triggerEvent;
        public int durationMinutes;
    }

    /// <summary>
    /// 热门内容
    /// </summary>
    [System.Serializable]
    public class TrendingContent
    {
        public string contentId;
        public string title;
        public ContentType contentType;
        public PlatformType platform;
        public int currentRank;
        public int peakRank;
        public int viewCount;
        public int engagementCount;
        public DateTime trendingSince;
        public int trendingHours;
        public bool isStillTrending;
    }

    /// <summary>
    /// 热搜条目
    /// </summary>
    [System.Serializable]
    public class HotSearchEntry
    {
        public string entryId;
        public string keyword;
        public string description;
        public PlatformType platform;
        public int rank;
        public int searchVolume;
        public DateTime trendingSince;
        public int trendingHours;
        public bool isPromoted;
        public int promotionCost;
    }

    /// <summary>
    /// 观众反馈数据
    /// </summary>
    [System.Serializable]
    public class AudienceFeedbackData
    {
        public int totalRatings;
        public float averageRating;
        public int fiveStarRatings;
        public int fourStarRatings;
        public int threeStarRatings;
        public int twoStarRatings;
        public int oneStarRatings;

        [Header("反馈分类")]
        public int positiveFeedback;
        public int neutralFeedback;
        public int negativeFeedback;

        [Header("反馈主题")]
        public Dictionary<string, int> feedbackTopics = new Dictionary<string, int>();
        public List<string> mostLikedAspects = new List<string>();
        public List<string> mostDislikedAspects = new List<string>();

        [Header("实时反馈")]
        public int liveViewers;
        public int peakLiveViewers;
        public float liveEngagementRate;
        public List<LiveReaction> recentReactions = new List<LiveReaction>();
    }

    /// <summary>
    /// 实时反应
    /// </summary>
    [System.Serializable]
    public class LiveReaction
    {
        public string reactionId;
        public string emoji;
        public int count;
        public DateTime timestamp;
    }

    /// <summary>
    /// 热度事件
    /// </summary>
    [System.Serializable]
    public class HeatEvent
    {
        public string eventId;
        public string eventName;
        public HeatChangeReason reason;
        public DateTime eventTime;
        public int heatImpact;
        public int durationMinutes;
        public bool isPositive;
        public string description;
    }

    /// <summary>
    /// 热度配置
    /// </summary>
    [System.Serializable]
    public class HeatConfig
    {
        [Header("热度阈值")]
        public int coldThreshold = 0;
        public int lukewarmThreshold = 1000;
        public int warmThreshold = 5000;
        public int hotThreshold = 20000;
        public int veryHotThreshold = 50000;
        public int onFireThreshold = 100000;
        public int viralThreshold = 500000;

        [Header("每日衰减")]
        public int dailyDecayRate = 500;
        public int noUpdateDecayMultiplier = 2;

        [Header("互动加成")]
        public int viewHeatPer1000 = 1;
        public int likeHeatPer100 = 5;
        public int commentHeatPer50 = 10;
        public int shareHeatPer20 = 20;

        [Header("热搜配置")]
        public int hotSearchThreshold = 10000;
        public int promotedSearchCost = 1000;
        public int maxHotSearchRank = 50;
    }

    /// <summary>
    /// 热度工具类
    /// </summary>
    public static class HeatLevelHelper
    {
        public static HeatLevel GetHeatLevel(int heat, HeatConfig config)
        {
            if (heat >= config.viralThreshold) return HeatLevel.Viral;
            if (heat >= config.onFireThreshold) return HeatLevel.OnFire;
            if (heat >= config.veryHotThreshold) return HeatLevel.VeryHot;
            if (heat >= config.hotThreshold) return HeatLevel.Hot;
            if (heat >= config.warmThreshold) return HeatLevel.Warm;
            if (heat >= config.lukewarmThreshold) return HeatLevel.Lukewarm;
            return HeatLevel.Cold;
        }

        public static string GetHeatLevelName(HeatLevel level)
        {
            switch (level)
            {
                case HeatLevel.Cold: return "冷清";
                case HeatLevel.Lukewarm: return "温吞";
                case HeatLevel.Warm: return "升温中";
                case HeatLevel.Hot: return "热门";
                case HeatLevel.VeryHot: return "超热门";
                case HeatLevel.OnFire: return "爆火";
                case HeatLevel.Viral: return "全网刷屏";
                default: return "未知";
            }
        }

        public static float CalculateEngagementRate(int likes, int comments, int shares, int views)
        {
            if (views == 0) return 0f;
            return (float)(likes + comments * 2 + shares * 3) / views;
        }

        public static int CalculateTrendScore(TrendingContent content)
        {
            int rankScore = 100 - content.currentRank;
            int engagementScore = content.engagementCount / 100;
            int durationScore = content.trendingHours / 10;
            return rankScore + engagementScore + durationScore;
        }

        public static HeatChangeRecord CreateHeatChangeRecord(int previous, int current, HeatChangeReason reason, string description)
        {
            return new HeatChangeRecord
            {
                recordId = Guid.NewGuid().ToString(),
                timestamp = DateTime.Now,
                previousHeat = previous,
                currentHeat = current,
                changeAmount = current - previous,
                reason = reason,
                description = description
            };
        }
    }
}
