using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DatingShow.Statistics
{
    /// <summary>
    /// 成就显示组件 - 用于显示单个成就
    /// </summary>
    public class AchievementDisplay : MonoBehaviour
    {
        [Header("显示元素")]
        [SerializeField] private TextMeshProUGUI iconText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Image progressBar;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image lockOverlay;
        [SerializeField] private GameObject newBadge;
        
        [Header("稀有度颜色")]
        [SerializeField] private Color commonColor = new Color(0.7f, 0.7f, 0.7f);
        [SerializeField] private Color rareColor = new Color(0.3f, 0.5f, 1f);
        [SerializeField] private Color epicColor = new Color(0.8f, 0.2f, 0.9f);
        [SerializeField] private Color legendaryColor = new Color(1f, 0.7f, 0.2f);
        
        [Header("动画配置")]
        [SerializeField] private bool enableAnimations = true;
        [SerializeField] private AnimationCurve unlockAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private Achievement achievement;
        private bool isNew;
        private RectTransform rectTransform;
        
        public Achievement Achievement => achievement;
        public bool IsNew => isNew;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(Achievement achievement, bool isNew = false)
        {
            this.achievement = achievement;
            this.isNew = isNew;
            
            rectTransform = GetComponent<RectTransform>();
            
            UpdateDisplay();
            
            if (enableAnimations && isNew)
            {
                PlayNewBadgeAnimation();
            }
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        public void UpdateDisplay()
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
            
            if (progressText != null)
            {
                if (achievement.MaxProgress > 0)
                {
                    progressText.text = $"{achievement.CurrentProgress}/{achievement.MaxProgress}";
                }
                else if (achievement.IsUnlocked)
                {
                    progressText.text = "✓";
                }
                else
                {
                    progressText.text = "";
                }
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
                
                progressBar.color = achievement.IsUnlocked ? GetRarityColor() : Color.gray;
            }
            
            if (rewardText != null)
            {
                rewardText.text = $"奖励: {achievement.Reward}";
            }
            
            if (backgroundImage != null)
            {
                Color bgColor = GetRarityColor();
                bgColor.a = achievement.IsUnlocked ? 0.3f : 0.1f;
                backgroundImage.color = bgColor;
            }
            
            if (lockOverlay != null)
            {
                lockOverlay.gameObject.SetActive(!achievement.IsUnlocked);
            }
            
            if (newBadge != null)
            {
                newBadge.SetActive(isNew && !achievement.IsUnlocked);
            }
        }
        
        /// <summary>
        /// 获取稀有度颜色
        /// </summary>
        public Color GetRarityColor()
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
        /// 设置为已读
        /// </summary>
        public void SetAsRead()
        {
            isNew = false;
            if (newBadge != null)
            {
                newBadge.SetActive(false);
            }
        }
        
        /// <summary>
        /// 播放解锁动画
        /// </summary>
        public void PlayUnlockAnimation(Action onComplete = null)
        {
            if (!enableAnimations)
            {
                onComplete?.Invoke();
                return;
            }
            
            isNew = false;
            if (newBadge != null)
            {
                newBadge.SetActive(false);
            }
            
            float duration = 0.5f;
            float elapsed = 0f;
            
            Color originalBgColor = backgroundImage.color;
            Vector3 originalScale = rectTransform.localScale;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float curveValue = unlockAnimationCurve.Evaluate(t);
                
                Color currentColor = Color.Lerp(originalBgColor, GetRarityColor() * 1.5f, curveValue);
                currentColor.a = 0.5f + curveValue * 0.3f;
                backgroundImage.color = currentColor;
                
                rectTransform.localScale = originalScale * (1f + curveValue * 0.1f);
                
                if (elapsed >= duration)
                {
                    backgroundImage.color = originalBgColor;
                    rectTransform.localScale = originalScale;
                    onComplete?.Invoke();
                }
                
                return;
            }
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 播放新标记动画
        /// </summary>
        public void PlayNewBadgeAnimation()
        {
            if (newBadge == null)
                return;
            
            newBadge.SetActive(true);
            newBadge.transform.localScale = Vector3.zero;
            
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scale = Mathf.Lerp(0f, 1.2f, t);
                
                if (t >= 1f)
                {
                    scale = 1f;
                }
                
                newBadge.transform.localScale = Vector3.one * scale;
                
                if (elapsed >= duration)
                {
                    newBadge.transform.localScale = Vector3.one;
                }
                
                return;
            }
        }
        
        /// <summary>
        /// 获取提示信息
        /// </summary>
        public string GetTooltip()
        {
            if (achievement == null)
                return "";
            
            string status = achievement.IsUnlocked ? "已解锁" : "未解锁";
            string progress = achievement.MaxProgress > 0 ? 
                $"{achievement.CurrentProgress}/{achievement.MaxProgress}" : "";
            
            return $"{achievement.Name}\n{achievement.Description}\n状态: {status}\n进度: {progress}\n奖励: {achievement.Reward}";
        }
    }
    
    /// <summary>
    /// 成就通知组件 - 用于显示成就解锁通知
    /// </summary>
    public class AchievementNotification : MonoBehaviour
    {
        [Header("显示元素")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI achievementNameText;
        [SerializeField] private TextMeshProUGUI achievementIconText;
        [SerializeField] private Image backgroundImage;
        
        [Header("动画配置")]
        [SerializeField] private float showDuration = 3f;
        [SerializeField] private float slideInDuration = 0.3f;
        [SerializeField] private float slideOutDuration = 0.3f;
        [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private RectTransform rectTransform;
        private Vector2 showPosition;
        private Vector2 hidePosition;
        private bool isShowing;
        private float showTimer;
        
        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            showPosition = rectTransform.anchoredPosition;
            hidePosition = showPosition + new Vector2(0, 200);
            
            rectTransform.anchoredPosition = hidePosition;
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 显示通知
        /// </summary>
        public void ShowNotification(Achievement achievement)
        {
            if (isShowing)
                return;
            
            gameObject.SetActive(true);
            isShowing = true;
            showTimer = 0f;
            
            UpdateDisplay(achievement);
            
            StartCoroutine(SlideInAnimation());
        }
        
        /// <summary>
        /// 更新显示
        /// </summary>
        private void UpdateDisplay(Achievement achievement)
        {
            if (titleText != null)
            {
                titleText.text = "🏆 成就解锁！";
            }
            
            if (achievementNameText != null)
            {
                achievementNameText.text = achievement.Name;
            }
            
            if (achievementIconText != null)
            {
                achievementIconText.text = achievement.Icon;
            }
            
            if (backgroundImage != null)
            {
                backgroundImage.color = GetRarityColor(achievement.Rarity);
            }
        }
        
        /// <summary>
        /// 获取稀有度颜色
        /// </summary>
        private Color GetRarityColor(AchievementRarity rarity)
        {
            switch (rarity)
            {
                case AchievementRarity.Common: return new Color(0.7f, 0.7f, 0.7f, 0.9f);
                case AchievementRarity.Rare: return new Color(0.3f, 0.5f, 1f, 0.9f);
                case AchievementRarity.Epic: return new Color(0.8f, 0.2f, 0.9f, 0.9f);
                case AchievementRarity.Legendary: return new Color(1f, 0.7f, 0.2f, 0.9f);
                default: return new Color(1f, 1f, 1f, 0.9f);
            }
        }
        
        /// <summary>
        /// 滑入动画
        /// </summary>
        private System.Collections.IEnumerator SlideInAnimation()
        {
            float elapsed = 0f;
            
            while (elapsed < slideInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / slideInDuration;
                float curveValue = slideCurve.Evaluate(t);
                
                rectTransform.anchoredPosition = Vector2.Lerp(hidePosition, showPosition, curveValue);
                
                yield return null;
            }
            
            rectTransform.anchoredPosition = showPosition;
            
            yield return new WaitForSeconds(showDuration);
            
            StartCoroutine(SlideOutAnimation());
        }
        
        /// <summary>
        /// 滑出动画
        /// </summary>
        private System.Collections.IEnumerator SlideOutAnimation()
        {
            float elapsed = 0f;
            
            while (elapsed < slideOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / slideOutDuration;
                float curveValue = slideCurve.Evaluate(t);
                
                rectTransform.anchoredPosition = Vector2.Lerp(showPosition, hidePosition, curveValue);
                
                yield return null;
            }
            
            rectTransform.anchoredPosition = hidePosition;
            gameObject.SetActive(false);
            isShowing = false;
        }
    }
}
