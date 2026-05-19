using UnityEngine;

namespace HeartHook.Game.Ending.Guest.SecretEndings
{
    public class CE01_EndingHandler : SecretEndingHandlerBase
    {
        [Header("全员心动特效")]
        [SerializeField] private Color backgroundColor = new Color(1f, 0.95f, 0.9f);
        [SerializeField] private ParticleSystem heartExplosion;
        [SerializeField] private AudioClip celebrationSound;

        protected override void Start()
        {
            EndingId = "SE01";
            EndingName = "全员心动";
            base.Start();
        }

        protected override bool CheckConditionInternal()
        {
            if (GuestEndingSystem.Instance == null)
                return false;

            var unlocker = new EndingUnlocker();
            var allFav = unlocker.GetAllFavorability();
            int countAbove80 = 0;

            foreach (var fav in allFav.Values)
            {
                if (fav >= 80)
                    countAbove80++;
            }

            return countAbove80 >= 4;
        }

        protected override string GenerateStoryText()
        {
            return $"这一天，小屋里弥漫着奇妙的气氛。\n\n" +
                   $"不只是你，所有人心底都泛起了涟漪。\n\n" +
                   $"冷冷的眼神不再冰冷，森森的笑容更加灿烂，\n\n" +
                   $"琛哥开始笨拙地学浪漫，洲洲的公式里写满了你的名字。\n\n" +
                   $"原来这就是传说中的——全员心动！\n\n" +
                   $"摄像机捕捉到了这一切，\n\n" +
                   $"这一期节目，将成为收视冠军。\n\n" +
                   $"但更重要的是，\n\n" +
                   $"每个人都找到了属于自己的那份心动。\n\n" +
                   $"【彩蛋结局 · 全员心动】\n\n" +
                   $"♥ 感谢你让这个夏天充满爱 ♥";
        }

        protected override System.Collections.IEnumerator PlayEndingSequence()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            yield return base.PlayEndingSequence();

            if (heartExplosion != null)
            {
                heartExplosion.Play();
            }
        }
    }
}
