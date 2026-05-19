using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace HeartHook.Game.Ending.Guest.MainEndings
{
    public abstract class MainEndingHandler : MonoBehaviour
    {
        public string EndingId;
        public string EndingName;

        [Header("UI引用")]
        [SerializeField] protected CanvasGroup mainCanvasGroup;
        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected Image fadeOverlay;
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI storyText;
        [SerializeField] protected Button skipButton;
        [SerializeField] protected Button replayButton;
        [SerializeField] protected Button mainMenuButton;

        [Header("动画配置")]
        [SerializeField] protected float fadeInDuration = 1.5f;
        [SerializeField] protected float titleDisplayDuration = 2f;
        [SerializeField] protected float textScrollSpeed = 30f;
        [SerializeField] protected float autoSkipDelay = 8f;

        protected bool isPlaying;
        protected bool isSkipping;
        protected string fullStoryText;
        protected float displayProgress;

        public event Action OnEndingCompleted;
        public event Action OnReplayRequested;

        protected virtual void Start()
        {
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipEnding);
            }
            if (replayButton != null)
            {
                replayButton.onClick.AddListener(ReplayEnding);
                replayButton.gameObject.SetActive(false);
            }
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
                mainMenuButton.gameObject.SetActive(false);
            }
        }

        public virtual void PlayEnding(GuestEndingResult result)
        {
            if (isPlaying)
                return;

            isPlaying = true;
            isSkipping = false;
            displayProgress = 0f;
            
            PrepareEnding(result);
            StartCoroutine(PlayEndingSequence());
        }

        protected abstract void PrepareEnding(GuestEndingResult result);

        protected abstract string GenerateStoryText(GuestEndingResult result);

        protected virtual IEnumerator PlayEndingSequence()
        {
            if (mainCanvasGroup != null)
            {
                mainCanvasGroup.alpha = 0f;
            }

            yield return StartCoroutine(FadeIn());

            if (titleText != null)
            {
                titleText.text = EndingName;
                yield return StartCoroutine(DisplayTitle());
            }

            fullStoryText = GenerateStoryText(GetCurrentResult());
            yield return StartCoroutine(DisplayStory());

            yield return new WaitForSeconds(autoSkipDelay);

            ShowActionButtons();
            isPlaying = false;
        }

        protected virtual IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeInDuration;
                
                if (mainCanvasGroup != null)
                {
                    mainCanvasGroup.alpha = progress;
                }

                yield return null;
            }

            if (mainCanvasGroup != null)
            {
                mainCanvasGroup.alpha = 1f;
            }
        }

        protected virtual IEnumerator DisplayTitle()
        {
            float elapsed = 0f;
            Color startColor = titleText.color;
            startColor.a = 0f;
            titleText.color = startColor;

            while (elapsed < titleDisplayDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / (titleDisplayDuration * 0.5f));
                startColor.a = progress;
                titleText.color = startColor;
                yield return null;
            }
        }

        protected virtual IEnumerator DisplayStory()
        {
            if (storyText == null || string.IsNullOrEmpty(fullStoryText))
                yield break;

            storyText.text = "";
            int charIndex = 0;

            while (charIndex < fullStoryText.Length)
            {
                if (isSkipping)
                {
                    storyText.text = fullStoryText;
                    yield break;
                }

                charIndex++;
                storyText.text = fullStoryText.Substring(0, charIndex);
                yield return new WaitForSeconds(1f / textScrollSpeed);
            }
        }

        protected virtual void ShowActionButtons()
        {
            if (replayButton != null)
            {
                replayButton.gameObject.SetActive(true);
            }
            if (mainMenuButton != null)
            {
                mainMenuButton.gameObject.SetActive(true);
            }
        }

        protected virtual void SkipEnding()
        {
            if (!isPlaying)
                return;

            isSkipping = true;
            StopAllCoroutines();

            if (storyText != null)
            {
                storyText.text = fullStoryText;
            }

            ShowActionButtons();
            isPlaying = false;
            OnEndingCompleted?.Invoke();
        }

        protected virtual void ReplayEnding()
        {
            OnReplayRequested?.Invoke();
            StartCoroutine(PlayEndingSequence());
        }

        protected virtual void ReturnToMainMenu()
        {
            OnEndingCompleted?.Invoke();
        }

        protected abstract GuestEndingResult GetCurrentResult();

        public bool IsPlaying => isPlaying;
    }
}
