using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class XiaoJiEnding : CharacterEndingHandler
    {
        [Header("小季专属配置")]
        [SerializeField] private Color characterColor = new Color(1f, 0.9f, 0.85f);
        [SerializeField] private string characterTrait = "可爱萌系";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_XiaoJi";
            CharacterId = "XiaoJi";
            CharacterName = "小季";
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
            return $"小季是个话不多但很贴心的存在，\n\n" +
                   $"他总是默默记下你说的每一句话。\n\n" +
                   $"你喜欢吃什么，他记得；\n\n" +
                   $"你不经意说想要的礼物，他悄悄准备。\n\n" +
                   $"\"那个...这个给你。\"\n\n" +
                   $"他红着脸递过来的，是一份心意。\n\n" +
                   $"被这样笨拙却真诚地喜欢着，\n\n" +
                   $"原来是世界上最幸福的事。\n\n" +
                   $"【角色结局 · 小季】";
        }
    }
}
