using UnityEngine;
using System.Collections;

namespace HeartHook.Game.Ending.Guest.MainEndings
{
    public class NE01_EndingHandler : MainEndingHandler
    {
        [Header("意难平特效")]
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private AudioClip sadSound;
        [SerializeField] private Light sceneLight;
        [SerializeField] private float lightIntensity = 0.5f;
        [SerializeField] private Color rainColor = new Color(0.7f, 0.8f, 1f);

        private GuestEndingResult currentResult;

        protected override void Start()
        {
            EndingId = "NE01";
            EndingName = "意难平";
            base.Start();
        }

        protected override void PrepareEnding(GuestEndingResult result)
        {
            currentResult = result;

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.6f, 0.65f, 0.75f);
            }

            if (sceneLight != null)
            {
                sceneLight.color = rainColor;
                sceneLight.intensity = lightIntensity;
            }

            if (rainParticles != null)
            {
                var main = rainParticles.main;
                main.startColor = rainColor;
                rainParticles.Play();
            }
        }

        protected override string GenerateStoryText(GuestEndingResult result)
        {
            var characterName = result.HighestCharacter ?? "TA";

            return $"录制结束的那晚，你们在阳台上聊了很久。\n\n" +
                   $"\"如果...早一点遇见你就好了。\" {characterName}轻声说道。\n\n" +
                   $"你没有回答，只是看着窗外的雨。\n\n" +
                   $"有些缘分，差的就是那么一点点。\n\n" +
                   $"你们交换了联系方式，约定做永远的朋友。\n\n" +
                   $"但你知道，有些话，永远说不出口了。\n\n" +
                   $"好感度：{result.MaxFavorability}\n" +
                   $"结局达成：◇ 意难平 ◇";
        }

        protected override IEnumerator PlayEndingSequence()
        {
            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.color = Color.black;
            }

            yield return base.PlayEndingSequence();

            if (rainParticles != null)
            {
                rainParticles.Stop();
            }
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
