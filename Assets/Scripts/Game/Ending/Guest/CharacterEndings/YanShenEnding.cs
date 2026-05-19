using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class YanShenEnding : CharacterEndingHandler
    {
        [Header("言深专属配置")]
        [SerializeField] private Color characterColor = new Color(0.6f, 0.7f, 0.9f);
        [SerializeField] private string characterTrait = "神秘深沉";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_YanShen";
            CharacterId = "YanShen";
            CharacterName = "言深";
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
            return $"言深总是独坐在角落，\n\n" +
                   $"眼神深邃得让人捉摸不透。\n\n" +
                   $"你花了很多时间才走进他的世界，\n\n" +
                   $"才发现那里藏着不为人知的温柔。\n\n" +
                   $"\"你是第一个...让我想要倾诉的人。\"\n\n" +
                   $"他握住你的手，声音微微颤抖。\n\n" +
                   $"原来遇见对的人，\n\n" +
                   $"就是找到了愿意卸下防备的理由。\n\n" +
                   $"【角色结局 · 言深】";
        }
    }
}
