using UnityEngine;
using System.Collections;

namespace HeartHook.Game.Ending.Director.RankEndings
{
    public class D_Rank_Ending : RankEndingBase
    {
        [Header("D级特效")]
        [SerializeField] private Color rankColor = new Color(1f, 0.7f, 0.5f);
        [SerializeField] private ParticleSystem warningParticles;
        [SerializeField] private Image vignetteImage;

        protected override void Start()
        {
            RankId = "D";
            RankName = "口碑滑铁卢";
            base.Start();

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.98f, 0.95f, 0.9f);
            }
        }

        protected override string GenerateStatsText(DirectorEndingResult result)
        {
            return $"【节目终期统计】\n\n" +
                   $"热度指数：{result.FinalHeat}\n" +
                   $"口碑评分：{result.FinalReputation}\n" +
                   $"观众人数：{result.FinalAudience:N0}\n\n" +
                   $"CP配对数：{result.CPCount}\n" +
                   $"话题数量：{result.TopicCount}\n" +
                   $"危机次数：{result.CrisisCount}";
        }

        protected override string GenerateAchievementText(DirectorEndingResult result)
        {
            return $"\n★ 评定结果 ★\n\n" +
                   $"【需要反思】\n" +
                   $"达成D级评定\n\n" +
                   $"这次的表现不太理想，\n" +
                   $"但失败是成功之母，总结经验再来！";
        }

        protected override Color GetRankColor()
        {
            return rankColor;
        }

        protected override IEnumerator PlayEndingSequence()
        {
            if (vignetteImage != null)
            {
                vignetteImage.gameObject.SetActive(true);
                Color color = Color.black;
                color.a = 0.3f;
                vignetteImage.color = color;
            }

            yield return base.PlayEndingSequence();

            if (warningParticles != null)
            {
                warningParticles.Play();
            }
        }
    }
}
