using UnityEngine;

namespace HeartHook.Game.Ending.Guest.MainEndings
{
    public class HE04_EndingHandler : MainEndingHandler
    {
        [Header("独自美丽特效")]
        [SerializeField] private ParticleSystem sparkleParticles;
        [SerializeField] private AudioClip soloBGM;
        [SerializeField] private Gradient backgroundGradient;

        private GuestEndingResult currentResult;

        protected override void Start()
        {
            EndingId = "HE04";
            EndingName = "独自美丽";
            base.Start();
        }

        protected override void PrepareEnding(GuestEndingResult result)
        {
            currentResult = result;

            if (backgroundImage != null && backgroundGradient != null)
            {
                backgroundImage.color = backgroundGradient.Evaluate(0f);
            }

            if (sparkleParticles != null)
            {
                var main = sparkleParticles.main;
                main.startColor = new Color(1f, 0.95f, 0.8f);
                sparkleParticles.Play();
            }
        }

        protected override string GenerateStoryText(GuestEndingResult result)
        {
            return $"你没有选择任何人，\n\n" +
                   $"不是因为没有遇见心动的人，\n\n" +
                   $"而是你深知，此刻最重要的是——\n\n" +
                   $"好好爱自己。\n\n" +
                   $"你选择了提前退出录制，\n\n" +
                   $"给自己一段独处的时间。\n\n" +
                   $"在海边的咖啡厅，你翻开日记本，\n\n" +
                   $"写下了这段旅程的感受。\n\n" +
                   $"有些故事，不必有结局。\n\n" +
                   $"有些美好，只属于自己。\n\n" +
                   $"结局达成：✦ 独自美丽 ✦";
        }

        protected override System.Collections.IEnumerator PlayEndingSequence()
        {
            float elapsed = 0f;
            float duration = fadeInDuration * 2f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                if (backgroundImage != null && backgroundGradient != null)
                {
                    backgroundImage.color = backgroundGradient.Evaluate(progress);
                }

                yield return null;
            }

            yield return base.PlayEndingSequence();
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
