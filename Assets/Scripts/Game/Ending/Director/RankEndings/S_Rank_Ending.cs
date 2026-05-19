using UnityEngine;
using System.Collections;

namespace HeartHook.Game.Ending.Director.RankEndings
{
    public class S_Rank_Ending : RankEndingBase
    {
        [Header("S级特效")]
        [SerializeField] private Color rankColor = new Color(1f, 0.9f, 0.3f);
        [SerializeField] private ParticleSystem goldenParticles;
        [SerializeField] private Light sceneLight;
        [SerializeField] private AudioClip legendaryBGM;

        protected override void Start()
        {
            RankId = "S";
            RankName = "现象级神作";
            base.Start();

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(1f, 0.95f, 0.8f);
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
            return $"\n★ 解锁成就 ★\n\n" +
                   $"【传说级制作人】\n" +
                   $"达成S级评定\n\n" +
                   $"【完美主义者】\n" +
                   $"全程无危机发生\n\n" +
                   $"恭喜！你已成为综艺界的传奇！";
        }

        protected override Color GetRankColor()
        {
            return rankColor;
        }

        protected override IEnumerator PlayEndingSequence()
        {
            if (sceneLight != null)
            {
                sceneLight.color = rankColor;
                sceneLight.intensity = 2f;
            }

            yield return base.PlayEndingSequence();

            if (goldenParticles != null)
            {
                goldenParticles.Play();
            }
        }
    }
}
