using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class XingChenEnding : CharacterEndingHandler
    {
        [Header("星辰专属配置")]
        [SerializeField] private Color characterColor = new Color(0.8f, 0.8f, 1f);
        [SerializeField] private string characterTrait = "追梦少年";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_XingChen";
            CharacterId = "XingChen";
            CharacterName = "星辰";
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
            return $"星辰总是仰望星空，\n\n" +
                   $"眼里装着大大的梦想。\n\n" +
                   $"他说想要环游世界，\n\n" +
                   $"而你说想要陪他一起看遍星辰大海。\n\n" +
                   $"\"那就...一起出发吧！\"\n\n" +
                   $"他的手紧紧握着你的手，\n\n" +
                   $"仿佛握住了一整个宇宙。\n\n" +
                   $"有梦想的人会发光，\n\n" +
                   $"而你们，就是彼此的光。\n\n" +
                   $"【角色结局 · 星辰】";
        }
    }
}
