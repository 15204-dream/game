using UnityEngine;

namespace HeartHook.Game.Ending.Guest.CharacterEndings
{
    public class WanNingEnding : CharacterEndingHandler
    {
        [Header("晚宁专属配置")]
        [SerializeField] private Color characterColor = new Color(0.9f, 0.85f, 1f);
        [SerializeField] private string characterTrait = "优雅御姐";
        [SerializeField] private Sprite characterSprite;

        protected override void Start()
        {
            EndingId = "CE_WanNing";
            CharacterId = "WanNing";
            CharacterName = "晚宁";
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
            return $"晚宁举手投足间都是优雅，\n\n" +
                   $"仿佛从画中走出的仙子。\n\n" +
                   $"但只有你知道，\n\n" +
                   $"她卸下精致妆容后的真实与可爱。\n\n" +
                   $"\"在你面前，我不需要完美。\"\n\n" +
                   $"她笑着依偎在你怀里，\n\n" +
                   $"那一刻，所有的光芒都黯淡下去，\n\n" +
                   $"因为她眼里只有你。\n\n" +
                   $"【角色结局 · 晚宁】";
        }
    }
}
