using UnityEngine;
using System.Collections;

namespace HeartHook.Game.Ending.Guest.SecretEndings
{
    public class CE02_EndingHandler : SecretEndingHandlerBase
    {
        [Header("修罗场特效")]
        [SerializeField] private Color backgroundColor = new Color(1f, 0.6f, 0.6f);
        [SerializeField] private ParticleSystem conflictParticles;
        [SerializeField] private AudioClip dramaticSound;
        [SerializeField] private Image vignetteImage;
        [SerializeField] private float vignetteIntensity = 0.6f;

        protected override void Start()
        {
            EndingId = "SE02";
            EndingName = "修罗场";
            base.Start();
        }

        protected override bool CheckConditionInternal()
        {
            if (GuestEndingSystem.Instance == null)
                return false;

            var unlocker = new EndingUnlocker();
            var allFav = unlocker.GetAllFavorability();
            int countAbove60 = 0;

            foreach (var fav in allFav.Values)
            {
                if (fav >= 60)
                    countAbove60++;
            }

            return countAbove60 >= 6;
        }

        protected override string GenerateStoryText()
        {
            return $"录制最后一天，场面一度失控。\n\n" +
                   $"六个人同时向你表白，\n\n" +
                   $"空气中弥漫着火药味。\n\n" +
                   $"\"他/她是我的！\"\n\n" +
                   $"\"凭什么！公平竞争！\"\n\n" +
                   $"摄像机疯狂拍摄，\n\n" +
                   $"这一幕将成为本季最经典的画面。\n\n" +
                   $"而你站在人群中央，\n\n" +
                   $"感受着来自四面八方的热情。\n\n" +
                   $"\"大家...冷静一下...\"\n\n" +
                   $"但谁也没有要停下来的意思。\n\n" +
                   $"【彩蛋结局 · 修罗场】\n\n" +
                   $"⚡ 传说级名场面 ⚡";
        }

        protected override IEnumerator PlayEndingSequence()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            if (vignetteImage != null)
            {
                vignetteImage.gameObject.SetActive(true);
            }

            yield return base.PlayEndingSequence();

            if (conflictParticles != null)
            {
                conflictParticles.Play();
            }

            yield return new WaitForSeconds(2f);
            StartCoroutine(AnimateVignette());
        }

        private IEnumerator AnimateVignette()
        {
            if (vignetteImage == null)
                yield break;

            float elapsed = 0f;
            while (elapsed < 2f)
            {
                elapsed += Time.deltaTime;
                float pulse = Mathf.Sin(elapsed * 3f) * 0.2f + vignetteIntensity;
                Color color = Color.black;
                color.a = pulse;
                vignetteImage.color = color;
                yield return null;
            }
        }
    }
}
