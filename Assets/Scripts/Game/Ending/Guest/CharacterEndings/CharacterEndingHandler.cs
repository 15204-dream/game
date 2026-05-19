using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public abstract class CharacterEndingHandler : MonoBehaviour
    {
        public string EndingId;
        public string CharacterId;
        public string CharacterName;

        [Header("UI引用")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected Image characterImage;
        [SerializeField] protected Image cgImage;
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected TextMeshProUGUI storyText;
        [SerializeField] protected Button skipButton;
        [SerializeField] protected Button replayButton;
        [SerializeField] protected Button galleryButton;

        [Header("动画配置")]
        [SerializeField] protected float fadeInDuration = 1.5f;
        [SerializeField] protected float cgRevealDuration = 2f;
        [SerializeField] protected float textScrollSpeed = 40f;

        protected bool isPlaying;
        protected bool isSkipping;
        protected string fullStoryText;
        protected GuestEndingResult endingResult;

        public event Action OnEndingCompleted;
        public event Action OnCGUnlocked;

        protected virtual void Start()
        {
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipEnding);
                skipButton.gameObject.SetActive(false);
            }
            if (replayButton != null)
            {
                replayButton.onClick.AddListener(ReplayEnding);
                replayButton.gameObject.SetActive(false);
            }
            if (galleryButton != null)
            {
                galleryButton.onClick.AddListener(OpenGallery);
                galleryButton.gameObject.SetActive(false);
            }
        }

        public virtual void PlayEnding(GuestEndingResult result)
        {
            if (isPlaying)
                return;

            isPlaying = true;
            isSkipping = false;
            endingResult = result;

            PrepareCharacterData(result);
            StartCoroutine(PlayEndingSequence());
        }

        protected abstract void PrepareCharacterData(GuestEndingResult result);

        protected abstract string GenerateStoryText(GuestEndingResult result);

        protected virtual IEnumerator PlayEndingSequence()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            yield return StartCoroutine(FadeIn());

            if (nameText != null)
            {
                nameText.text = CharacterName;
            }

            yield return StartCoroutine(ShowCharacter());

            fullStoryText = GenerateStoryText(endingResult);
            yield return StartCoroutine(DisplayStory());

            yield return StartCoroutine(RevealCG());

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
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = progress;
                }
                yield return null;
            }
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        protected virtual IEnumerator ShowCharacter()
        {
            if (characterImage == null)
                yield break;

            float elapsed = 0f;
            var startPos = characterImage.rectTransform.anchoredPosition;
            startPos.x -= 100f;
            characterImage.rectTransform.anchoredPosition = startPos;

            Color startColor = Color.clear;
            characterImage.color = startColor;

            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / 1f;
                startColor.a = progress;
                characterImage.color = startColor;
                startPos.x = Mathf.Lerp(startPos.x, startPos.x + 100f, progress);
                characterImage.rectTransform.anchoredPosition = startPos;
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

        protected virtual IEnumerator RevealCG()
        {
            if (cgImage == null)
                yield break;

            cgImage.gameObject.SetActive(true);
            float elapsed = 0f;
            Color startColor = Color.black;
            cgImage.color = startColor;

            while (elapsed < cgRevealDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / cgRevealDuration;
                startColor.a = 1f - progress;
                cgImage.color = startColor;
                yield return null;
            }

            startColor.a = 0f;
            cgImage.color = startColor;
            OnCGUnlocked?.Invoke();
        }

        protected virtual void ShowActionButtons()
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }
            if (replayButton != null)
            {
                replayButton.gameObject.SetActive(true);
            }
            if (galleryButton != null)
            {
                galleryButton.gameObject.SetActive(true);
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
            StartCoroutine(PlayEndingSequence());
        }

        protected virtual void OpenGallery()
        {
            OnEndingCompleted?.Invoke();
        }

        public bool IsPlaying => isPlaying;
        public string GetCharacterId() => CharacterId;
    }
}
