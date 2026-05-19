using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HeartHook.Game.Ending.UI
{
    public class EndingTransition : MonoBehaviour
    {
        [Header("过渡组件")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image transitionImage;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private ParticleSystem effectParticles;

        [Header("过渡配置")]
        [SerializeField] private float transitionDuration = 1.5f;
        [SerializeField] private float holdDuration = 0.5f;
        [SerializeField] private Color transitionColor = Color.black;
        [SerializeField] private TransitionType transitionType = TransitionType.Fade;

        [Header("特效配置")]
        [SerializeField] private bool enableParticles = true;
        [SerializeField] private AudioClip transitionSound;

        private bool isTransitioning;
        private AudioSource audioSource;

        public event Action OnTransitionComplete;

        private void Awake()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && transitionSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = transitionSound;
                audioSource.playOnAwake = false;
            }
        }

        public void PlayTransition(Action onComplete = null)
        {
            if (isTransitioning)
                return;

            StartCoroutine(TransitionSequence(onComplete));
        }

        private IEnumerator TransitionSequence(Action onComplete)
        {
            isTransitioning = true;

            if (audioSource != null && transitionSound != null)
            {
                audioSource.Play();
            }

            yield return StartCoroutine(FadeToBlack());
            yield return new WaitForSeconds(holdDuration);
            yield return StartCoroutine(FadeFromBlack());

            if (effectParticles != null && enableParticles)
            {
                effectParticles.Play();
            }

            isTransitioning = false;
            onComplete?.Invoke();
            OnTransitionComplete?.Invoke();
        }

        private IEnumerator FadeToBlack()
        {
            if (transitionImage == null)
                yield break;

            transitionImage.gameObject.SetActive(true);
            transitionImage.color = Color.clear;

            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / transitionDuration;

                transitionImage.color = Color.Lerp(Color.clear, transitionColor, progress);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = progress;
                }

                yield return null;
            }

            transitionImage.color = transitionColor;
        }

        private IEnumerator FadeFromBlack()
        {
            if (transitionImage == null)
                yield break;

            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / transitionDuration;

                transitionImage.color = Color.Lerp(transitionColor, Color.clear, progress);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - progress;
                }

                yield return null;
            }

            transitionImage.color = Color.clear;
            transitionImage.gameObject.SetActive(false);
        }

        public void PlayQuickTransition(Action onComplete = null)
        {
            if (isTransitioning)
                return;

            StartCoroutine(QuickTransitionSequence(onComplete));
        }

        private IEnumerator QuickTransitionSequence(Action onComplete)
        {
            isTransitioning = true;

            if (transitionImage != null)
            {
                transitionImage.gameObject.SetActive(true);
                transitionImage.color = transitionColor;

                yield return new WaitForSeconds(0.1f);

                float elapsed = 0f;
                float duration = transitionDuration * 0.5f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    transitionImage.color = Color.Lerp(transitionColor, Color.clear, elapsed / duration);
                    yield return null;
                }

                transitionImage.color = Color.clear;
                transitionImage.gameObject.SetActive(false);
            }

            isTransitioning = false;
            onComplete?.Invoke();
        }

        public void ShowHint(string hint, float duration = 2f)
        {
            if (hintText == null)
                return;

            hintText.text = hint;
            hintText.gameObject.SetActive(true);

            StartCoroutine(HideHintAfterDelay(duration));
        }

        private IEnumerator HideHintAfterDelay(float duration)
        {
            yield return new WaitForSeconds(duration);

            float elapsed = 0f;
            float fadeDuration = 0.5f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                hintText.color = Color.Lerp(Color.white, Color.clear, elapsed / fadeDuration);
                yield return null;
            }

            hintText.gameObject.SetActive(false);
            hintText.color = Color.white;
        }

        public void SetTransitionType(TransitionType type)
        {
            transitionType = type;
        }

        public bool IsTransitioning => isTransitioning;
    }

    public enum TransitionType
    {
        Fade,
        Slide,
        Wipe,
        Iris
    }
}
