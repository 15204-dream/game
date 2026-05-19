using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 成就面板 - 显示和管理游戏成就
    /// </summary>
    public class AchievementPanel : StatisticsPanel
    {
        [Header("标题区域")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI achievementCountText;
        [SerializeField] private Image totalProgressBar;
        [SerializeField] private TextMeshProUGUI totalProgressText;
        
        [Header("成就分类")]
        [SerializeField] private ToggleGroup categoryToggleGroup;
        [SerializeField] private Toggle allCategoryToggle;
        [SerializeField] private Toggle unlockedCategoryToggle;
        [SerializeField] private Toggle lockedCategoryToggle;
        
        [Header("成就列表")]
        [SerializeField] private Transform achievementListContainer;
        [SerializeField] private GameObject achievementItemPrefab;
        [SerializeField] private ScrollRect scrollRect;
        
        [Header("成就详情")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private TextMeshProUGUI detailIconText;
        [SerializeField] private TextMeshProUGUI detailNameText;
        [SerializeField] private TextMeshProUGUI detailDescriptionText;
        [SerializeField] private TextMeshProUGUI detailProgressText;
        [SerializeField] private Image detailProgressBar;
        [SerializeField] private TextMeshProUGUI detailRewardText;
        [SerializeField] private TextMeshProUGUI detailUnlockTimeText;
        
        [Header("分类标题")]
        [SerializeField] private TextMeshProUGUI allCategoryTitle;
        [SerializeField] private TextMeshProUGUI unlockedCategoryTitle;
        [SerializeField] private TextMeshProUGUI lockedCategoryTitle;
        
        private AchievementManager achievementManager;
        private Dictionary<string, AchievementItem> achievementItems;
        private AchievementCategory currentCategory = AchievementCategory.All;
        
        public enum AchievementCategory
        {
            All,
            Unlocked,
            Locked
        }
        
        protected override void Awake()
        {
            base.Awake();
            achievementItems = new Dictionary<string, AchievementItem>();
        }
        
        /// <summary>
        /// 初始化面板
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            
            if (titleText != null)
            {
                titleText.text = "🏆 成就中心";
            }
            
            if (allCategoryTitle != null)
            {
                allCategoryTitle.text = "全部成就";
            }
            
            if (unlockedCategoryTitle != null)
            {
                unlockedCategoryTitle.text = "已解锁";
            }
            
            if (lockedCategoryTitle != null)
            {
                lockedCategoryTitle.text = "未解锁";
            }
            
            SetupCategoryToggles();
        }
        
        /// <summary>
        /// 设置成就管理器
        /// </summary>
        public void SetAchievementManager(AchievementManager manager)
        {
            achievementManager = manager;
            
            if (achievementManager != null)
            {
                achievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
                achievementManager.OnAchievementProgressChanged += OnAchievementProgressChanged;
            }
            
            UpdatePanel();
        }
        
        /// <summary>
        /// 设置分类切换
        /// </summary>
        private void SetupCategoryToggles()
        {
            if (allCategoryToggle != null)
            {
                allCategoryToggle.onValueChanged.AddListener((selected) =>
                {
                    if (selected) SetCategory(AchievementCategory.All);
                });
            }
            
            if (unlockedCategoryToggle != null)
            {
                unlockedCategoryToggle.onValueChanged.AddListener((selected) =>
                {
                    if (selected) SetCategory(AchievementCategory.Unlocked);
                });
            }
            
            if (lockedCategoryToggle != null)
            {
                lockedCategoryToggle.onValueChanged.AddListener((selected) =>
                {
                    if (selected) SetCategory(AchievementCategory.Locked);
                });
            }
        }
        
        /// <summary>
        /// 设置当前分类
        /// </summary>
        public void SetCategory(AchievementCategory category)
        {
            currentCategory = category;
            UpdatePanel();
        }
        
        /// <summary>
        /// 更新面板内容
        /// </summary>
        protected override void UpdatePanel()
        {
            if (achievementManager == null)
                return;
            
            UpdateOverallProgress();
            UpdateAchievementList();
        }
        
        /// <summary>
        /// 更新总体进度
        /// </summary>
        private void UpdateOverallProgress()
        {
            if (achievementManager == null)
                return;
            
            int unlockedCount = achievementManager.GetUnlockedCount();
            int totalCount = achievementManager.GetTotalCount();
            float progress = totalCount > 0 ? (float)unlockedCount / totalCount * 100f : 0f;
            
            if (achievementCountText != null)
            {
                achievementCountText.text = $"{unlockedCount}/{totalCount}";
            }
            
            if (totalProgressBar != null)
            {
                totalProgressBar.fillAmount = progress / 100f;
            }
            
            if (totalProgressText != null)
            {
                totalProgressText.text = $"{progress:F0}%";
            }
        }
        
        /// <summary>
        /// 更新成就列表
        /// </summary>
        private void UpdateAchievementList()
        {
            if (achievementListContainer == null || achievementItemPrefab == null || achievementManager == null)
                return;
            
            ClearAchievementList();
            
            List<Achievement> achievements = GetFilteredAchievements();
            
            foreach (var achievement in achievements)
            {
                CreateAchievementItem(achievement);
            }
        }
        
        /// <summary>
        /// 获取过滤后的成就列表
        /// </summary>
        private List<Achievement> GetFilteredAchievements()
        {
            if (achievementManager == null)
                return new List<Achievement>();
            
            List<Achievement> allAchievements = achievementManager.GetAllAchievements();
            List<Achievement> filtered = new List<Achievement>();
            
            foreach (var achievement in allAchievements)
            {
                switch (currentCategory)
                {
                    case AchievementCategory.Unlocked:
                        if (achievement.IsUnlocked)
                            filtered.Add(achievement);
                        break;
                    case AchievementCategory.Locked:
                        if (!achievement.IsUnlocked)
                            filtered.Add(achievement);
                        break;
                    default:
                        filtered.Add(achievement);
                        break;
                }
            }
            
            filtered.Sort((a, b) =>
            {
                if (a.IsUnlocked != b.IsUnlocked)
                    return a.IsUnlocked ? -1 : 1;
                return b.Rarity.CompareTo(a.Rarity);
            });
            
            return filtered;
        }
        
        /// <summary>
        /// 创建成就项
        /// </summary>
        private void CreateAchievementItem(Achievement achievement)
        {
            if (achievementListContainer == null || achievementItemPrefab == null)
                return;
            
            GameObject itemObj = Instantiate(achievementItemPrefab, achievementListContainer);
            AchievementItem item = itemObj.GetComponent<AchievementItem>();
            
            if (item != null)
            {
                item.Initialize(achievement);
                achievementItems[achievement.Id] = item;
                
                Button button = itemObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => ShowAchievementDetail(achievement.Id));
                }
            }
        }
        
        /// <summary>
        /// 清除成就列表
        /// </summary>
        private void ClearAchievementList()
        {
            foreach (var item in achievementItems.Values)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }
            achievementItems.Clear();
        }
        
        /// <summary>
        /// 显示成就详情
        /// </summary>
        public void ShowAchievementDetail(string achievementId)
        {
            if (detailPanel == null || achievementManager == null)
                return;
            
            Achievement achievement = achievementManager.GetAchievement(achievementId);
            if (achievement == null)
                return;
            
            detailPanel.SetActive(true);
            
            if (detailIconText != null)
            {
                detailIconText.text = achievement.IsUnlocked ? achievement.Icon : "🔒";
            }
            
            if (detailNameText != null)
            {
                detailNameText.text = achievement.Name;
                detailNameText.color = achievement.IsUnlocked ? Color.white : Color.gray;
            }
            
            if (detailDescriptionText != null)
            {
                detailDescriptionText.text = achievement.Description;
                detailDescriptionText.color = achievement.IsUnlocked ? Color.white : Color.gray;
            }
            
            if (detailProgressText != null)
            {
                if (achievement.MaxProgress > 0)
                {
                    detailProgressText.text = $"{achievement.CurrentProgress}/{achievement.MaxProgress}";
                }
                else
                {
                    detailProgressText.text = achievement.IsUnlocked ? "已完成" : "未完成";
                }
            }
            
            if (detailProgressBar != null)
            {
                if (achievement.MaxProgress > 0)
                {
                    detailProgressBar.fillAmount = (float)achievement.CurrentProgress / achievement.MaxProgress;
                }
                else
                {
                    detailProgressBar.fillAmount = achievement.IsUnlocked ? 1f : 0f;
                }
            }
            
            if (detailRewardText != null)
            {
                detailRewardText.text = $"奖励: {achievement.Reward}";
            }
            
            if (detailUnlockTimeText != null)
            {
                if (achievement.IsUnlocked && achievement.UnlockTime.HasValue)
                {
                    detailUnlockTimeText.text = $"解锁时间: {achievement.UnlockTime.Value:yyyy-MM-dd HH:mm}";
                }
                else
                {
                    detailUnlockTimeText.text = "";
                }
            }
        }
        
        /// <summary>
        /// 隐藏成就详情
        /// </summary>
        public void HideAchievementDetail()
        {
            if (detailPanel != null)
            {
                detailPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 成就解锁回调
        /// </summary>
        private void OnAchievementUnlocked(Achievement achievement)
        {
            UpdatePanel();
            
            if (achievementItems.ContainsKey(achievement.Id))
            {
                achievementItems[achievement.Id].PlayUnlockAnimation();
            }
        }
        
        /// <summary>
        /// 成就进度变化回调
        /// </summary>
        private void OnAchievementProgressChanged(Achievement achievement)
        {
            if (achievementItems.ContainsKey(achievement.Id))
            {
                achievementItems[achievement.Id].UpdateProgress();
            }
        }
        
        /// <summary>
        /// 滚动到指定成就
        /// </summary>
        public void ScrollToAchievement(string achievementId)
        {
            if (!achievementItems.ContainsKey(achievementId))
                return;
            
            AchievementItem item = achievementItems[achievementId];
            Canvas.ForceUpdateCanvases();
            
            if (scrollRect != null)
            {
                scrollRect.content = item.transform.parent as RectTransform;
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
    /// 成就项组件
    /// </summary>
    public class AchievementItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI iconText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image lockOverlay;
        
        [Header("稀有度颜色")]
        [SerializeField] private Color commonColor = Color.gray;
        [SerializeField] private Color rareColor = Color.blue;
        [SerializeField] private Color epicColor = Color.magenta;
        [SerializeField] private Color legendaryColor = new Color(1f, 0.6f, 0f);
        
        private Achievement achievement;
        private Animation unlockAnimation;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(Achievement achievement)
        {
            this.achievement = achievement;
            UpdateDisplay();
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        private void UpdateDisplay()
        {
            if (achievement == null)
                return;
            
            if (iconText != null)
            {
                iconText.text = achievement.IsUnlocked ? achievement.Icon : "🔒";
            }
            
            if (nameText != null)
            {
                nameText.text = achievement.Name;
                nameText.color = achievement.IsUnlocked ? GetRarityColor() : Color.gray;
            }
            
            if (descriptionText != null)
            {
                descriptionText.text = achievement.Description;
                descriptionText.color = achievement.IsUnlocked ? Color.white : Color.gray;
            }
            
            if (progressBar != null)
            {
                if (achievement.MaxProgress > 0)
                {
                    progressBar.fillAmount = (float)achievement.CurrentProgress / achievement.MaxProgress;
                }
                else
                {
                    progressBar.fillAmount = achievement.IsUnlocked ? 1f : 0f;
                }
            }
            
            if (progressText != null)
            {
                if (achievement.MaxProgress > 0)
                {
                    progressText.text = $"{achievement.CurrentProgress}/{achievement.MaxProgress}";
                }
                else
                {
                    progressText.text = achievement.IsUnlocked ? "✓" : "";
                }
            }
            
            if (lockOverlay != null)
            {
                lockOverlay.gameObject.SetActive(!achievement.IsUnlocked);
            }
            
            if (backgroundImage != null)
            {
                backgroundImage.color = achievement.IsUnlocked ? 
                    new Color(GetRarityColor().r, GetRarityColor().g, GetRarityColor().b, 0.2f) : 
                    new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }
        }
        
        /// <summary>
        /// 获取稀有度颜色
        /// </summary>
        private Color GetRarityColor()
        {
            switch (achievement.Rarity)
            {
                case AchievementRarity.Common: return commonColor;
                case AchievementRarity.Rare: return rareColor;
                case AchievementRarity.Epic: return epicColor;
                case AchievementRarity.Legendary: return legendaryColor;
                default: return Color.white;
            }
        }
        
        /// <summary>
        /// 更新进度
        /// </summary>
        public void UpdateProgress()
        {
            UpdateDisplay();
        }
        
        /// <summary>
        /// 播放解锁动画
        /// </summary>
        public void PlayUnlockAnimation()
        {
            UpdateDisplay();
            
            if (backgroundImage != null)
            {
                LeanTween.alpha(backgroundImage.rectTransform, 0.5f, 0.3f).setLoopPingPong().setRepeat(3);
            }
            
            if (iconText != null)
            {
                LeanTween.scale(iconText.rectTransform, Vector3.one * 1.3f, 0.3f).setLoopPingPong().setRepeat(2);
            }
        }
    }
}
