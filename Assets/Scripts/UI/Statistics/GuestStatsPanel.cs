using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 嘉宾模式统计面板 - 显示嘉宾模式下的各种统计数据
    /// </summary>
    public class GuestStatsPanel : StatisticsPanel
    {
        [Header("基本信息区域")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private Image dayProgressBar;
        [SerializeField] private TextMeshProUGUI dayProgressText;
        
        [Header("感情线区域")]
        [SerializeField] private TextMeshProUGUI targetGuestNameText;
        [SerializeField] private TextMeshProUGUI affectionLevelText;
        [SerializeField] private Image affectionProgressBar;
        [SerializeField] private TextMeshProUGUI heartIconText;
        
        [Header("好感度列表区域")]
        [SerializeField] private Transform guestListContainer;
        [SerializeField] private GameObject guestStatsItemPrefab;
        [SerializeField] private TextMeshProUGUI affectionListTitle;
        
        [Header("统计指标区域")]
        [SerializeField] private Transform metricsContainer;
        [SerializeField] private GameObject metricItemPrefab;
        
        [Header("图表区域")]
        [SerializeField] private LineChart affectionTrendChart;
        [SerializeField] private BarChart guestRankingChart;
        
        [Header("详细信息面板")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private TextMeshProUGUI detailTitleText;
        [SerializeField] private TextMeshProUGUI detailStatsText;
        
        private GuestStatistics guestStatistics;
        private Dictionary<string, GuestStatsItem> guestStatsItems;
        private Dictionary<string, MetricItem> metricItems;
        private GuestStatsDisplay guestStatsDisplay;
        
        protected override void Awake()
        {
            base.Awake();
            guestStatsItems = new Dictionary<string, GuestStatsItem>();
            metricItems = new Dictionary<string, MetricItem>();
        }
        
        protected override void Start()
        {
            base.Start();
            InitializeMetrics();
        }
        
        /// <summary>
        /// 初始化面板
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            if (titleText != null)
            {
                titleText.text = "📊 数据统计面板（嘉宾模式）";
            }
        }
        
        /// <summary>
        /// 设置统计数据源
        /// </summary>
        public void SetStatistics(GuestStatistics stats)
        {
            guestStatistics = stats;
            
            if (guestStatsDisplay != null)
            {
                guestStatsDisplay.Initialize(stats);
            }
            
            UpdatePanel();
        }
        
        /// <summary>
        /// 设置统计显示组件
        /// </summary>
        public void SetStatsDisplay(GuestStatsDisplay display)
        {
            guestStatsDisplay = display;
            
            if (guestStatistics != null && display != null)
            {
                display.Initialize(guestStatistics);
            }
        }
        
        /// <summary>
        /// 更新面板内容
        /// </summary>
        protected override void UpdatePanel()
        {
            if (guestStatistics == null)
                return;
            
            UpdateBasicInfo();
            UpdateTargetGuestInfo();
            UpdateGuestList();
            UpdateMetrics();
            UpdateCharts();
        }
        
        /// <summary>
        /// 更新基本信息
        /// </summary>
        private void UpdateBasicInfo()
        {
            if (dayText != null)
            {
                dayText.text = $"第 {guestStatistics.CurrentDay} 天 / {guestStatistics.TotalDays} 天";
            }
            
            if (dayProgressBar != null)
            {
                dayProgressBar.fillAmount = guestStatistics.DayProgress / 100f;
            }
            
            if (dayProgressText != null)
            {
                dayProgressText.text = $"{(int)guestStatistics.DayProgress}%";
            }
        }
        
        /// <summary>
        /// 更新目标嘉宾信息
        /// </summary>
        private void UpdateTargetGuestInfo()
        {
            GuestStatsData targetStats = guestStatistics.GetTargetGuestStats();
            
            if (targetStats != null)
            {
                if (targetGuestNameText != null)
                {
                    targetGuestNameText.text = $"正在攻略: {targetStats.guestName}";
                }
                
                if (affectionLevelText != null)
                {
                    affectionLevelText.text = $"{(int)targetStats.affectionLevel}%";
                }
                
                if (affectionProgressBar != null)
                {
                    affectionProgressBar.fillAmount = targetStats.affectionLevel / 100f;
                }
                
                if (heartIconText != null)
                {
                    heartIconText.text = GetHeartEmoji(targetStats.affectionLevel);
                }
            }
        }
        
        /// <summary>
        /// 获取心形emoji
        /// </summary>
        private string GetHeartEmoji(float affectionLevel)
        {
            if (affectionLevel >= 90)
                return "❤️‍🔥";
            else if (affectionLevel >= 70)
                return "❤️";
            else if (affectionLevel >= 50)
                return "💗";
            else if (affectionLevel >= 30)
                return "💕";
            else
                return "🤍";
        }
        
        /// <summary>
        /// 更新嘉宾列表
        /// </summary>
        private void UpdateGuestList()
        {
            if (guestListContainer == null || guestStatsItemPrefab == null || guestStatistics == null)
                return;
            
            ClearGuestList();
            
            List<GuestRanking> rankings = guestStatistics.GetAffectionRanking();
            
            foreach (var ranking in rankings)
            {
                CreateGuestStatsItem(ranking);
            }
        }
        
        /// <summary>
        /// 创建嘉宾统计项
        /// </summary>
        private void CreateGuestStatsItem(GuestRanking ranking)
        {
            GameObject itemObj = Instantiate(guestStatsItemPrefab, guestListContainer);
            GuestStatsItem item = itemObj.GetComponent<GuestStatsItem>();
            
            if (item != null)
            {
                GuestStatsData stats = guestStatistics.GetGuestStats(ranking.guestId);
                if (stats != null)
                {
                    item.Initialize(ranking, stats);
                    guestStatsItems[ranking.guestId] = item;
                    
                    Button button = itemObj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnGuestItemClicked(ranking.guestId));
                    }
                }
            }
        }
        
        /// <summary>
        /// 清除嘉宾列表
        /// </summary>
        private void ClearGuestList()
        {
            foreach (var item in guestStatsItems.Values)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }
            guestStatsItems.Clear();
        }
        
        /// <summary>
        /// 初始化统计指标
        /// </summary>
        private void InitializeMetrics()
        {
            if (metricsContainer == null || metricItemPrefab == null)
                return;
            
            CreateMetricItem("heartIndex", "💗 心动指数", "0");
            CreateMetricItem("focusRate", "🎯 专注度", "0%");
            CreateMetricItem("socialActivity", "🗣️ 社交活跃度", "0%");
            CreateMetricItem("storyProgress", "📖 剧情完成度", "0%");
            CreateMetricItem("endingRate", "🎬 结局达成率", "0%");
            CreateMetricItem("conversationCount", "💬 对话次数", "0");
            CreateMetricItem("dateCount", "🌹 约会次数", "0");
            CreateMetricItem("taskCount", "✅ 任务完成数", "0/0");
        }
        
        /// <summary>
        /// 创建统计指标项
        /// </summary>
        private void CreateMetricItem(string id, string label, string value)
        {
            if (metricsContainer == null || metricItemPrefab == null)
                return;
            
            GameObject itemObj = Instantiate(metricItemPrefab, metricsContainer);
            MetricItem item = itemObj.GetComponent<MetricItem>();
            
            if (item != null)
            {
                item.Initialize(label, value);
                metricItems[id] = item;
            }
        }
        
        /// <summary>
        /// 更新统计指标
        /// </summary>
        private void UpdateMetrics()
        {
            if (guestStatistics == null)
                return;
            
            GuestStatsData targetStats = guestStatistics.GetTargetGuestStats();
            
            if (targetStats != null)
            {
                UpdateMetric("heartIndex", $"{targetStats.heartIndex:F0}");
                UpdateMetric("focusRate", $"{targetStats.focusRate:F0}%");
                UpdateMetric("socialActivity", $"{targetStats.socialActivity:F0}%");
            }
            
            UpdateMetric("storyProgress", $"{guestStatistics.EventProgress:F0}%");
            UpdateMetric("endingRate", $"{guestStatistics.EndingRate:F0}%");
            UpdateMetric("conversationCount", guestStatistics.TotalConversations.ToString());
            UpdateMetric("dateCount", guestStatistics.TotalDates.ToString());
            UpdateMetric("taskCount", $"{guestStatistics.CompletedTasks}/{guestStatistics.TotalTasks}");
        }
        
        /// <summary>
        /// 更新统计指标值
        /// </summary>
        private void UpdateMetric(string id, string value)
        {
            if (metricItems.ContainsKey(id))
            {
                metricItems[id].SetValue(value);
            }
        }
        
        /// <summary>
        /// 更新图表
        /// </summary>
        private void UpdateCharts()
        {
            if (affectionTrendChart != null && guestStatistics != null)
            {
                GuestStatsData targetStats = guestStatistics.GetTargetGuestStats();
                if (targetStats != null && targetStats.affectionHistory.Count > 0)
                {
                    List<float> values = new List<float>();
                    foreach (var record in targetStats.affectionHistory)
                    {
                        values.Add(record.value);
                    }
                    affectionTrendChart.SetData(values);
                }
            }
            
            if (guestRankingChart != null && guestStatistics != null)
            {
                List<GuestRanking> rankings = guestStatistics.GetAffectionRanking();
                List<string> labels = new List<string>();
                List<float> values = new List<float>();
                
                foreach (var ranking in rankings)
                {
                    labels.Add(ranking.guestName);
                    values.Add(ranking.score);
                }
                
                guestRankingChart.SetData(labels, values);
            }
        }
        
        /// <summary>
        /// 嘉宾项点击事件
        /// </summary>
        private void OnGuestItemClicked(string guestId)
        {
            ShowGuestDetail(guestId);
            HighlightGuest(guestId);
        }
        
        /// <summary>
        /// 显示嘉宾详情
        /// </summary>
        public void ShowGuestDetail(string guestId)
        {
            if (detailPanel == null || guestStatistics == null)
                return;
            
            GuestStatsData stats = guestStatistics.GetGuestStats(guestId);
            if (stats == null)
                return;
            
            detailPanel.SetActive(true);
            
            if (detailTitleText != null)
            {
                detailTitleText.text = $"💕 {stats.guestName} 详细信息";
            }
            
            if (detailStatsText != null)
            {
                detailStatsText.text = $"
💗 好感度: {(int)stats.affectionLevel}%
💗 心动指数: {(int)stats.heartIndex}
🎯 专注度: {stats.focusRate:F0}%
🗣️ 社交活跃度: {stats.socialActivity:F0}%
💬 对话次数: {stats.conversationCount}
🌹 约会次数: {stats.dateCount}
❤️ 心动次数: {stats.heartCount}
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
            foreach (var kvp in guestStatsItems)
            {
                kvp.Value.SetHighlight(kvp.Key == guestId);
            }
        }
        
        /// <summary>
        /// 清除高亮
        /// </summary>
        public void ClearHighlight()
        {
            foreach (var item in guestStatsItems.Values)
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
    
    /// <summary>
    /// 统计指标项组件
    /// </summary>
    public class MetricItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(string label, string value)
        {
            if (labelText != null)
            {
                labelText.text = label;
            }
            
            if (valueText != null)
            {
                valueText.text = value;
            }
        }
        
        /// <summary>
        /// 设置值
        /// </summary>
        public void SetValue(string value)
        {
            if (valueText != null)
            {
                valueText.text = value;
            }
        }
        
        /// <summary>
        /// 设置值（带颜色）
        /// </summary>
        public void SetValue(string value, Color color)
        {
            if (valueText != null)
            {
                valueText.text = value;
                valueText.color = color;
            }
        }
    }
}
