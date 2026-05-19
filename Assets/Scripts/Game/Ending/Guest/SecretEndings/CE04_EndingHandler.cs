using UnityEngine;
using System.Collections;

namespace HeartHook.Game.Ending.Guest.SecretEndings
{
    public class CE04_EndingHandler : SecretEndingHandlerBase
    {
        [Header("心动番外特效")]
        [SerializeField] private Color backgroundColor = new Color(1f, 0.9f, 1f);
        [SerializeField] private ParticleSystem sparkleParticles;
        [SerializeField] private AudioClip magicalSound;
        [SerializeField] private Image fourthWallBreak;
        [SerializeField] private TextMeshProUGUI fourthWallText;

        protected override void Start()
        {
            EndingId = "SE04";
            EndingName = "心动番外";
            base.Start();
        }

        protected override bool CheckConditionInternal()
        {
            if (GuestEndingSystem.Instance == null)
                return false;

            var unlocker = new EndingUnlocker();
            return unlocker.IsEventTriggered("Special_Confession") &&
                   unlocker.IsEventTriggered("Special_FourthWall");
        }

        protected override string GenerateStoryText()
        {
            return $"【番外篇 · 特别放送】\n\n" +
                   $"嗨，亲爱的玩家～\n\n" +
                   $"感谢你一路陪伴我们走到这里！\n\n" +
                   $"作为奖励，你解锁了这个隐藏结局。\n\n" +
                   $"你知道吗？\n\n" +
                   $"在屏幕的另一端，\n\n" +
                   $"我们也在为你创造这段故事。\n\n" +
                   $"希望这段旅程给你带来了快乐。\n\n" +
                   $"也许在另一个平行世界，\n\n" +
                   $"你真的在恋综里遇见了那些人。\n\n" +
                   $"无论如何——\n\n" +
                   $"愿你在现实中，也能遇到心动的人。\n\n" +
                   $"【彩蛋结局 · 心动番外】\n\n" +
                   $"★ 感谢游玩 ★\n\n" +
                   $"♥ 期待下次相遇 ♥";
        }

        protected override IEnumerator PlayEndingSequence()
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }

            yield return StartCoroutine(FourthWallBreakSequence());

            yield return base.PlayEndingSequence();

            if (sparkleParticles != null)
            {
                sparkleParticles.Play();
            }
        }

        private IEnumerator FourthWallBreakSequence()
        {
            if (fourthWallBreak != null)
            {
                fourthWallBreak.gameObject.SetActive(true);
                fourthWallBreak.color = Color.black;

                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    fourthWallBreak.color = Color.Lerp(Color.black, Color.white, elapsed);
                    yield return null;
                }
            }

            if (fourthWallText != null)
            {
                fourthWallText.gameObject.SetActive(true);
                fourthWallText.text = "恭喜你发现了隐藏内容！";
                fourthWallText.color = Color.white;

                yield return new WaitForSeconds(2f);

                elapsed = 0f;
                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    fourthWallText.color = Color.Lerp(Color.white, Color.clear, elapsed);
                    yield return null;
                }

                fourthWallText.gameObject.SetActive(false);
            }

            if (fourthWallBreak != null)
            {
                elapsed = 0f;
                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    fourthWallBreak.color = Color.Lerp(Color.white, Color.clear, elapsed);
                    yield return null;
                }

                fourthWallBreak.gameObject.SetActive(false);
            }
        }
    }
}
