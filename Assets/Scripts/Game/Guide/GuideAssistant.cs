using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导助手（小Nova）
    /// </summary>
    public class GuideAssistant : MonoBehaviour
    {
        private static GuideAssistant instance;
        public static GuideAssistant Instance => instance;

        [Header("UI引用")]
        [SerializeField] private GameObject assistantRoot;
        [SerializeField] private Image assistantImage;
        [SerializeField] private TextMeshProUGUI assistantNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject bubbleObject;
        [SerializeField] private Image bubbleBackground;

        [Header("配置")]
        [SerializeField] private GuideAssistantConfig config;
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float fadeSpeed = 0.3f;

        [Header("动画")]
        [SerializeField] private bool enableBounceAnimation = true;
        [SerializeField] private float bounceSpeed = 1f;
        [SerializeField] private float bounceAmount = 5f;

        private bool isVisible;
        private bool isAnimating;
        private float timer;
        private float bubbleAlpha;
        private Vector3 originalPosition;
        private List<GuideDialogueLine> currentDialogues;
        private int currentDialogueIndex;

        public bool IsVisible => isVisible;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeAssistant();
        }

        private void Start()
        {
            if (assistantRoot != null)
            {
                originalPosition = assistantRoot.transform.localPosition;
                assistantRoot.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isVisible)
                return;

            timer -= Time.deltaTime;

            if (timer <= 0 && !isAnimating)
            {
                AdvanceDialogue();
            }

            if (enableBounceAnimation && isVisible && !isAnimating)
            {
                UpdateBounceAnimation();
            }
        }

        /// <summary>
        /// 初始化助手
        /// </summary>
        private void InitializeAssistant()
        {
            if (config != null)
            {
                if (assistantNameText != null)
                {
                    assistantNameText.text = config.assistantName;
                }

                if (config.assistantSprite != null && assistantImage != null)
                {
                    assistantImage.sprite = config.assistantSprite;
                }

                bubbleAlpha = config.bubbleColor.a;
            }

            if (bubbleBackground != null)
            {
                Color color = bubbleBackground.color;
                color.a = 0f;
                bubbleBackground.color = color;
            }
        }

        /// <summary>
        /// 显示助手
        /// </summary>
        public void ShowAssistant(string title, string description)
        {
            if (assistantRoot == null || isVisible)
                return;

            ShowBubble(description);
        }

        /// <summary>
        /// 显示助手对话框
        /// </summary>
        public void ShowAssistantDialogue(string title, string dialogue)
        {
            if (assistantRoot == null)
                return;

            if (!isVisible)
            {
                assistantRoot.SetActive(true);
                FadeIn();
            }

            if (assistantNameText != null)
            {
                assistantNameText.text = title;
            }

            if (dialogueText != null)
            {
                dialogueText.text = dialogue;
            }

            timer = displayDuration;
        }

        /// <summary>
        /// 显示气泡
        /// </summary>
        public void ShowBubble(string text)
        {
            if (bubbleObject == null || dialogueText == null)
                return;

            if (!isVisible)
            {
                assistantRoot.SetActive(true);
                FadeIn();
            }

            dialogueText.text = text;
            bubbleObject.SetActive(true);
            timer = displayDuration;
            isVisible = true;
        }

        /// <summary>
        /// 显示多行对话
        /// </summary>
        public void ShowDialogues(List<GuideDialogueLine> dialogues)
        {
            if (dialogues == null || dialogues.Count == 0)
                return;

            currentDialogues = dialogues;
            currentDialogueIndex = 0;

            ShowNextDialogue();
        }

        /// <summary>
        /// 显示下一行对话
        /// </summary>
        private void ShowNextDialogue()
        {
            if (currentDialogues == null || currentDialogueIndex >= currentDialogues.Count)
            {
                HideAssistant();
                return;
            }

            GuideDialogueLine line = currentDialogues[currentDialogueIndex];
            
            if (assistantNameText != null)
            {
                assistantNameText.text = line.speaker;
            }

            if (dialogueText != null)
            {
                dialogueText.text = line.text;
            }

            timer = line.displayTime > 0 ? line.displayTime : displayDuration;
            isVisible = true;
        }

        /// <summary>
        /// 推进对话
        /// </summary>
        private void AdvanceDialogue()
        {
            currentDialogueIndex++;

            if (currentDialogues != null && currentDialogueIndex < currentDialogues.Count)
            {
                ShowNextDialogue();
            }
            else
            {
                HideAssistant();
            }
        }

        /// <summary>
        /// 隐藏助手
        /// </summary>
        public void HideAssistant()
        {
            if (!isVisible || assistantRoot == null)
                return;

            FadeOut();
        }

        /// <summary>
        /// 淡入
        /// </summary>
        private void FadeIn()
        {
            isAnimating = true;
            bubbleAlpha = 0f;

            StartCoroutine(FadeRoutine(true));
        }

        /// <summary>
        /// 淡出
        /// </summary>
        private void FadeOut()
        {
            isAnimating = true;
            StartCoroutine(FadeRoutine(false));
        }

        /// <summary>
        /// 淡入淡出协程
        /// </summary>
        private System.Collections.IEnumerator FadeRoutine(bool fadeIn)
        {
            float targetAlpha = fadeIn ? (config?.bubbleColor.a ?? 1f) : 0f;
            float startAlpha = bubbleAlpha;
            float elapsed = 0f;

            while (elapsed < fadeSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeSpeed;
                bubbleAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                if (bubbleBackground != null)
                {
                    Color color = bubbleBackground.color;
                    color.a = bubbleAlpha;
                    bubbleBackground.color = color;
                }

                yield return null;
            }

            bubbleAlpha = targetAlpha;

            if (!fadeIn)
            {
                assistantRoot?.SetActive(false);
                bubbleObject?.SetActive(false);
                isVisible = false;
            }

            isAnimating = false;
        }

        /// <summary>
        /// 更新弹跳动画
        /// </summary>
        private void UpdateBounceAnimation()
        {
            if (assistantRoot == null)
                return;

            float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
            Vector3 position = originalPosition;
            position.y += bounce;
            assistantRoot.transform.localPosition = position;
        }

        /// <summary>
        /// 显示完成消息
        /// </summary>
        public void ShowCompletionMessage(string sequenceName)
        {
            string message = $"恭喜完成引导：{sequenceName}！\n你做得太棒啦~";
            ShowBubble(message);
            timer = 5f;
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        public void ShowTip(string tip)
        {
            ShowBubble(tip);
            timer = 3f;
        }

        /// <summary>
        /// 显示鼓励消息
        /// </summary>
        public void ShowEncouragement()
        {
            string[] encouragements = new string[]
            {
                "加油！你可以的~",
                "做得很棒哦！",
                "继续努力！",
                "相信你一定能行！",
                "太厉害了！"
            };

            string message = encouragements[UnityEngine.Random.Range(0, encouragements.Length)];
            ShowBubble(message);
            timer = 2f;
        }

        /// <summary>
        /// 设置助手位置
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            if (assistantRoot != null)
            {
                assistantRoot.transform.position = position;
                originalPosition = position;
            }
        }

        /// <summary>
        /// 重置助手位置
        /// </summary>
        public void ResetPosition()
        {
            if (assistantRoot != null)
            {
                assistantRoot.transform.localPosition = originalPosition;
            }
        }

        /// <summary>
        /// 跳过当前对话
        /// </summary>
        public void SkipDialogue()
        {
            if (currentDialogues != null && currentDialogueIndex < currentDialogues.Count - 1)
            {
                currentDialogueIndex = currentDialogues.Count - 1;
                ShowNextDialogue();
            }
            else
            {
                HideAssistant();
            }
        }
    }
}
