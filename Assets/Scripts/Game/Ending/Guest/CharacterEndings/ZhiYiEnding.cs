using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class ZhiYiEnding : CharacterEndingHandler
    {
        [Header("知意专属配置")]
        [SerializeField] private Color characterColor = new Color(0.85f, 1f, 0.9f);
        [SerializeField] private string characterTrait = "知性温柔";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_ZhiYi";
            CharacterId = "ZhiYi";
            CharacterName = "知意";
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
            return $"知意说话总是轻声细语，\n\n" +
                   $"像春风拂过心田。\n\n" +
                   $"她总能看穿你的小心思，\n\n" +
                   $"在你需要的时候给予最恰当的安慰。\n\n" +
                   $"\"我懂你的欲言又止。\"\n\n" +
                   $"这份心有灵犀的默契，\n\n" +
                   $"让你们之间无需太多言语，\n\n" +
                   $"一个眼神就能读懂彼此。\n\n" +
                   $"【角色结局 · 知意】";
        }
    }
}
