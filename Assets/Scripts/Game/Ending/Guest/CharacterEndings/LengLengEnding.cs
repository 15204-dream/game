using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class LengLengEnding : CharacterEndingHandler
    {
        [Header("冷冷专属配置")]
        [SerializeField] private Color characterColor = new Color(0.7f, 0.85f, 1f);
        [SerializeField] private string characterTrait = "高冷学长";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_LengLeng";
            CharacterId = "LengLeng";
            CharacterName = "冷冷";
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
            return $"冷冷总是独来独往，眼神里带着些许疏离。\n\n" +
                   $"但你知道，那只是他保护自己的方式。\n\n" +
                   $"在一次深夜的阳台偶遇后，\n\n" +
                   $"他第一次对你敞开了心扉。\n\n" +
                   $"\"谢谢你...愿意等我。\"\n\n" +
                   $"那一刻，冰山融化，心墙倒塌。\n\n" +
                   $"你牵住了他的手，感受到了久违的温度。\n\n" +
                   $"也许真正的爱情，就是愿意为一个人改变。\n\n" +
                   $"【角色结局 · 冷冷】";
        }
    }
}
