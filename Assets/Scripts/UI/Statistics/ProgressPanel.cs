using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 进度面板 - 显示游戏进度和各种任务进度
    /// </summary>
    public class ProgressPanel : StatisticsPanel
    {
        [Header("标题区域")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI currentDayText;
        [SerializeField] private Image dayProgressBar;
        [SerializeField] private TextMeshProUGUI dayProgressText;
        
        [Header("剧情进度区域")]
        [SerializeField] private TextMeshProUGUI storyTitleText;
        [SerializeField] private TextMeshProUGUI storyProgressText;
        [SerializeField] private Image storyProgressBar;
        [SerializeField] private Transform storyTaskContainer;
        [SerializeField] private GameObject storyTaskItemPrefab;
        
        [Header("支线任务区域")]
        [SerializeField] private TextMeshProUGUI sideQuestTitleText;
        [SerializeField] private TextMeshProUGUI sideQuestProgressText;
        [SerializeField] private Transform sideQuestContainer;
        [SerializeField] private GameObject sideQuestItemPrefab;
        
        [Header("每日任务区域")]
        [SerializeField] private TextMeshProUGUI dailyQuestTitleText;
        [SerializeField] private TextMeshProUGUI dailyQuestProgressText;
        [SerializeField] private Transform dailyQuestContainer;
        [SerializeField] private GameObject dailyQuestItemPrefab;
        
        [Header("成就进度区域")]
        [SerializeField] private TextMeshProUGUI achievementTitleText;
        [SerializeField] private TextMeshProUGUI achievementProgressText;
        [SerializeField] private Image achievementProgressBar;
        
        [Header("章节进度")]
        [SerializeField] private Transform chapterContainer;
        [SerializeField] private GameObject chapterItemPrefab;
        
        private GuestStatistics guestStatistics;
        private AchievementManager achievementManager;
        private Dictionary<string, ProgressItem> progressItems;
        
        protected override void Awake()
        {
            base.Awake();
            progressItems = new Dictionary<string, ProgressItem>();
        }
        
        /// <summary>
        /// 初始化面板
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            if (titleText != null)
            {
                titleText.text = "📋 进度面板";
            }
            
            if (storyTitleText != null)
            {
                storyTitleText.text = "📖 剧情进度";
            }
            
            if (sideQuestTitleText != null)
            {
                sideQuestTitleText.text = "🌟 支线任务";
            }
            
            if (dailyQuestTitleText != null)
            {
                dailyQuestTitleText.text = "⭐ 每日任务";
            }
            
            if (achievementTitleText != null)
            {
                achievementTitleText.text = "🏆 成就进度";
            }
        }
        
        /// <summary>
        /// 设置嘉宾统计数据
        /// </summary>
        public void SetGuestStatistics(GuestStatistics stats)
        {
            guestStatistics = stats;
            UpdatePanel();
        }
        
        /// <summary>
        /// 设置成就管理器
        /// </summary>
        public void SetAchievementManager(AchievementManager manager)
        {
            achievementManager = manager;
            UpdatePanel();
        }
        
        /// <summary>
        /// 更新面板内容
        /// </summary>
        protected override void UpdatePanel()
        {
            UpdateDayProgress();
            UpdateStoryProgress();
            UpdateQuestProgress();
            UpdateAchievementProgress();
            UpdateChapterProgress();
        }
        
        /// <summary>
        /// 更新天数进度
        /// </summary>
        private void UpdateDayProgress()
        {
            if (guestStatistics == null)
                return;
            
            if (currentDayText != null)
            {
                currentDayText.text = $"第 {guestStatistics.CurrentDay} 天 / {guestStatistics.TotalDays} 天";
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
        /// 更新剧情进度
        /// </summary>
        private void UpdateStoryProgress()
        {
            if (guestStatistics == null)
                return;
            
            float progress = guestStatistics.EventProgress;
            
            if (storyProgressText != null)
            {
                storyProgressText.text = $"{guestStatistics.CompletedEvents}/{guestStatistics.TotalEvents} 事件";
            }
            
            if (storyProgressBar != null)
            {
                storyProgressBar.fillAmount = progress / 100f;
            }
        }
        
        /// <summary>
        /// 更新任务进度
        /// </summary>
        private void UpdateQuestProgress()
        {
            if (guestStatistics == null)
                return;
            
            if (sideQuestProgressText != null)
            {
                sideQuestProgressText.text = "0/10 已完成";
            }
            
            if (dailyQuestProgressText != null)
            {
                dailyQuestProgressText.text = "0/5 已完成";
            }
        }
        
        /// <summary>
        /// 更新成就进度
        /// </summary>
        private void UpdateAchievementProgress()
        {
            if (achievementManager == null)
                return;
            
            int unlockedCount = achievementManager.GetUnlockedCount();
            int totalCount = achievementManager.GetTotalCount();
            float progress = totalCount > 0 ? (float)unlockedCount / totalCount * 100f : 0f;
            
            if (achievementProgressText != null)
            {
                achievementProgressText.text = $"{unlockedCount}/{totalCount}";
            }
            
            if (achievementProgressBar != null)
            {
                achievementProgressBar.fillAmount = progress / 100f;
            }
        }
        
        /// <summary>
        /// 更新章节进度
        /// </summary>
        private void UpdateChapterProgress()
        {
            if (chapterContainer == null || chapterItemPrefab == null || guestStatistics == null)
                return;
            
            ClearChapterList();
            
            int totalChapters = 7;
            int completedChapters = Mathf.CeilToInt(guestStatistics.DayProgress / 100f * totalChapters);
            
            for (int i = 0; i < totalChapters; i++)
            {
                CreateChapterItem(i + 1, i < completedChapters);
            }
        }
        
        /// <summary>
        /// 创建章节项
        /// </summary>
        private void CreateChapterItem(int chapterNumber, bool isCompleted)
        {
            if (chapterContainer == null || chapterItemPrefab == null)
                return;
            
            GameObject itemObj = Instantiate(chapterItemPrefab, chapterContainer);
            ChapterItem item = itemObj.GetComponent<ChapterItem>();
            
            if (item != null)
            {
                item.Initialize(chapterNumber, isCompleted);
            }
        }
        
        /// <summary>
        /// 清除章节列表
        /// </summary>
        private void ClearChapterList()
        {
            foreach (Transform child in chapterContainer)
            {
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        
        /// <summary>
        /// 添加故事任务
        /// </summary>
        public void AddStoryTask(string taskId, string taskName, int current, int total)
        {
            CreateTaskItem(storyTaskContainer, storyTaskItemPrefab, taskId, taskName, current, total);
        }
        
        /// <summary>
        /// 添加支线任务
        /// </summary>
        public void AddSideQuest(string questId, string questName, int current, int total)
        {
            CreateTaskItem(sideQuestContainer, sideQuestItemPrefab, questId, questName, current, total);
        }
        
        /// <summary>
        /// 添加每日任务
        /// </summary>
        public void AddDailyQuest(string questId, string questName, int current, int total)
        {
            CreateTaskItem(dailyQuestContainer, dailyQuestItemPrefab, questId, questName, current, total);
        }
        
        /// <summary>
        /// 创建任务项
        /// </summary>
        private void CreateTaskItem(Transform container, GameObject prefab, string id, string name, int current, int total)
        {
            if (container == null || prefab == null)
                return;
            
            GameObject itemObj = Instantiate(prefab, container);
            ProgressItem item = itemObj.GetComponent<ProgressItem>();
            
            if (item != null)
            {
                item.Initialize(name, current, total);
                progressItems[id] = item;
            }
        }
        
        /// <summary>
        /// 更新任务进度
        /// </summary>
        public void UpdateTaskProgress(string taskId, int current, int total)
        {
            if (progressItems.ContainsKey(taskId))
            {
                progressItems[taskId].SetProgress(current, total);
            }
        }
        
        /// <summary>
        /// 完成任务
        /// </summary>
        public void CompleteTask(string taskId)
        {
            if (progressItems.ContainsKey(taskId))
            {
                progressItems[taskId].SetCompleted();
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
    /// 进度项组件
    /// </summary>
    public class ProgressItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressBar;
        [SerializeField] private Image completedIcon;
        
        private string taskName;
        private int currentProgress;
        private int totalProgress;
        private bool isCompleted;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(string name, int current, int total)
        {
            taskName = name;
            currentProgress = current;
            totalProgress = total;
            isCompleted = false;
            
            UpdateDisplay();
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        private void UpdateDisplay()
        {
            if (nameText != null)
            {
                nameText.text = taskName;
                nameText.color = isCompleted ? Color.green : Color.white;
            }
            
            if (progressText != null)
            {
                progressText.text = $"{currentProgress}/{totalProgress}";
            }
            
            if (progressBar != null)
            {
                float progress = totalProgress > 0 ? (float)currentProgress / totalProgress : 0f;
                progressBar.fillAmount = progress;
                progressBar.color = isCompleted ? Color.green : Color.cyan;
            }
            
            if (completedIcon != null)
            {
                completedIcon.gameObject.SetActive(isCompleted);
            }
        }
        
        /// <summary>
        /// 设置进度
        /// </summary>
        public void SetProgress(int current, int total)
        {
            currentProgress = current;
            totalProgress = total;
            UpdateDisplay();
        }
        
        /// <summary>
        /// 设置完成状态
        /// </summary>
        public void SetCompleted()
        {
            isCompleted = true;
            currentProgress = totalProgress;
            UpdateDisplay();
            
            PlayCompleteAnimation();
        }
        
        /// <summary>
        /// 播放完成动画
        /// </summary>
        private void PlayCompleteAnimation()
        {
            if (completedIcon != null)
            {
                completedIcon.transform.localScale = Vector3.zero;
                LeanTween.scale(completedIcon.rectTransform, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack);
            }
            
            if (progressBar != null)
            {
                LeanTween.alpha(progressBar.rectTransform, 0.5f, 0.2f).setLoopPingPong().setRepeat(1);
            }
        }
    }
    
    /// <summary>
    /// 章节项组件
    /// </summary>
    public class ChapterItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI chapterNumberText;
        [SerializeField] private Image statusIcon;
        [SerializeField] private Image lineImage;
        
        private int chapterNumber;
        private bool isCompleted;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(int chapterNumber, bool isCompleted)
        {
            this.chapterNumber = chapterNumber;
            this.isCompleted = isCompleted;
            
            UpdateDisplay();
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        private void UpdateDisplay()
        {
            if (chapterNumberText != null)
            {
                chapterNumberText.text = $"第{GetChineseNumber(chapterNumber)}章";
                chapterNumberText.color = isCompleted ? Color.green : Color.gray;
            }
            
            if (statusIcon != null)
            {
                if (isCompleted)
                {
                    statusIcon.sprite = CreateCheckmarkSprite();
                }
                else
                {
                    statusIcon.sprite = CreateCircleSprite();
                }
            }
        }
        
        /// <summary>
        /// 获取中文数字
        /// </summary>
        private string GetChineseNumber(int number)
        {
            string[] chineseNumbers = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
            if (number >= 1 && number <= 10)
                return chineseNumbers[number - 1];
            return number.ToString();
        }
        
        /// <summary>
        /// 创建勾选图标
        /// </summary>
        private Sprite CreateCheckmarkSprite()
        {
            return null;
        }
        
        /// <summary>
        /// 创建圆形图标
        /// </summary>
        private Sprite CreateCircleSprite()
        {
            return null;
        }
    }
}
