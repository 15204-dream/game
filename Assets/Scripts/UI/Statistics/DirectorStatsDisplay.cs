using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 导演统计显示组件 - 用于在UI上显示导演统计数据
    /// </summary>
    public class DirectorStatsDisplay : MonoBehaviour
    {
        [Header("核心数据显示")]
        [SerializeField] private TextMeshProUGUI realTimeHeatText;
        [SerializeField] private Image realTimeHeatBar;
        [SerializeField] private TextMeshProUGUI heatTrendText;
        
        [SerializeField] private TextMeshProUGUI viewershipText;
        [SerializeField] private TextMeshProUGUI viewershipChangeText;
        
        [SerializeField] private TextMeshProUGUI danmakuCountText;
        [SerializeField] private TextMeshProUGUI danmakuChangeText;
        
        [SerializeField] private TextMeshProUGUI topicViewsText;
        [SerializeField] private TextMeshProUGUI douyinHeatText;
        [SerializeField] private TextMeshProUGUI ratingText;
        [SerializeField] private TextMeshProUGUI revenueText;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Image actionPointsBar;
        
        [Header("嘉宾热度榜")]
        [SerializeField] private Transform popularityListContainer;
        [SerializeField] private GameObject popularityItemPrefab;
        
        [Header("图表组件")]
        [SerializeField] private LineChart heatTrendChart;
        [SerializeField] private LineChart viewershipTrendChart;
        [SerializeField] private BarChart guestRankingChart;
        
        [Header("实时数据更新")]
        [SerializeField] private float updateInterval = 0.5f;
        
        private DirectorStatistics directorStatistics;
        private Dictionary<string, PopularityItem> popularityItems;
        private float lastUpdateTime;
        
        void Start()
        {
            popularityItems = new Dictionary<string, PopularityItem>();
        }
        
        void Update()
        {
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateDisplay();
                lastUpdateTime = Time.time;
            }
        }
        
        /// <summary>
        /// 初始化显示组件
        /// </summary>
        public void Initialize(DirectorStatistics stats)
        {
            directorStatistics = stats;
            UpdateDisplay();
            UpdatePopularityList();
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        public void UpdateDisplay()
        {
            if (directorStatistics == null)
                return;
            
            UpdateRealTimeHeat();
            UpdateViewership();
            UpdateDanmaku();
            UpdateTopicAndDouyin();
            UpdateRating();
            UpdateRevenue();
            UpdateActionPoints();
        }
        
        /// <summary>
        /// 更新实时热度显示
        /// </summary>
        private void UpdateRealTimeHeat()
        {
            if (realTimeHeatText != null)
            {
                realTimeHeatText.text = $"{directorStatistics.RealTimeHeat:F0}/100";
            }
            
            if (realTimeHeatBar != null)
            {
                realTimeHeatBar.fillAmount = directorStatistics.RealTimeHeat / 100f;
            }
            
            if (heatTrendText != null)
            {
                string trendSymbol = directorStatistics.HeatTrend > 0 ? "📈" : (directorStatistics.HeatTrend < 0 ? "📉" : "➖");
                heatTrendText.text = trendSymbol;
            }
        }
        
        /// <summary>
        /// 更新收视率显示
        /// </summary>
        private void UpdateViewership()
        {
            if (viewershipText != null)
            {
                viewershipText.text = $"{directorStatistics.Viewership:F2}%";
            }
            
            if (viewershipChangeText != null)
            {
                float change = directorStatistics.ViewershipChange;
                string changeText = change >= 0 ? $"+{change:F2}%" : $"{change:F2}%";
                string emoji = change > 0 ? "📈" : (change < 0 ? "📉" : "");
                viewershipChangeText.text = $"{emoji} {changeText}";
            }
        }
        
        /// <summary>
        /// 更新弹幕数显示
        /// </summary>
        private void UpdateDanmaku()
        {
            if (danmakuCountText != null)
            {
                danmakuCountText.text = StatisticsCalculator.FormatNumber(directorStatistics.DanmakuCount);
            }
            
            if (danmakuChangeText != null)
            {
                float changeRate = directorStatistics.DanmakuChange;
                string changeText = changeRate >= 0 ? $"+{changeRate:F1}%" : $"{changeRate:F1}%";
                string emoji = changeRate > 0 ? "📈" : (changeRate < 0 ? "📉" : "");
                danmakuChangeText.text = $"{emoji} {changeText}";
            }
        }
        
        /// <summary>
        /// 更新话题和抖音热度显示
        /// </summary>
        private void UpdateTopicAndDouyin()
        {
            if (topicViewsText != null)
            {
                topicViewsText.text = $"#糟糕是心动鸭# 阅读量{StatisticsCalculator.FormatNumber(directorStatistics.TopicViews)}";
            }
            
            if (douyinHeatText != null)
            {
                douyinHeatText.text = $"抖音热度: {directorStatistics.DouyinHeat:F0}";
            }
        }
        
        /// <summary>
        /// 更新口碑评分显示
        /// </summary>
        private void UpdateRating()
        {
            if (ratingText != null)
            {
                ratingText.text = $"口碑评分: {directorStatistics.Rating:F1}";
            }
        }
        
        /// <summary>
        /// 更新收益显示
        /// </summary>
        private void UpdateRevenue()
        {
            if (revenueText != null)
            {
                revenueText.text = $"收益估算: ¥{StatisticsCalculator.FormatNumber(directorStatistics.Revenue)}";
            }
        }
        
        /// <summary>
        /// 更新行动点显示
        /// </summary>
        private void UpdateActionPoints()
        {
            if (actionPointsText != null)
            {
                actionPointsText.text = $"{directorStatistics.ActionPoints}/{directorStatistics.MaxActionPoints}";
            }
            
            if (actionPointsBar != null)
            {
                actionPointsBar.fillAmount = directorStatistics.ActionPointConsumptionRate / 100f;
            }
        }
        
        /// <summary>
        /// 更新嘉宾热度列表
        /// </summary>
        public void UpdatePopularityList()
        {
            if (popularityListContainer == null || popularityItemPrefab == null || directorStatistics == null)
                return;
            
            ClearPopularityList();
            
            List<GuestPopularity> rankings = directorStatistics.GetGuestPopularityRanking();
            
            foreach (var popularity in rankings)
            {
                CreatePopularityItem(popularity);
            }
        }
        
        /// <summary>
        /// 创建嘉宾热度项
        /// </summary>
        private void CreatePopularityItem(GuestPopularity popularity)
        {
            GameObject itemObj = Instantiate(popularityItemPrefab, popularityListContainer);
            PopularityItem item = itemObj.GetComponent<PopularityItem>();
            
            if (item != null)
            {
                item.Initialize(popularity);
                popularityItems[popularity.guestId] = item;
            }
        }
        
        /// <summary>
        /// 清除嘉宾热度列表
        /// </summary>
        private void ClearPopularityList()
        {
            foreach (var item in popularityItems.Values)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }
            popularityItems.Clear();
        }
        
        /// <summary>
        /// 更新热度趋势图
        /// </summary>
        public void UpdateHeatTrendChart()
        {
            if (heatTrendChart == null || directorStatistics == null)
                return;
            
            List<DirectorStatsRecord> history = directorStatistics.GetHeatHistory(20);
            List<float> values = new List<float>();
            
            foreach (var record in history)
            {
                values.Add(record.value);
            }
            
            heatTrendChart.SetData(values);
        }
        
        /// <summary>
        /// 更新收视率趋势图
        /// </summary>
        public void UpdateViewershipTrendChart()
        {
            if (viewershipTrendChart == null || directorStatistics == null)
                return;
            
            List<DirectorStatsRecord> history = directorStatistics.GetViewershipHistory(20);
            List<float> values = new List<float>();
            
            foreach (var record in history)
            {
                values.Add(record.value);
            }
            
            viewershipTrendChart.SetData(values);
        }
        
        /// <summary>
        /// 更新嘉宾排名图
        /// </summary>
        public void UpdateGuestRankingChart()
        {
            if (guestRankingChart == null || directorStatistics == null)
                return;
            
            List<GuestPopularity> rankings = directorStatistics.GetGuestPopularityRanking();
            List<string> labels = new List<string>();
            List<float> values = new List<float>();
            
            foreach (var popularity in rankings)
            {
                labels.Add(popularity.guestName);
                values.Add(popularity.popularity);
            }
            
            guestRankingChart.SetData(labels, values);
        }
        
        /// <summary>
        /// 高亮指定嘉宾
        /// </summary>
        public void HighlightGuest(string guestId)
        {
            foreach (var kvp in popularityItems)
            {
                kvp.Value.SetHighlight(kvp.Key == guestId);
            }
        }
        
        /// <summary>
        /// 刷新显示
        /// </summary>
        public void Refresh()
        {
            UpdateDisplay();
            UpdatePopularityList();
        }
    }
    
    /// <summary>
    /// 热度项组件
    /// </summary>
    public class PopularityItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI trendText;
        [SerializeField] private Image highlightImage;
        
        private GuestPopularity popularity;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(GuestPopularity popularity)
        {
            this.popularity = popularity;
            UpdateDisplay();
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        private void UpdateDisplay()
        {
            if (rankText != null)
            {
                string medal = "";
                switch (popularity.rank)
                {
                    case 1: medal = "🥇"; break;
                    case 2: medal = "🥈"; break;
                    case 3: medal = "🥉"; break;
                    default: medal = $"{popularity.rank}."; break;
                }
                rankText.text = medal;
            }
            
            if (nameText != null)
            {
                nameText.text = popularity.guestName;
            }
            
            if (progressBar != null)
            {
                progressBar.fillAmount = popularity.popularity / 100f;
            }
            
            if (scoreText != null)
            {
                scoreText.text = $"{popularity.popularity:F0}";
            }
            
            if (trendText != null)
            {
                string trendSymbol = "";
                switch (popularity.trend)
                {
                    case TrendDirection.Rising: trendSymbol = "↑"; break;
                    case TrendDirection.Falling: trendSymbol = "↓"; break;
                    case TrendDirection.Stable: trendSymbol = "→"; break;
                }
                trendText.text = trendSymbol;
            }
        }
        
        /// <summary>
        /// 设置高亮
        /// </summary>
        public void SetHighlight(bool highlight)
        {
            if (highlightImage != null)
            {
                highlightImage.gameObject.SetActive(highlight);
            }
        }
    }
}
