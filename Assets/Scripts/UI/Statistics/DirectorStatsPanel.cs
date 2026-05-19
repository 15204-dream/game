using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 导演模式统计面板 - 显示导演模式下的各种统计数据
    /// </summary>
    public class DirectorStatsPanel : StatisticsPanel
    {
        [Header("标题区域")]
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Header("核心数据显示")]
        [SerializeField] private TextMeshProUGUI realTimeHeatLabel;
        [SerializeField] private TextMeshProUGUI realTimeHeatValue;
        [SerializeField] private Image realTimeHeatBar;
        [SerializeField] private TextMeshProUGUI heatTrendText;
        
        [SerializeField] private TextMeshProUGUI viewershipLabel;
        [SerializeField] private TextMeshProUGUI viewershipValue;
        [SerializeField] private TextMeshProUGUI viewershipChangeValue;
        
        [SerializeField] private TextMeshProUGUI danmakuLabel;
        [SerializeField] private TextMeshProUGUI danmakuValue;
        [SerializeField] private TextMeshProUGUI danmakuChangeValue;
        
        [SerializeField] private TextMeshProUGUI topicViewsLabel;
        [SerializeField] private TextMeshProUGUI topicViewsValue;
        
        [SerializeField] private TextMeshProUGUI douyinHeatLabel;
        [SerializeField] private TextMeshProUGUI douyinHeatValue;
        
        [SerializeField] private TextMeshProUGUI ratingLabel;
        [SerializeField] private TextMeshProUGUI ratingValue;
        
        [SerializeField] private TextMeshProUGUI revenueLabel;
        [SerializeField] private TextMeshProUGUI revenueValue;
        
        [SerializeField] private TextMeshProUGUI actionPointsLabel;
        [SerializeField] private TextMeshProUGUI actionPointsValue;
        [SerializeField] private Image actionPointsBar;
        
        [Header("嘉宾热度榜")]
        [SerializeField] private TextMeshProUGUI popularityListTitle;
        [SerializeField] private Transform popularityListContainer;
        [SerializeField] private GameObject popularityItemPrefab;
        
        [Header("图表区域")]
        [SerializeField] private LineChart heatTrendChart;
        [SerializeField] private LineChart viewershipTrendChart;
        [SerializeField] private BarChart guestRankingChart;
        
        [Header("详细信息面板")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private TextMeshProUGUI detailTitleText;
        [SerializeField] private TextMeshProUGUI detailStatsText;
        
        private DirectorStatistics directorStatistics;
        private Dictionary<string, PopularityItem> popularityItems;
        private DirectorStatsDisplay directorStatsDisplay;
        
        protected override void Awake()
        {
            base.Awake();
            popularityItems = new Dictionary<string, PopularityItem>();
        }
        
        /// <summary>
        /// 初始化面板
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            if (titleText != null)
            {
                titleText.text = "📺 节目数据中心（导演模式）";
            }
        }
        
        /// <summary>
        /// 设置统计数据源
        /// </summary>
        public void SetStatistics(DirectorStatistics stats)
        {
            directorStatistics = stats;
            
            if (directorStatsDisplay != null)
            {
                directorStatsDisplay.Initialize(stats);
            }
            
            UpdatePanel();
        }
        
        /// <summary>
        /// 设置统计显示组件
        /// </summary>
        public void SetStatsDisplay(DirectorStatsDisplay display)
        {
            directorStatsDisplay = display;
            
            if (directorStatistics != null && display != null)
            {
                display.Initialize(directorStatistics);
            }
        }
        
        /// <summary>
        /// 更新面板内容
        /// </summary>
        protected override void UpdatePanel()
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
            UpdatePopularityList();
            UpdateCharts();
        }
        
        /// <summary>
        /// 更新实时热度
        /// </summary>
        private void UpdateRealTimeHeat()
        {
            if (realTimeHeatLabel != null)
            {
                realTimeHeatLabel.text = "🔥 实时热度：";
            }
            
            if (realTimeHeatValue != null)
            {
                realTimeHeatValue.text = $"{(int)directorStatistics.RealTimeHeat}/100";
            }
            
            if (realTimeHeatBar != null)
            {
                realTimeHeatBar.fillAmount = directorStatistics.RealTimeHeat / 100f;
            }
            
            if (heatTrendText != null)
            {
                string trendSymbol = directorStatistics.HeatTrend > 0 ? "📈" : 
                                     (directorStatistics.HeatTrend < 0 ? "📉" : "➖");
                heatTrendText.text = trendSymbol;
            }
        }
        
        /// <summary>
        /// 更新收视率
        /// </summary>
        private void UpdateViewership()
        {
            if (viewershipLabel != null)
            {
                viewershipLabel.text = "👁️ 收视率：";
            }
            
            if (viewershipValue != null)
            {
                viewershipValue.text = $"{directorStatistics.Viewership:F2}%";
            }
            
            if (viewershipChangeValue != null)
            {
                float change = directorStatistics.ViewershipChange;
                string changeText = change >= 0 ? $"+{change:F2}%" : $"{change:F2}%";
                string emoji = change > 0 ? "📈" : (change < 0 ? "📉" : "");
                viewershipChangeValue.text = $"{emoji} {changeText}";
            }
        }
        
        /// <summary>
        /// 更新弹幕数
        /// </summary>
        private void UpdateDanmaku()
        {
            if (danmakuLabel != null)
            {
                danmakuLabel.text = "💬 弹幕数：";
            }
            
            if (danmakuValue != null)
            {
                danmakuValue.text = StatisticsCalculator.FormatNumber(directorStatistics.DanmakuCount);
            }
            
            if (danmakuChangeValue != null)
            {
                float changeRate = directorStatistics.DanmakuChange;
                string changeText = changeRate >= 0 ? $"+{changeRate:F1}%" : $"{changeRate:F1}%";
                string emoji = changeRate > 0 ? "📈" : (changeRate < 0 ? "📉" : "");
                danmakuChangeValue.text = $"{emoji} {changeText}";
            }
        }
        
        /// <summary>
        /// 更新话题和抖音热度
        /// </summary>
        private void UpdateTopicAndDouyin()
        {
            if (topicViewsLabel != null)
            {
                topicViewsLabel.text = "❤️ 话题总量：";
            }
            
            if (topicViewsValue != null)
            {
                topicViewsValue.text = $"#糟糕是心动鸭# 阅读量{StatisticsCalculator.FormatNumber(directorStatistics.TopicViews)}";
            }
            
            if (douyinHeatLabel != null)
            {
                douyinHeatLabel.text = "🎵 抖音热度：";
            }
            
            if (douyinHeatValue != null)
            {
                douyinHeatValue.text = $"{directorStatistics.DouyinHeat:F0}";
            }
        }
        
        /// <summary>
        /// 更新口碑评分
        /// </summary>
        private void UpdateRating()
        {
            if (ratingLabel != null)
            {
                ratingLabel.text = "⭐ 口碑评分：";
            }
            
            if (ratingValue != null)
            {
                ratingValue.text = $"{directorStatistics.Rating:F1}";
            }
        }
        
        /// <summary>
        /// 更新收益
        /// </summary>
        private void UpdateRevenue()
        {
            if (revenueLabel != null)
            {
                revenueLabel.text = "💰 收益估算：";
            }
            
            if (revenueValue != null)
            {
                revenueValue.text = $"¥{StatisticsCalculator.FormatNumber(directorStatistics.Revenue)}";
            }
        }
        
        /// <summary>
        /// 更新行动点
        /// </summary>
        private void UpdateActionPoints()
        {
            if (actionPointsLabel != null)
            {
                actionPointsLabel.text = "⚡ 行动点：";
            }
            
            if (actionPointsValue != null)
            {
                actionPointsValue.text = $"{directorStatistics.ActionPoints}/{directorStatistics.MaxActionPoints}";
            }
            
            if (actionPointsBar != null)
            {
                actionPointsBar.fillAmount = directorStatistics.ActionPointConsumptionRate / 100f;
            }
        }
        
        /// <summary>
        /// 更新嘉宾热度列表
        /// </summary>
        private void UpdatePopularityList()
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
                
                Button button = itemObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnPopularityItemClicked(popularity.guestId));
                }
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
        /// 更新图表
        /// </summary>
        private void UpdateCharts()
        {
            if (heatTrendChart != null && directorStatistics != null)
            {
                List<DirectorStatsRecord> history = directorStatistics.GetHeatHistory(20);
                List<float> values = new List<float>();
                
                foreach (var record in history)
                {
                    values.Add(record.value);
                }
                
                heatTrendChart.SetData(values);
            }
            
            if (viewershipTrendChart != null && directorStatistics != null)
            {
                List<DirectorStatsRecord> history = directorStatistics.GetViewershipHistory(20);
                List<float> values = new List<float>();
                
                foreach (var record in history)
                {
                    values.Add(record.value);
                }
                
                viewershipTrendChart.SetData(values);
            }
            
            if (guestRankingChart != null && directorStatistics != null)
            {
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
        }
        
        /// <summary>
        /// 嘉宾热度项点击事件
        /// </summary>
        private void OnPopularityItemClicked(string guestId)
        {
            ShowGuestDetail(guestId);
            HighlightGuest(guestId);
        }
        
        /// <summary>
        /// 显示嘉宾详情
        /// </summary>
        public void ShowGuestDetail(string guestId)
        {
            if (detailPanel == null || directorStatistics == null)
                return;
            
            GuestPopularity popularity = directorStatistics.GetGuestPopularity(guestId);
            if (popularity == null)
                return;
            
            detailPanel.SetActive(true);
            
            if (detailTitleText != null)
            {
                detailTitleText.text = $"👥 {popularity.guestName} 热度详情";
            }
            
            if (detailStatsText != null)
            {
                string trendSymbol = "";
                switch (popularity.trend)
                {
                    case TrendDirection.Rising: trendSymbol = "📈 上升中"; break;
                    case TrendDirection.Falling: trendSymbol = "📉 下降中"; break;
                    case TrendDirection.Stable: trendSymbol = "➖ 稳定"; break;
                }
                
                detailStatsText.text = $"
🔥 当前热度: {(int)popularity.popularity}
📊 热度排名: 第{popularity.rank}名
{trendSymbol}
";
            }
        }
        
        /// <summary>
        /// 隐藏嘉宾详情
        /// </summary>
        public void HideGuestDetail()
        {
            if (detailPanel != null)
            {
                detailPanel.SetActive(false);
            }
            
            ClearHighlight();
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
        /// 清除高亮
        /// </summary>
        public void ClearHighlight()
        {
            foreach (var item in popularityItems.Values)
            {
                item.SetHighlight(false);
            }
        }
        
        /// <summary>
        /// 刷新面板
        /// </summary>
        public override void Refresh()
        {
            UpdatePanel();
        }
    }
}
