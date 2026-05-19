using UnityEngine;

namespace HeartHook.Game.Ending.Guest.SecretEndings
{
    public class CE03_EndingHandler : SecretEndingHandlerBase
    {
        [Header("恋综之外特效")]
        [SerializeField] private Color backgroundColor = new Color(0.3f, 0.35f, 0.4f);
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private AudioClip nostalgicSound;
        [SerializeField] private Sprite[] memoryImages;
        [SerializeField] private Image slideshowImage;

        private int currentSlideIndex;
        private bool isPlaying;

        protected override void Start()
        {
            EndingId = "SE03";
            EndingName = "恋综之外";
            base.Start();
        }

        protected override bool CheckConditionInternal()
        {
            if (GuestEndingSystem.Instance == null)
                return false;

            var unlocker = new EndingUnlocker();
            var maxFav = unlocker.GetMaxFavorability();
            var currentDay = unlocker.GetCurrentDay();

            return maxFav >= 50 && currentDay >= 14;
        }

        protected override string GenerateStoryText()
        {
            return $"节目结束了，但故事还在继续。\n\n" +
                   $"那些在镜头前的欢笑与泪水，\n\n" +
                   $"那些深夜阳台的私语，\n\n" +
                   $"那些心跳加速的瞬间——\n\n" +
                   $"都成为了生命中最珍贵的记忆。\n\n" +
                   $"你们约定，即使回到各自的生活，\n\n" +
                   $"也要记得这段特别的旅程。\n\n" +
                   $"也许有一天，命运会让你们再次相遇。\n\n" +
                   $"到那时，希望你们还能笑着说起——\n\n" +
                   $"那个夏天，那档恋综，那些心动。\n\n" +
                   $"【彩蛋结局 · 恋综之外】\n\n" +
                   $"✨ 后来的故事，由你书写 ✨";
        }

        protected override System.Collections.IEnumerator PlayEndingSequence()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            if (slideshowImage != null && memoryImages != null && memoryImages.Length > 0)
            {
                slideshowImage.gameObject.SetActive(true);
            }

            yield return base.PlayEndingSequence();

            if (dustParticles != null)
            {
                dustParticles.Play();
            }

            if (slideshowImage != null && memoryImages != null)
            {
                StartCoroutine(SlideshowSequence());
            }
        }

        private System.Collections.IEnumerator SlideshowSequence()
        {
            while (isPlaying)
            {
                for (int i = 0; i < memoryImages.Length; i++)
                {
                    if (!isPlaying)
                        yield break;

                    slideshowImage.sprite = memoryImages[i];
                    
                    float elapsed = 0f;
                    float duration = 3f;
                    Color color = Color.white;
                    color.a = 0f;
                    slideshowImage.color = color;

                    while (elapsed < 1f)
                    {
                        elapsed += Time.deltaTime;
                        color.a = Mathf.Clamp01(elapsed);
                        slideshowImage.color = color;
                        yield return null;
                    }

                    yield return new WaitForSeconds(2f);

                    elapsed = 0f;
                    while (elapsed < 1f)
                    {
                        elapsed += Time.deltaTime;
                        color.a = 1f - Mathf.Clamp01(elapsed);
                        slideshowImage.color = color;
                        yield return null;
                    }
                }
            }
        }

        public override void PlayEnding()
        {
            isPlaying = true;
            base.PlayEnding();
        }
    }
}
