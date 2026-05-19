using UnityEngine;
using System.Collections;

namespace HeartHook.Game.Ending.Director.RankEndings
{
    public class E_Rank_Ending : RankEndingBase
    {
        [Header("E级特效")]
        [SerializeField] private Color rankColor = new Color(1f, 0.4f, 0.4f);
        [SerializeField] private ParticleSystem dangerParticles;
        [SerializeField] private AudioClip failureSound;
        [SerializeField] private Image staticImage;
        [SerializeField] private float staticIntensity = 0.3f;

        protected override void Start()
        {
            RankId = "E";
            RankName = "紧急停播";
            base.Start();

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.9f, 0.85f, 0.85f);
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
                   $"【紧急停播】\n" +
                   $"达成E级评定\n\n" +
                   $"节目遭遇重大危机，\n" +
                   $"不得不提前停播。\n\n" +
                   $"别灰心，这只是暂时的挫折，\n" +
                   $"总结教训，下次一定能做得更好！";
        }

        protected override Color GetRankColor()
        {
            return rankColor;
        }

        protected override IEnumerator PlayEndingSequence()
        {
            if (staticImage != null)
            {
                staticImage.gameObject.SetActive(true);
                StartCoroutine(StaticEffect());
            }

            yield return base.PlayEndingSequence();

            if (dangerParticles != null)
            {
                dangerParticles.Play();
            }
        }

        private IEnumerator StaticEffect()
        {
            if (staticImage == null)
                yield break;

            Color color = Color.white;
            color.a = 0f;
            staticImage.color = color;

            while (isPlaying)
            {
                float elapsed = 0f;
                while (elapsed < 0.1f)
                {
                    elapsed += Time.deltaTime;
                    color.a = UnityEngine.Random.Range(0f, staticIntensity);
                    staticImage.color = color;
                    yield return null;
                }
                yield return new WaitForSeconds(0.2f);
            }

            color.a = 0f;
            staticImage.color = color;
            staticImage.gameObject.SetActive(false);
        }
    }
}
