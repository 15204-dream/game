using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class NianNianEnding : CharacterEndingHandler
    {
        [Header("念念专属配置")]
        [SerializeField] private Color characterColor = new Color(1f, 0.85f, 0.9f);
        [SerializeField] private string characterTrait = "文艺少女";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_NianNian";
            CharacterId = "NianNian";
            CharacterName = "念念";
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
            return $"念念喜欢写诗，喜欢做梦，\n\n" +
                   $"喜欢一切美好的事物。\n\n" +
                   $"而她最美好的梦，\n\n" +
                   $"是与你一起走过的每一个明天。\n\n" +
                   $"\"我想把我们故事，写成一本书。\"\n\n" +
                   $"她的眼睛里闪着星星般的光芒。\n\n" +
                   $"你们的故事才刚刚开始，\n\n" +
                   $"而结局，由你们一起书写。\n\n" +
                   $"【角色结局 · 念念】";
        }
    }
}
