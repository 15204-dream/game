using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class SenSenEnding : CharacterEndingHandler
    {
        [Header("森森专属配置")]
        [SerializeField] private Color characterColor = new Color(0.8f, 1f, 0.8f);
        [SerializeField] private string characterTrait = "阳光少年";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_SenSen";
            CharacterId = "SenSen";
            CharacterName = "森森";
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
            return $"森森的笑容像是夏天的阳光，明媚而温暖。\n\n" +
                   $"他总是能在你最需要的时候出现，\n\n" +
                   $"用最简单的话语化解你的烦恼。\n\n" +
                   $"\"别担心，有我在呢！\"\n\n" +
                   $"这份毫无保留的善意，\n\n" +
                   $"最终化作了一颗跳动的心。\n\n" +
                   $"你们一起看日出，一起数星星，\n\n" +
                   $"在这个夏天，留下了最美好的回忆。\n\n" +
                   $"【角色结局 · 森森】";
        }
    }
}
