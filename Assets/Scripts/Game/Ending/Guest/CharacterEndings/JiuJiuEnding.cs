using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class JiuJiuEnding : CharacterEndingHandler
    {
        [Header("酒酒专属配置")]
        [SerializeField] private Color characterColor = new Color(0.9f, 0.7f, 0.85f);
        [SerializeField] private string characterTrait = "微醺女神";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_JiuJiu";
            CharacterId = "JiuJiu";
            CharacterName = "酒酒";
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
            return $"酒酒总是带着淡淡的酒香，\n\n" +
                   $"微醺的眼眸里藏着故事。\n\n" +
                   $"她说自己习惯了一人饮酒，\n\n" +
                   $"但那晚，她愿意与你共饮。\n\n" +
                   $"\"这杯酒，我想敬你。\"\n\n" +
                   $"杯盏交错间，情愫暗生。\n\n" +
                   $"也许酒精只是借口，\n\n" +
                   $"真正让人沉醉的，从来都是心动。\n\n" +
                   $"【角色结局 · 酒酒】";
        }
    }
}
