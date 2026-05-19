using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace HeartHook.Game.Ending.UI
{
    public class EndingScreen : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private CanvasGroup mainCanvasGroup;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI storyText;
        [SerializeField] private TextMeshProUGUI statsText;

        [Header("按钮")]
        [SerializeField] private Button skipButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button galleryButton;
        [SerializeField] private Button nextChapterButton;

        [Header("效果组件")]
        [SerializeField] private ParticleSystem backgroundParticles;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Image fadeOverlay;

        [Header("配置")]
        [SerializeField] private float fadeInDuration = 1.5f;
        [SerializeField] private float textScrollSpeed = 40f;
        [SerializeField] private float autoSkipDelay = 10f;

        private EndingData currentEnding;
        private bool isPlaying;
        private bool isSkipping;
        private string fullStoryText;
        private int currentCharIndex;
        private List<string> storySegments = new List<string>();
        private int currentSegmentIndex;

        public event Action OnEndingCompleted;
        public event Action OnReplayRequested;
        public event Action OnGalleryOpened;

        private void Start()
        {
            InitializeButtons();
            gameObject.SetActive(false);
        }

        private void InitializeButtons()
        {
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipStory);
                skipButton.gameObject.SetActive(false);
            }

            if (replayButton != null)
            {
                replayButton.onClick.AddListener(RequestReplay);
                replayButton.gameObject.SetActive(false);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
                mainMenuButton.gameObject.SetActive(false);
            }

            if (galleryButton != null)
            {
                galleryButton.onClick.AddListener(OpenGallery);
                galleryButton.gameObject.SetActive(false);
            }

            if (nextChapterButton != null)
            {
                nextChapterButton.onClick.AddListener(LoadNextChapter);
                nextChapterButton.gameObject.SetActive(false);
            }
        }

        public void ShowEnding(EndingData ending)
        {
            if (ending == null)
            {
                Debug.LogWarning("[EndingScreen] 结局数据为空！");
                return;
            }

            currentEnding = ending;
            gameObject.SetActive(true);
            isPlaying = true;
            isSkipping = false;
            currentCharIndex = 0;
            currentSegmentIndex = 0;

            PrepareEnding(ending);
            StartCoroutine(PlayEndingSequence());
        }

        private void PrepareEnding(EndingData ending)
        {
            if (titleText != null)
            {
                titleText.text = ending.GetDisplayName();
                titleText.color = ending.TitleColor;
            }

            if (subtitleText != null)
            {
                subtitleText.text = ending.GetCategoryPrefix();
            }

            if (backgroundImage != null && ending.BackgroundImage != null)
            {
                backgroundImage.sprite = ending.BackgroundImage;
            }

            if (characterImage != null && ending.CharacterImage != null)
            {
                characterImage.sprite = ending.CharacterImage;
                characterImage.gameObject.SetActive(true);
            }

            if (audioSource != null && ending.BGM != null)
            {
                audioSource.clip = ending.BGM;
                audioSource.Play();
            }

            fullStoryText = ending.Summary;
        }

        private System.Collections.IEnumerator PlayEndingSequence()
        {
            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.color = Color.black;
            }

            if (mainCanvasGroup != null)
            {
                mainCanvasGroup.alpha = 0f;
            }

            yield return StartCoroutine(FadeIn());

            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(true);
            }

            yield return StartCoroutine(DisplayStory());

            yield return new WaitForSeconds(autoSkipDelay);

            ShowActionButtons();

            if (backgroundParticles != null)
            {
                backgroundParticles.Play();
            }

            isPlaying = false;
        }

        private System.Collections.IEnumerator FadeIn()
        {
            if (fadeOverlay == null)
                yield break;

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeInDuration;
                fadeOverlay.color = Color.Lerp(Color.black, Color.clear, progress);

                if (mainCanvasGroup != null)
                {
                    mainCanvasGroup.alpha = progress;
                }

                yield return null;
            }

            fadeOverlay.gameObject.SetActive(false);
        }

        private System.Collections.IEnumerator DisplayStory()
        {
            if (storyText == null || string.IsNullOrEmpty(fullStoryText))
                yield break;

            storyText.text = "";

            while (currentCharIndex < fullStoryText.Length)
            {
                if (isSkipping)
                {
                    storyText.text = fullStoryText;
                    yield break;
                }

                currentCharIndex++;
                storyText.text = fullStoryText.Substring(0, currentCharIndex);

                if (currentCharIndex % 100 == 0)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(1f / textScrollSpeed);
                }
            }
        }

        private void ShowActionButtons()
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }

            if (replayButton != null)
            {
                replayButton.gameObject.SetActive(true);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.gameObject.SetActive(true);
            }

            if (galleryButton != null)
            {
                galleryButton.gameObject.SetActive(true);
            }
        }

        public void ShowStats(string stats)
        {
            if (statsText != null)
            {
                statsText.text = stats;
                statsText.gameObject.SetActive(true);
            }
        }

        public void ShowNextChapterButton(string sceneName)
        {
            if (nextChapterButton != null)
            {
                nextChapterButton.gameObject.SetActive(true);
            }
        }

        private void SkipStory()
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
        }

        private void RequestReplay()
        {
            OnReplayRequested?.Invoke();
        }

        private void ReturnToMainMenu()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            OnEndingCompleted?.Invoke();
            SceneManager.LoadScene("MainMenu");
        }

        private void OpenGallery()
        {
            OnGalleryOpened?.Invoke();
        }

        private void LoadNextChapter()
        {
            OnEndingCompleted?.Invoke();
        }

        public void Hide()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }

            gameObject.SetActive(false);
        }
    }
}
