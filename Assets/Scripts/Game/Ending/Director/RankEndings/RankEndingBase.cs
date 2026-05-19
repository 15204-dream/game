using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace HeartHook.Game.Ending.Director.RankEndings
{
    public abstract class RankEndingBase : MonoBehaviour
    {
        public string RankId;
        public string RankName;

        [Header("UI引用")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected Image badgeImage;
        [SerializeField] protected TextMeshProUGUI rankText;
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI statsText;
        [SerializeField] protected TextMeshProUGUI achievementText;
        [SerializeField] protected Button continueButton;
        [SerializeField] protected ParticleSystem rankParticles;

        [Header("动画配置")]
        [SerializeField] protected float fadeInDuration = 1.5f;
        [SerializeField] protected float badgeRevealDuration = 1f;
        [SerializeField] protected float statsScrollSpeed = 50f;

        protected bool isPlaying;
        protected DirectorEndingResult endingResult;

        public event Action OnEndingCompleted;

        protected virtual void Start()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(ContinueToNext);
                continueButton.gameObject.SetActive(false);
            }
        }

        public virtual void PlayEnding(DirectorEndingResult result)
        {
            if (isPlaying)
                return;

            isPlaying = true;
            endingResult = result;

            StartCoroutine(PlayEndingSequence());
        }

        protected abstract string GenerateStatsText(DirectorEndingResult result);

        protected abstract string GenerateAchievementText(DirectorEndingResult result);

        protected abstract Color GetRankColor();

        protected virtual IEnumerator PlayEndingSequence()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            yield return StartCoroutine(FadeIn());

            yield return StartCoroutine(ShowRankBadge());

            if (titleText != null)
            {
                titleText.text = $"评定等级：{RankName}";
                titleText.color = GetRankColor();
            }

            yield return StartCoroutine(DisplayStats());

            yield return StartCoroutine(DisplayAchievements());

            if (rankParticles != null)
            {
                rankParticles.Play();
            }

            ShowContinueButton();
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

        protected virtual IEnumerator ShowRankBadge()
        {
            if (badgeImage == null)
                yield break;

            badgeImage.gameObject.SetActive(true);
            float elapsed = 0f;

            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;
            badgeImage.transform.localScale = startScale;

            while (elapsed < badgeRevealDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / badgeRevealDuration;
                float scaleProgress = Mathf.SmoothStep(0f, 1f, progress);
                badgeImage.transform.localScale = Vector3.Lerp(startScale, endScale, scaleProgress);
                yield return null;
            }

            badgeImage.transform.localScale = endScale;
        }

        protected virtual IEnumerator DisplayStats()
        {
            if (statsText == null)
                yield break;

            string fullStats = GenerateStatsText(endingResult);
            statsText.text = "";
            int charIndex = 0;

            while (charIndex < fullStats.Length)
            {
                charIndex++;
                statsText.text = fullStats.Substring(0, charIndex);
                yield return new WaitForSeconds(1f / statsScrollSpeed);
            }

            yield return new WaitForSeconds(1f);
        }

        protected virtual IEnumerator DisplayAchievements()
        {
            if (achievementText == null)
                yield break;

            yield return new WaitForSeconds(0.5f);

            string fullAchievements = GenerateAchievementText(endingResult);
            if (string.IsNullOrEmpty(fullAchievements))
                yield break;

            achievementText.text = "";
            int charIndex = 0;

            while (charIndex < fullAchievements.Length)
            {
                charIndex++;
                achievementText.text = fullAchievements.Substring(0, charIndex);
                yield return new WaitForSeconds(1f / statsScrollSpeed);
            }
        }

        protected virtual void ShowContinueButton()
        {
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
        }

        protected virtual void ContinueToNext()
        {
            OnEndingCompleted?.Invoke();
        }

        public bool IsPlaying => isPlaying;
    }
}
