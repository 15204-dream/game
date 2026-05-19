using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 嘉宾统计显示组件 - 用于在UI上显示嘉宾统计数据
    /// </summary>
    public class GuestStatsDisplay : MonoBehaviour
    {
        [Header("UI组件引用")]
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI dayProgressText;
        [SerializeField] private Image dayProgressBar;
        [SerializeField] private TextMeshProUGUI targetGuestNameText;
        [SerializeField] private TextMeshProUGUI affectionLevelText;
        [SerializeField] private Image affectionProgressBar;
        [SerializeField] private Transform guestListContainer;
        [SerializeField] private GameObject guestStatsItemPrefab;
        
        [Header("统计指标显示")]
        [SerializeField] private TextMeshProUGUI heartIndexText;
        [SerializeField] private TextMeshProUGUI focusRateText;
        [SerializeField] private TextMeshProUGUI socialActivityText;
        [SerializeField] private TextMeshProUGUI storyProgressText;
        [SerializeField] private TextMeshProUGUI endingRateText;
        [SerializeField] private TextMeshProUGUI conversationCountText;
        [SerializeField] private TextMeshProUGUI dateCountText;
        [SerializeField] private TextMeshProUGUI taskCountText;
        
        [Header("图表组件")]
        [SerializeField] private LineChart affectionTrendChart;
        [SerializeField] private BarChart guestRankingChart;
        
        private GuestStatistics guestStatistics;
        private Dictionary<string, GuestStatsItem> guestStatsItems;
        
        void Start()
        {
            guestStatsItems = new Dictionary<string, GuestStatsItem>();
            InitializeDisplay();
        }
        
        void Update()
        {
            if (guestStatistics != null)
            {
                UpdateDisplay();
            }
        }
        
        /// <summary>
        /// 初始化显示组件
        /// </summary>
        public void Initialize(GuestStatistics stats)
        {
            guestStatistics = stats;
            InitializeDisplay();
        }
        
        private void InitializeDisplay()
        {
            if (guestStatistics == null)
                return;
            
            UpdateBasicInfo();
            UpdateGuestList();
            UpdateStatisticsMetrics();
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        public void UpdateDisplay()
        {
            if (guestStatistics == null)
                return;
            
            UpdateBasicInfo();
            UpdateTargetGuestInfo();
            UpdateStatisticsMetrics();
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
            
            if (dayProgressText != null)
            {
                dayProgressText.text = $"{guestStatistics.DayProgress:F0}%";
            }
            
            if (dayProgressBar != null)
            {
                dayProgressBar.fillAmount = guestStatistics.DayProgress / 100f;
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
                    affectionLevelText.text = $"{targetStats.affectionLevel:F0}%";
                }
                
                if (affectionProgressBar != null)
                {
                    affectionProgressBar.fillAmount = targetStats.affectionLevel / 100f;
                }
            }
        }
        
        /// <summary>
        /// 更新嘉宾列表
        /// </summary>
        private void UpdateGuestList()
        {
            if (guestListContainer == null || guestStatsItemPrefab == null)
                return;
            
            ClearGuestList();
            
            var allGuestStats = guestStatistics.GetAllGuestStats();
            List<GuestRanking> rankings = guestStatistics.GetAffectionRanking();
            
            foreach (var ranking in rankings)
            {
                if (allGuestStats.ContainsKey(ranking.guestId))
                {
                    CreateGuestStatsItem(ranking, allGuestStats[ranking.guestId]);
                }
            }
        }
        
        /// <summary>
        /// 创建嘉宾统计项
        /// </summary>
        private void CreateGuestStatsItem(GuestRanking ranking, GuestStatsData stats)
        {
            GameObject itemObj = Instantiate(guestStatsItemPrefab, guestListContainer);
            GuestStatsItem item = itemObj.GetComponent<GuestStatsItem>();
            
            if (item != null)
            {
                item.Initialize(ranking, stats);
                guestStatsItems[stats.guestId] = item;
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
        /// 更新统计指标
        /// </summary>
        private void UpdateStatisticsMetrics()
        {
            GuestStatsData targetStats = guestStatistics.GetTargetGuestStats();
            
            if (targetStats != null)
            {
                if (heartIndexText != null)
                {
                    heartIndexText.text = $"{targetStats.heartIndex:F0}";
                }
                
                if (focusRateText != null)
                {
                    focusRateText.text = $"{targetStats.focusRate:F0}%";
                }
                
                if (socialActivityText != null)
                {
                    socialActivityText.text = $"{targetStats.socialActivity:F0}%";
                }
            }
            
            if (storyProgressText != null)
            {
                storyProgressText.text = $"{guestStatistics.EventProgress:F0}%";
            }
            
            if (endingRateText != null)
            {
                endingRateText.text = $"{guestStatistics.EndingRate:F0}%";
            }
            
            if (conversationCountText != null)
            {
                conversationCountText.text = guestStatistics.TotalConversations.ToString();
            }
            
            if (dateCountText != null)
            {
                dateCountText.text = guestStatistics.TotalDates.ToString();
            }
            
            if (taskCountText != null)
            {
                taskCountText.text = $"{guestStatistics.CompletedTasks}/{guestStatistics.TotalTasks}";
            }
        }
        
        /// <summary>
        /// 更新好感度趋势图
        /// </summary>
        public void UpdateAffectionTrendChart(string guestId)
        {
            if (affectionTrendChart == null || guestStatistics == null)
                return;
            
            GuestStatsData stats = guestStatistics.GetGuestStats(guestId);
            if (stats != null && stats.affectionHistory.Count > 0)
            {
                List<float> values = new List<float>();
                foreach (var record in stats.affectionHistory)
                {
                    values.Add(record.value);
                }
                affectionTrendChart.SetData(values);
            }
        }
        
        /// <summary>
        /// 更新嘉宾排名图
        /// </summary>
        public void UpdateGuestRankingChart()
        {
            if (guestRankingChart == null || guestStatistics == null)
                return;
            
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
        /// 刷新显示
        /// </summary>
        public void Refresh()
        {
            UpdateDisplay();
            UpdateGuestList();
        }
    }
    
    /// <summary>
    /// 嘉宾统计项组件
    /// </summary>
    public class GuestStatsItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI percentageText;
        [SerializeField] private TextMeshProUGUI trendText;
        [SerializeField] private Image highlightImage;
        
        private GuestRanking ranking;
        private GuestStatsData stats;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(GuestRanking ranking, GuestStatsData stats)
        {
            this.ranking = ranking;
            this.stats = stats;
            
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
                switch (ranking.rank)
                {
                    case 1: medal = "🥇"; break;
                    case 2: medal = "🥈"; break;
                    case 3: medal = "🥉"; break;
                    default: medal = $"{ranking.rank}."; break;
                }
                rankText.text = medal;
            }
            
            if (nameText != null)
            {
                nameText.text = stats.guestName;
            }
            
            if (progressBar != null)
            {
                progressBar.fillAmount = ranking.score / 100f;
            }
            
            if (percentageText != null)
            {
                percentageText.text = $"{ranking.score:F0}%";
            }
            
            if (trendText != null)
            {
                string trendSymbol = "";
                switch (ranking.trend)
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
