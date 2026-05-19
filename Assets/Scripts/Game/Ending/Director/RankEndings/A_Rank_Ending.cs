using UnityEngine;

namespace HeartHook.Game.Ending.Director.RankEndings
{
    public class A_Rank_Ending : RankEndingBase
    {
        [Header("A级特效")]
        [SerializeField] private Color rankColor = new Color(0.9f, 0.5f, 1f);
        [SerializeField] private ParticleSystem purpleParticles;
        [SerializeField] private AudioClip excellentBGM;

        protected override void Start()
        {
            RankId = "A";
            RankName = "年度爆款";
            base.Start();

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.95f, 0.9f, 1f);
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
                   $"【年度热门导演】\n" +
                   $"达成A级评定\n\n" +
                   $"恭喜！你的节目成为年度爆款！";
        }

        protected override Color GetRankColor()
        {
            return rankColor;
        }

        protected override UnityEngine.Coroutine StartCoroutine(UnityEngine.Coroutine routine)
        {
            if (purpleParticles != null)
            {
                purpleParticles.Play();
            }
            return base.StartCoroutine(routine);
        }
    }
}
