using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LoveBeatGuide
{
    /// <summary>
    /// 引导对话框
    /// </summary>
    public class GuideDialogue : MonoBehaviour
    {
        [Header("对话框组件")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image speakerImage;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private Image continueIcon;

        [Header("样式配置")]
        [SerializeField] private Color backgroundColor = new Color(1f, 0.95f, 0.9f, 0.95f);
        [SerializeField] private Color textColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color nameColor = new Color(0.8f, 0.4f, 0.6f, 1f);

        [Header("动画配置")]
        [SerializeField] private float fadeSpeed = 0.3f;
        [SerializeField] private float typewriterSpeed = 0.05f;
        [SerializeField] private bool enableTypewriter = true;

        [Header("布局配置")]
        [SerializeField] private Vector2 dialoguePosition = new Vector2(0.5f, 0.3f);
        [SerializeField] private float dialogueWidth = 600f;
        [SerializeField] private float dialogueHeight = 200f;

        private bool isVisible;
        private bool isTyping;
        private string currentText;
        private float displayTimer;
        private Coroutine typewriterCoroutine;

        public bool IsVisible => isVisible;

        private void Awake()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            SetupLayout();
        }

        private void Start()
        {
            SetupUI();
        }

        private void Update()
        {
            if (!isVisible)
                return;

            if (!isTyping && continueIcon != null)
            {
                continueIcon.enabled = Mathf.Sin(Time.time * 3f) > 0;
            }

            if (isTyping && Input.GetMouseButtonDown(0))
            {
                CompleteTypewriter();
            }
        }

        /// <summary>
        /// 设置布局
        /// </summary>
        private void SetupLayout()
        {
            RectTransform rectTransform = dialoguePanel?.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = dialoguePosition;
                rectTransform.anchorMax = dialoguePosition;
                rectTransform.pivot = dialoguePosition;
                rectTransform.sizeDelta = new Vector2(dialogueWidth, dialogueHeight);
            }
        }

        /// <summary>
        /// 设置UI
        /// </summary>
        private void SetupUI()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            if (contentText != null)
            {
                contentText.color = textColor;
            }

            if (speakerNameText != null)
            {
                speakerNameText.color = nameColor;
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        public void ShowDialogue(string content, string speaker = "小Nova")
        {
            if (string.IsNullOrEmpty(content))
                return;

            currentText = content;
            isVisible = true;

            if (speakerNameText != null)
            {
                speakerNameText.text = speaker;
            }

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                FadeIn();
            }

            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }

            typewriterCoroutine = StartCoroutine(TypewriterEffect(content));
        }

        /// <summary>
        /// 隐藏对话框
        /// </summary>
        public void HideDialogue()
        {
            if (!isVisible)
                return;

            isVisible = false;

            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            FadeOut();
        }

        /// <summary>
        /// 打字机效果
        /// </summary>
        private IEnumerator TypewriterEffect(string text)
        {
            isTyping = true;
            contentText.text = "";

            for (int i = 0; i <= text.Length; i++)
            {
                if (!isTyping)
                {
                    contentText.text = text;
                    yield break;
                }

                contentText.text = text.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }

            isTyping = false;
        }

        /// <summary>
        /// 完成打字机效果
        /// </summary>
        private void CompleteTypewriter()
        {
            if (!isTyping)
                return;

            isTyping = false;

            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            if (contentText != null)
            {
                contentText.text = currentText;
            }
        }

        /// <summary>
        /// 淡入
        /// </summary>
        private void FadeIn()
        {
            if (backgroundImage != null)
            {
                StartCoroutine(FadeImage(backgroundImage, 0f, backgroundColor.a, fadeSpeed));
            }
        }

        /// <summary>
        /// 淡出
        /// </summary>
        private void FadeOut()
        {
            if (backgroundImage != null)
            {
                StartCoroutine(FadeImage(backgroundImage, backgroundColor.a, 0f, fadeSpeed, () =>
                {
                    if (dialoguePanel != null)
                    {
                        dialoguePanel.SetActive(false);
                    }
                }));
            }
            else if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        /// <summary>
        /// 渐变图片透明度
        /// </summary>
        private IEnumerator FadeImage(Image image, float from, float to, float duration, System.Action onComplete = null)
        {
            float elapsed = 0f;
            Color color = image.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                color.a = Mathf.Lerp(from, to, t);
                image.color = color;
                yield return null;
            }

            color.a = to;
            image.color = color;
            onComplete?.Invoke();
        }

        /// <summary>
        /// 设置对话框位置
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            dialoguePosition = position;
            SetupLayout();
        }

        /// <summary>
        /// 设置对话框大小
        /// </summary>
        public void SetSize(float width, float height)
        {
            dialogueWidth = width;
            dialogueHeight = height;
            SetupLayout();
        }

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        public void SetBackgroundColor(Color color)
        {
            backgroundColor = color;
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
        }

        /// <summary>
        /// 设置文字颜色
        /// </summary>
        public void SetTextColor(Color color)
        {
            textColor = color;
            if (contentText != null)
            {
                contentText.color = color;
            }
        }

        /// <summary>
        /// 设置说话人名字颜色
        /// </summary>
        public void SetSpeakerNameColor(Color color)
        {
            nameColor = color;
            if (speakerNameText != null)
            {
                speakerNameText.color = color;
            }
        }

        /// <summary>
        /// 设置打字机速度
        /// </summary>
        public void SetTypewriterSpeed(float speed)
        {
            typewriterSpeed = speed;
        }

        /// <summary>
        /// 启用/禁用打字机效果
        /// </summary>
        public void SetTypewriterEnabled(bool enabled)
        {
            enableTypewriter = enabled;
            if (!enabled && isTyping)
            {
                CompleteTypewriter();
            }
        }

        /// <summary>
        /// 显示多行对话
        /// </summary>
        public void ShowMultipleDialogues(System.Collections.Generic.List<GuideDialogueLine> dialogues, System.Action onComplete = null)
        {
            StartCoroutine(ShowDialoguesCoroutine(dialogues, onComplete));
        }

        /// <summary>
        /// 显示多行对话协程
        /// </summary>
        private IEnumerator ShowDialoguesCoroutine(System.Collections.Generic.List<GuideDialogueLine> dialogues, System.Action onComplete)
        {
            foreach (var dialogue in dialogues)
            {
                ShowDialogue(dialogue.text, dialogue.speaker);
                yield return new WaitForSeconds(dialogue.displayTime);
            }

            HideDialogue();
            onComplete?.Invoke();
        }
    }
}
