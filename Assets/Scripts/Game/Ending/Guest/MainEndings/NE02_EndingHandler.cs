using UnityEngine;
using System.Collections;

namespace HeartHook.Game.Ending.Guest.MainEndings
{
    public class NE02_EndingHandler : MainEndingHandler
    {
        [Header("擦肩而过特效")]
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private AudioClip lonelySound;
        [SerializeField] private Image vignetteImage;
        [SerializeField] private float vignetteIntensity = 0.5f;

        private GuestEndingResult currentResult;

        protected override void Start()
        {
            EndingId = "NE02";
            EndingName = "擦肩而过";
            base.Start();
        }

        protected override void PrepareEnding(GuestEndingResult result)
        {
            currentResult = result;

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.5f, 0.5f, 0.55f);
            }

            if (vignetteImage != null)
            {
                Color vignetteColor = Color.black;
                vignetteColor.a = vignetteIntensity;
                vignetteImage.color = vignetteColor;
                vignetteImage.gameObject.SetActive(true);
            }

            if (dustParticles != null)
            {
                var main = dustParticles.main;
                main.startColor = new Color(0.7f, 0.7f, 0.7f, 0.3f);
                dustParticles.Play();
            }
        }

        protected override string GenerateStoryText(GuestEndingResult result)
        {
            var characterName = result.HighestCharacter ?? "TA";

            return $"节目结束了，你拎着行李箱走出录制现场。\n\n" +
                   $"回望那栋承载了无数回忆的小屋，\n\n" +
                   $"你没有看到{characterName}的身影。\n\n" +
                   $"也许这就是缘分吧——\n\n" +
                   $"有些人注定只是生命中的过客。\n\n" +
                   $"你戴上耳机，走进了人海。\n\n" +
                   $"这座城市的霓虹依然闪烁，\n\n" +
                   $"但你知道，前方还有更长的路要走。\n\n" +
                   $"也许下一次，会是不同的故事。\n\n" +
                   $"结局达成：· 擦肩而过 ·";
        }

        protected override IEnumerator PlayEndingSequence()
        {
            yield return StartCoroutine(AnimateVignette());
            yield return base.PlayEndingSequence();
        }

        private IEnumerator AnimateVignette()
        {
            if (vignetteImage == null)
                yield break;

            float elapsed = 0f;
            float duration = fadeInDuration;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                Color color = Color.black;
                color.a = vignetteIntensity * (1f - progress) + 0.1f;
                vignetteImage.color = color;
                yield return null;
            }

            Color finalColor = Color.black;
            finalColor.a = 0.1f;
            vignetteImage.color = finalColor;
        }

        protected override UnityEngine.Coroutine StartCoroutine(UnityEngine.Coroutine routine)
        {
            return base.StartCoroutine(routine);
        }

        protected override GuestEndingResult GetCurrentResult()
        {
            return currentResult;
        }
    }
}
