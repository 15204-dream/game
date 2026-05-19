using UnityEngine;

namespace HeartHook.Game.Ending.Guest.MainEndings
{
    public class HE01_EndingHandler : MainEndingHandler
    {
        [Header("心动终章特效")]
        [SerializeField] private ParticleSystem heartParticles;
        [SerializeField] private AudioClip heartSound;
        [SerializeField] private Light sceneLight;
        [SerializeField] private float lightIntensity = 1.5f;

        private GuestEndingResult currentResult;

        protected override void Start()
        {
            EndingId = "HE01";
            EndingName = "心动终章";
            base.Start();
        }

        protected override void PrepareEnding(GuestEndingResult result)
        {
            currentResult = result;

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(1f, 0.9f, 0.95f);
            }

            if (sceneLight != null)
            {
                sceneLight.color = new Color(1f, 0.8f, 0.9f);
                sceneLight.intensity = lightIntensity;
            }

            if (heartParticles != null)
            {
                heartParticles.Play();
            }
        }

        protected override string GenerateStoryText(GuestEndingResult result)
        {
            var characterName = result.HighestCharacter ?? "TA";
            var favorability = result.MaxFavorability;

            return $"在恋综的最后一夜，{characterName}牵起了你的手。\n\n" +
                   $"月光洒落在你们身上，周围的摄像机都安静了下来。\n\n" +
                   $"这一刻，所有的聚光灯都不再重要。\n\n" +
                   $"因为此刻，你的心跳只为一个人加速。\n\n" +
                   $"好感度：{favorability}\n" +
                   $"结局达成：♥ 心动终章 ♥";
        }

        protected override GuestEndingResult GetCurrentResult()
        {
            return currentResult;
        }

        protected override UnityEngine.Coroutine StartCoroutine(UnityEngine.Coroutine routine)
        {
            return base.StartCoroutine(routine);
        }
    }
}
