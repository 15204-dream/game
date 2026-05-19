using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 统计计算器 - 负责各种统计数据的计算
    /// </summary>
    public static class StatisticsCalculator
    {
        /// <summary>
        /// 计算心动指数
        /// </summary>
        public static float CalculateHeartIndex(GuestStatisticsData guestStats)
        {
            if (guestStats == null)
                return 0f;
            
            float affectionWeight = 0.4f;
            float conversationWeight = 0.2f;
            float dateWeight = 0.2f;
            float heartWeight = 0.2f;
            
            float affectionScore = guestStats.affectionLevel;
            float conversationScore = Mathf.Min(guestStats.conversationCount / 20f, 1f) * 100f;
            float dateScore = Mathf.Min(guestStats.dateCount / 5f, 1f) * 100f;
            float heartScore = Mathf.Min(guestStats.heartCount / 10f, 1f) * 100f;
            
            float heartIndex = affectionScore * affectionWeight +
                               conversationScore * conversationWeight +
                               dateScore * dateWeight +
                               heartScore * heartWeight;
            
            return Mathf.Clamp(heartIndex, 0f, 100f);
        }
        
        /// <summary>
        /// 计算专注度
        /// </summary>
        public static float CalculateFocusRate(int totalInteractions, int focusInteractions)
        {
            if (totalInteractions == 0)
                return 0f;
            return Mathf.Clamp((float)focusInteractions / totalInteractions * 100f, 0f, 100f);
        }
        
        /// <summary>
        /// 计算社交活跃度
        /// </summary>
        public static float CalculateSocialActivity(List<GuestStatisticsData> allGuests, string currentGuestId)
        {
            if (allGuests == null || allGuests.Count == 0)
                return 0f;
            
            float totalActivity = 0f;
            int count = 0;
            
            foreach (var guest in allGuests)
            {
                if (guest.guestId != currentGuestId)
                {
                    totalActivity += guest.conversationCount + guest.dateCount * 2;
                    count++;
                }
            }
            
            if (count == 0)
                return 0f;
            
            return Mathf.Clamp(totalActivity / count, 0f, 100f);
        }
        
        /// <summary>
        /// 计算剧情完成度
        /// </summary>
        public static float CalculateStoryProgress(int completedEvents, int totalEvents)
        {
            if (totalEvents == 0)
                return 0f;
            return Mathf.Clamp((float)completedEvents / totalEvents * 100f, 0f, 100f);
        }
        
        /// <summary>
        /// 计算结局达成率
        /// </summary>
        public static float CalculateEndingRate(int achievedEndings, int totalEndings)
        {
            if (totalEndings == 0)
                return 0f;
            return Mathf.Clamp((float)achievedEndings / totalEndings * 100f, 0f, 100f);
        }
        
        /// <summary>
        /// 计算实时热度
        /// </summary>
        public static float CalculateRealTimeHeat(DirectorStatisticsData directorStats)
        {
            if (directorStats == null)
                return 50f;
            
            float danmakuWeight = 0.3f;
            float viewershipWeight = 0.4f;
            float topicWeight = 0.3f;
            
            float danmakuScore = Mathf.Min(directorStats.danmakuCount / 10000f, 1f) * 100f;
            float viewershipScore = directorStats.viewership * 100f;
            float topicScore = Mathf.Min(directorStats.topicViews / 100000000f, 1f) * 100f;
            
            float heat = danmakuScore * danmakuWeight +
                         viewershipScore * viewershipWeight +
                         topicScore * topicWeight;
            
            return Mathf.Clamp(heat, 0f, 100f);
        }
        
        /// <summary>
        /// 计算综合评分
        /// </summary>
        public static float CalculateOverallRating(DirectorStatisticsData directorStats)
        {
            if (directorStats == null)
                return 0f;
            
            float heatScore = directorStats.realTimeHeat * 0.3f;
            float viewershipScore = directorStats.viewership * 20f;
            float ratingScore = directorStats.rating * 10f;
            float douyinScore = directorStats.douyinHeat * 0.1f;
            
            float overall = heatScore + viewershipScore + ratingScore + douyinScore;
            return Mathf.Clamp(overall, 0f, 100f);
        }
        
        /// <summary>
        /// 计算收益估算
        /// </summary>
        public static int EstimateRevenue(DirectorStatisticsData directorStats)
        {
            if (directorStats == null)
                return 0;
            
            int baseRevenue = 10000;
            int heatBonus = (int)(directorStats.realTimeHeat * 100);
            int viewershipBonus = (int)(directorStats.viewership * 50000);
            int ratingBonus = (int)(directorStats.rating * 1000);
            
            return baseRevenue + heatBonus + viewershipBonus + ratingBonus;
        }
        
        /// <summary>
        /// 计算行动点消耗率
        /// </summary>
        public static float CalculateActionPointConsumption(int usedPoints, int totalPoints)
        {
            if (totalPoints == 0)
                return 0f;
            return Mathf.Clamp((float)usedPoints / totalPoints * 100f, 0f, 100f);
        }
        
        /// <summary>
        /// 计算平均值
        /// </summary>
        public static float CalculateAverage(List<float> values)
        {
            if (values == null || values.Count == 0)
                return 0f;
            
            float sum = 0f;
            foreach (var value in values)
            {
                sum += value;
            }
            return sum / values.Count;
        }
        
        /// <summary>
        /// 计算标准差
        /// </summary>
        public static float CalculateStandardDeviation(List<float> values)
        {
            if (values == null || values.Count == 0)
                return 0f;
            
            float average = CalculateAverage(values);
            float sumSquaredDiff = 0f;
            
            foreach (var value in values)
            {
                float diff = value - average;
                sumSquaredDiff += diff * diff;
            }
            
            return Mathf.Sqrt(sumSquaredDiff / values.Count);
        }
        
        /// <summary>
        /// 计算趋势
        /// </summary>
        public static TrendDirection CalculateTrend(List<StatisticsRecord> history)
        {
            if (history == null || history.Count < 2)
                return TrendDirection.Stable;
            
            int recentCount = Mathf.Min(5, history.Count);
            float recentSum = 0f;
            float olderSum = 0f;
            
            for (int i = 0; i < recentCount; i++)
            {
                recentSum += history[history.Count - 1 - i].value;
            }
            
            int olderCount = Mathf.Min(5, history.Count - recentCount);
            for (int i = 0; i < olderCount; i++)
            {
                olderSum += history[history.Count - 1 - recentCount - i].value;
            }
            
            float recentAvg = recentSum / recentCount;
            float olderAvg = olderCount > 0 ? olderSum / olderCount : recentAvg;
            
            float threshold = 5f;
            
            if (recentAvg - olderAvg > threshold)
                return TrendDirection.Rising;
            else if (olderAvg - recentAvg > threshold)
                return TrendDirection.Falling;
            else
                return TrendDirection.Stable;
        }
        
        /// <summary>
        /// 计算环比变化
        /// </summary>
        public static float CalculateChangeRate(float current, float previous)
        {
            if (Mathf.Abs(previous) < 0.001f)
                return 0f;
            
            return ((current - previous) / previous) * 100f;
        }
        
        /// <summary>
        /// 格式化数字
        /// </summary>
        public static string FormatNumber(float number)
        {
            if (Mathf.Abs(number) >= 100000000)
                return (number / 100000000).ToString("F1") + "亿";
            else if (Mathf.Abs(number) >= 10000)
                return (number / 10000).ToString("F1") + "万";
            else if (Mathf.Abs(number) >= 1000)
                return number.ToString("N0");
            else
                return number.ToString("F1");
        }
        
        /// <summary>
        /// 格式化百分比
        /// </summary>
        public static string FormatPercentage(float percentage)
        {
            return percentage.ToString("F1") + "%";
        }
        
        /// <summary>
        /// 计算排名
        /// </summary>
        public static List<GuestRanking> CalculateRanking(List<GuestStatisticsData> guests)
        {
            List<GuestRanking> rankings = new List<GuestRanking>();
            
            foreach (var guest in guests)
            {
                GuestRanking ranking = new GuestRanking
                {
                    guestId = guest.guestId,
                    guestName = guest.guestName,
                    score = CalculateHeartIndex(guest),
                    trend = CalculateTrend(guest.history)
                };
                rankings.Add(ranking);
            }
            
            rankings.Sort((a, b) => b.score.CompareTo(a.score));
            
            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].rank = i + 1;
            }
            
            return rankings;
        }
    }
    
    /// <summary>
    /// 趋势方向
    /// </summary>
    public enum TrendDirection
    {
        Rising,
        Falling,
        Stable
    }
    
    /// <summary>
    /// 嘉宾排名
    /// </summary>
    [Serializable]
    public class GuestRanking
    {
        public string guestId;
        public string guestName;
        public float score;
        public int rank;
        public TrendDirection trend;
    }
}
