using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending.Guest.SecretEndings
{
    public class SecretEndingHandler : MonoBehaviour
    {
        public static SecretEndingHandler Instance { get; private set; }

        [SerializeField] private SecretEndingHandlerBase[] secretEndingHandlers;

        private SecretEndingData currentEndingData;

        public event Action<string> OnSecretEndingTriggered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            if (secretEndingHandlers == null || secretEndingHandlers.Length == 0)
            {
                secretEndingHandlers = GetComponentsInChildren<SecretEndingHandlerBase>();
            }
        }

        public bool CanUnlockSecretEnding(string endingId)
        {
            var handler = GetHandler(endingId);
            return handler != null && handler.CheckCondition();
        }

        public void TriggerSecretEnding(string endingId)
        {
            var handler = GetHandler(endingId);
            if (handler == null)
            {
                Debug.LogWarning($"[SecretEndingHandler] 找不到彩蛋结局处理器: {endingId}");
                return;
            }

            currentEndingData = new SecretEndingData
            {
                EndingId = endingId,
                Handler = handler
            };

            handler.PlayEnding();
            OnSecretEndingTriggered?.Invoke(endingId);
        }

        public SecretEndingHandlerBase GetHandler(string endingId)
        {
            foreach (var handler in secretEndingHandlers)
            {
                if (handler != null && handler.EndingId == endingId)
                {
                    return handler;
                }
            }
            return null;
        }

        public SecretEndingHandlerBase[] GetAllHandlers()
        {
            return secretEndingHandlers;
        }

        public string[] GetUnlockedSecretEndings()
        {
            var unlocked = new List<string>();
            foreach (var handler in secretEndingHandlers)
            {
                if (handler != null && handler.IsUnlocked)
                {
                    unlocked.Add(handler.EndingId);
                }
            }
            return unlocked.ToArray();
        }

        public SecretEndingData GetCurrentEndingData()
        {
            return currentEndingData;
        }
    }

    public class SecretEndingData
    {
        public string EndingId;
        public SecretEndingHandlerBase Handler;
        public bool IsViewed;
        public DateTime UnlockTime;
    }

    public abstract class SecretEndingHandlerBase : MonoBehaviour
    {
        public string EndingId;
        public string EndingName;
        public bool IsUnlocked { get; protected set; }

        [Header("UI引用")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected Image cgImage;
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI storyText;
        [SerializeField] protected ParticleSystem effectParticles;

        [Header("动画配置")]
        [SerializeField] protected float revealDuration = 3f;
        [SerializeField] protected float textScrollSpeed = 50f;

        protected bool isPlaying;
        protected string fullStoryText;

        public event Action OnEndingCompleted;

        protected abstract bool CheckConditionInternal();
        protected abstract string GenerateStoryText();

        public virtual bool CheckCondition()
        {
            return CheckConditionInternal();
        }

        public virtual void PlayEnding()
        {
            if (isPlaying)
                return;

            IsUnlocked = true;
            isPlaying = true;
            fullStoryText = GenerateStoryText();
            StartCoroutine(PlayEndingSequence());
        }

        protected virtual System.Collections.IEnumerator PlayEndingSequence()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            yield return StartCoroutine(RevealEffect());

            if (titleText != null)
            {
                titleText.text = EndingName;
                titleText.gameObject.SetActive(true);
            }

            yield return StartCoroutine(DisplayStory());

            yield return new WaitForSeconds(3f);

            if (canvasGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = 1f - elapsed;
                    yield return null;
                }
            }

            isPlaying = false;
            OnEndingCompleted?.Invoke();
        }

        protected virtual System.Collections.IEnumerator RevealEffect()
        {
            if (cgImage != null)
            {
                cgImage.gameObject.SetActive(true);
                Color color = Color.black;
                cgImage.color = color;

                float elapsed = 0f;
                while (elapsed < revealDuration)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / revealDuration;
                    color.a = 1f - progress;
                    cgImage.color = color;
                    yield return null;
                }
            }

            if (effectParticles != null)
            {
                effectParticles.Play();
            }

            if (canvasGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = elapsed;
                    yield return null;
                }
            }
        }

        protected virtual System.Collections.IEnumerator DisplayStory()
        {
            if (storyText == null || string.IsNullOrEmpty(fullStoryText))
                yield break;

            storyText.text = "";
            int charIndex = 0;

            while (charIndex < fullStoryText.Length)
            {
                charIndex++;
                storyText.text = fullStoryText.Substring(0, charIndex);
                yield return new WaitForSeconds(1f / textScrollSpeed);
            }
        }

        public bool IsPlaying => isPlaying;
    }
}
