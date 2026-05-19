using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class YouWeiEnding : CharacterEndingHandler
    {
        [Header("幼薇专属配置")]
        [SerializeField] private Color characterColor = new Color(1f, 0.95f, 0.9f);
        [SerializeField] private string characterTrait = "活泼甜心";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_YouWei";
            CharacterId = "YouWei";
            CharacterName = "幼薇";
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
            return $"幼薇就像一颗小太阳，\n\n" +
                   $"走到哪里都是欢声笑语。\n\n" +
                   $"她的快乐会传染，\n\n" +
                   $"让你忍不住也跟着笑起来。\n\n" +
                   $"\"和你在一起的每一天，\"\n\n" +
                   $"\"都是我最开心的日子！\"\n\n" +
                   $"她拉着你的手转圈，裙摆飞扬。\n\n" +
                   $"原来幸福就是这么简单，\n\n" +
                   $"和喜欢的人在一起，做什么都开心。\n\n" +
                   $"【角色结局 · 幼薇】";
        }
    }
}
