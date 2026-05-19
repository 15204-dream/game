using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class ChenGeEnding : CharacterEndingHandler
    {
        [Header("琛哥专属配置")]
        [SerializeField] private Color characterColor = new Color(1f, 0.85f, 0.7f);
        [SerializeField] private string characterTrait = "暖男大哥";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_ChenGe";
            CharacterId = "ChenGe";
            CharacterName = "琛哥";
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
            return $"琛哥总是照顾着身边的每一个人，\n\n" +
                   $"却常常忽略了自己的感受。\n\n" +
                   $"你注意到了他眼底的疲惫，\n\n" +
                   $"于是你决定成为那个照顾他的人。\n\n" +
                   $"\"以后，让我来照顾你吧。\"\n\n" +
                   $"他愣了一瞬，然后笑了——\n\n" +
                   $"那是你见过最真挚的笑容。\n\n" +
                   $"原来被爱，也是一种幸福。\n\n" +
                   $"【角色结局 · 琛哥】";
        }
    }
}
