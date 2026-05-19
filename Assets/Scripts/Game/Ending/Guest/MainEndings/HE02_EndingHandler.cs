using UnityEngine;

namespace HeartHook.Game.Ending.Guest.MainEndings
{
    public class HE02_EndingHandler : MainEndingHandler
    {
        [Header("双向奔赴特效")]
        [SerializeField] private ParticleSystem starParticles;
        [SerializeField] private AudioClip romanticSound;
        [SerializeField] private Transform cameraTarget;

        private GuestEndingResult currentResult;

        protected override void Start()
        {
            EndingId = "HE02";
            EndingName = "双向奔赴";
            base.Start();
        }

        protected override void PrepareEnding(GuestEndingResult result)
        {
            currentResult = result;

            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.95f, 0.95f, 1f);
            }

            if (starParticles != null)
            {
                starParticles.Play();
            }
        }

        protected override string GenerateStoryText(GuestEndingResult result)
        {
            var characterName = result.HighestCharacter ?? "TA";
            var tasks = result.CompletedTaskCount;

            return $"录制结束的那天，{characterName}在后台找到了你。\n\n" +
                   $"\"其实我一直想问你...\" TA有些紧张地开口。\n\n" +
                   $"\"我也很庆幸自己来了这里。\" 你笑着回答。\n\n" +
                   $"你们相视而笑，完成了节目组布置的最后一个任务——\n\n" +
                   $"「写下你对未来的期许」。\n\n" +
                   $"你在纸上写下了TA的名字。\n\n" +
                   $"完成任务数：{tasks}\n" +
                   $"结局达成：★ 双向奔赴 ★";
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
