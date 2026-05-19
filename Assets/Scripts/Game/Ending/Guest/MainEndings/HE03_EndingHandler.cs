using UnityEngine;

namespace HeartHook.Game.Ending.Guest.MainEndings
{
    public class HE03_EndingHandler : MainEndingHandler
    {
        [Header("友情万岁特效")]
        [SerializeField] private ParticleSystem sparkleParticles;
        [SerializeField] private AudioClip friendshipSound;
        [SerializeField] private Sprite[] friendshipImages;

        private GuestEndingResult currentResult;
        private int imageIndex;

        protected override void Start()
        {
            EndingId = "HE03";
            EndingName = "友情万岁";
            base.Start();
        }

        protected override void PrepareEnding(GuestEndingResult result)
        {
            currentResult = result;
            imageIndex = 0;

            if (backgroundImage != null && friendshipImages != null && friendshipImages.Length > 0)
            {
                backgroundImage.sprite = friendshipImages[imageIndex];
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = Color.white;
            }

            if (sparkleParticles != null)
            {
                sparkleParticles.Play();
            }
        }

        protected override string GenerateStoryText(GuestEndingResult result)
        {
            return $"虽然在这个夏天，你没有找到爱情，\n\n" +
                   $"但你收获了最珍贵的友情。\n\n" +
                   $"你们一起笑过、闹过、分享过秘密，\n\n" +
                   $"这些回忆，比心动更加珍贵。\n\n" +
                   $"节目录制结束后，你们建立了专属的小群聊，\n\n" +
                   $"即使分开也要常常联系。\n\n" +
                   $"因为你们都知道——\n\n" +
                   $"有些缘分，不必是爱情。\n\n" +
                   $"结局达成：☆ 友情万岁 ☆";
        }

        protected override UnityEngine.Coroutine StartCoroutine(UnityEngine.Coroutine routine)
        {
            return base.StartCoroutine(routine);
        }

        protected override GuestEndingResult GetCurrentResult()
        {
            return currentResult;
        }

        protected override System.Collections.IEnumerator DisplayStory()
        {
            var baseCoroutine = base.DisplayStory();
            StartCoroutine(baseCoroutine);
            
            yield return new WaitForSeconds(2f);
            imageIndex = (imageIndex + 1) % (friendshipImages?.Length ?? 1);
            if (backgroundImage != null && friendshipImages != null && friendshipImages.Length > imageIndex)
            {
                backgroundImage.sprite = friendshipImages[imageIndex];
            }
        }
    }
}
