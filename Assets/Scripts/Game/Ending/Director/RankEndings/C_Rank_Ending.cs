using UnityEngine;

namespace HeartHook.Game.Ending.Director.RankEndings
{
    public class C_Rank_Ending : RankEndingBase
    {
        [Header("C级特效")]
        [SerializeField] private Color rankColor = new Color(0.6f, 0.9f, 0.6f);
        [SerializeField] private ParticleSystem greenParticles;

        protected override void Start()
        {
            RankId = "C";
            RankName = "平稳收官";
            base.Start();

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.95f, 0.98f, 0.95f);
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
                   $"【稳定发挥】\n" +
                   $"达成C级评定\n\n" +
                   $"虽然不是特别出彩，\n" +
                   $"但节目平稳落地，继续加油！";
        }

        protected override Color GetRankColor()
        {
            return rankColor;
        }

        protected override UnityEngine.Coroutine StartCoroutine(UnityEngine.Coroutine routine)
        {
            if (greenParticles != null)
            {
                greenParticles.Play();
            }
            return base.StartCoroutine(routine);
        }
    }
}
