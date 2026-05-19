using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Male
{
    [RequireComponent(typeof(Animator))]
    public class LengLeng : MaleCharacter
    {
        [Header("冷冷专属属性")]
        [SerializeField] private float coldnessLevel = 85f;
        [SerializeField] private float restraintLevel = 90f;
        [SerializeField] private bool isShowingWarmSide = false;
        [SerializeField] private List<string> warmDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "lengleng";
            chineseName = "顾北辰";
            nickname = "冷冷";
            maleType = MaleCharacterType.高冷禁欲;
        }

        protected override void InitializeMaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.高冷,
                CharacterPersonalityTag.神秘,
                CharacterPersonalityTag.腹黑
            };

            appearanceDescription = "身穿深灰色西装，镜片后的眼神冷淡而深邃，气质禁欲而疏离";

            playerInitialFavorability = 45f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("油腻", "不喜欢过于油腻的表达", 25f),
                new 雷区Info("八卦", "讨厌打听隐私", 20f),
                new 雷区Info("轻浮", "无法忍受轻浮的行为", 30f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("niannian", 70f, "甜甜的女孩子可以融化冰冷的心"),
                new CpCompatibility("wanning", 75f, "知性女性可以引起兴趣"),
                new CpCompatibility("zhuyi", 45f, "太过聒噪让他想逃离"),
                new CpCompatibility("xingchen", 80f, "内敛的性格产生共鸣"),
                new CpCompatibility("youwei", 65f, "邻家气质让他放松"),
                new CpCompatibility("jiujiu", 60f, "酷飒御姐旗鼓相当")
            };

            personalEndingLineName = "冰山融化";
            directorValueTag = "高冷系男神";
            hiddenTraitDescription = "表面冷漠，内心深处渴望被理解";
            heartSignalDescription = "当他开始主动找你说话，说明已经心动了";

            favoritePickupLines = new List<string>
            {
                "你今天很安静",
                "这本书你也喜欢？",
                "要不要一起走走"
            };

            rejectPickupLines = new List<string>
            {
                "无聊",
                "没兴趣",
                "离我远点"
            };

            warmDialogueOptions = new List<string>
            {
                "其实我并不像表面那么冷漠",
                "只有在你面前才会这样",
                "你愿意了解更多吗"
            };

            muscleScore = 40f;
            intelligenceScore = 95f;
            humorScore = 30f;
        }

        public override void Initialize()
        {
            base.Initialize();
            coldnessLevel = 85f;
            isShowingWarmSide = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (stats.favorability > 70f && !isShowingWarmSide)
            {
                isShowingWarmSide = true;
                OnWarmSideShown();
            }
            else if (stats.favorability <= 60f && isShowingWarmSide)
            {
                isShowingWarmSide = false;
            }
        }

        protected virtual void OnWarmSideShown()
        {
            ChangeState(CharacterState.Surprised);
            Debug.Log($"{nickname}的冰冷外壳出现了裂痕...");
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isShowingWarmSide && stats.favorability > 75f)
            {
                return warmDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            if (isShowingWarmSide)
            {
                string[] warmLines = {
                    $"{nickname}偷偷看了你一眼",
                    $"{nickname}嘴角微微上扬",
                    $"{nickname}的眼神变得柔和了一些"
                };
                return warmLines[Random.Range(0, warmLines.Length)];
            }

            string[] coldLines = {
                $"{nickname}面无表情地看着窗外",
                $"{nickname}轻轻推了推眼镜",
                $"{nickname}若有所思地喝着黑咖啡",
                $"{nickname}冷淡地扫视了一圈房间"
            };
            return coldLines[Random.Range(0, coldLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            if (isShowingWarmSide)
            {
                string[] heartLines = {
                    $"{nickname}的心跳加速了，但他依然保持沉默",
                    $"他的耳根微微泛红，暴露了内心的波动",
                    $"{nickname}握笔的手微微颤抖"
                };
                return heartLines[Random.Range(0, heartLines.Length)];
            }

            return base.GetRandomHeartRateLine();
        }

        public override bool Check雷区Trigger(string triggerType)
        {
            bool triggered = base.Check雷区Trigger(triggerType);
            if (triggered)
            {
                ChangeState(CharacterState.Angry);
                coldnessLevel = Mathf.Min(100f, coldnessLevel + 5f);
            }
            return triggered;
        }

        public void ForceShowWarmSide()
        {
            isShowingWarmSide = true;
            OnWarmSideShown();
        }

        #region 属性访问器

        public float ColdnessLevel => coldnessLevel;
        public float RestraintLevel => restraintLevel;
        public bool IsShowingWarmSide => isShowingWarmSide;

        #endregion
    }
}
