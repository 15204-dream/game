using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class ZhouZhouEnding : CharacterEndingHandler
    {
        [Header("洲洲专属配置")]
        [SerializeField] private Color characterColor = new Color(0.75f, 0.85f, 1f);
        [SerializeField] private string characterTrait = "温柔学霸";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_ZhouZhou";
            CharacterId = "ZhouZhou";
            CharacterName = "洲洲";
            base.Start();

            if (backgroundImage != null)
            {
                backgroundImage.color = characterColor;
            }
        }

        protected override void PrepareCharacterData(GuestEndingResult result)
        {
            if (characterImage != null && characterSprite != null)
            {
                characterImage.sprite = characterSprite;
            }
        }

        protected override string GenerateStoryText(GuestEndingResult result)
        {
            return $"洲洲的世界里曾经只有书本和公式，\n\n" +
                   $"直到你闯入了他的生活。\n\n" +
                   $"他开始学着浪漫，学着惊喜，\n\n" +
                   $"笨拙却认真地为你准备每一个感动。\n\n" +
                   $"\"我不太会说情话...但是...\"\n\n" +
                   $"\"我愿意用余生来学习怎么爱你。\"\n\n" +
                   $"这份来自理科生的告白，\n\n" +
                   $"比任何情诗都要动人。\n\n" +
                   $"【角色结局 · 洲洲】";
        }
    }
}
