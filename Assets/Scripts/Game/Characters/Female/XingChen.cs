using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Female
{
    [RequireComponent(typeof(Animator))]
    public class XingChen : FemaleCharacter
    {
        [Header("星辰专属属性")]
        [SerializeField] private float romanceLevel = 95f;
        [SerializeField] private float introversionLevel = 85f;
        [SerializeField] private float imaginationLevel = 90f;
        [SerializeField] private bool isDaydreaming = false;
        [SerializeField] private List<string> romanticDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "xingchen";
            chineseName = "顾星辰";
            nickname = "星辰";
            femaleType = FemaleCharacterType.浪漫内敛;
        }

        protected override void InitializeFemaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.浪漫,
                CharacterPersonalityTag.内敛,
                CharacterPersonalityTag.艺术
            };

            appearanceDescription = "温柔的眼神中藏着星辰大海，举止轻柔，说话声音如微风拂面";

            playerInitialFavorability = 53f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("粗俗", "无法接受不浪漫的行为", 25f),
                new 雷区Info("吵闹", "喜欢安静的环境", 20f),
                new 雷区Info("现实", "讨厌太过功利的人", 30f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("lengleng", 85f, "高冷男神和她有相似的内敛"),
                new CpCompatibility("sensenn", 75f, "阳光可以照亮她的世界"),
                new CpCompatibility("chenge", 70f, "成熟男人可以给她安全感"),
                new CpCompatibility("xiaoji", 90f, "艺术家的灵魂伴侣"),
                new CpCompatibility("zhouzhou", 60f, "可能觉得太过直白"),
                new CpCompatibility("yanshen", 80f, "神秘感和浪漫相得益彰")
            };

            personalEndingLineName = "星语心愿";
            directorValueTag = "氛围感女神";
            hiddenTraitDescription = "内心世界丰富，渴望被读懂";
            heartSignalDescription = "当她为你写诗或创作时，说明你是她的灵感缪斯";

            favoriteCompliments = new List<string>
            {
                "你好浪漫啊",
                "你像星星一样闪耀",
                "你的灵魂好美",
                "我想一直这样看着你"
            };

            dislikedComments = new List<string>
            {
                "你想太多了",
                "能不能实际一点",
                "别那么文艺好吗"
            };

            romanticDialogueOptions = new List<string>
            {
                "我为你写了一首诗",
                "你是我的星星和月亮",
                "我想和你一起看星星"
            };

            eleganceScore = 85f;
            intelligenceScore = 80f;
            socialScore = 50f;
        }

        public override void Initialize()
        {
            base.Initialize();
            romanceLevel = 95f;
            introversionLevel = 85f;
            isDaydreaming = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                romanceLevel = Mathf.Min(100f, romanceLevel + amount * 0.4f);
            }

            if (stats.favorability > 70f)
            {
                introversionLevel = Mathf.Max(0f, introversionLevel - amount * 0.5f);
            }
        }

        public void StartDaydream()
        {
            isDaydreaming = true;
            imaginationLevel = Mathf.Min(100f, imaginationLevel + 5f);
            ChangeState(CharacterState.Thinking);
            Debug.Log($"{nickname}的眼神飘向了远方...");
        }

        public void StopDaydream()
        {
            isDaydreaming = false;
            if (currentState == CharacterState.Thinking)
            {
                ChangeState(CharacterState.Idle);
            }
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isDaydreaming)
            {
                List<string> daydreamLines = new List<string>
                {
                    "今晚的星星很美呢...",
                    "我在想一个故事",
                    "你愿意听我说吗？"
                };
                return daydreamLines;
            }

            if (stats.favorability > 75f)
            {
                return romanticDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}望着星空轻声哼唱",
                $"{nickname}在笔记本上写着什么",
                $"{nickname}安静地看着窗外的月亮",
                $"{nickname}的眼神深邃而遥远",
                $"{nickname}轻柔地翻动着书页"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳如星星闪烁般悸动",
                $"她的眼神中映出了你的身影",
                $"{nickname}害羞地低下了头",
                $"她想要牵住你的手..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public void SharePoem()
        {
            if (stats.favorability > 55f)
            {
                ChangeState(CharacterState.Speaking);
                Debug.Log($"{nickname}害羞地拿出一张纸：这是...我写给你的");
                ModifyFavorability(15f);
                romanceLevel = Mathf.Min(100f, romanceLevel + 10f);
            }
            else
            {
                Debug.Log($"{nickname}紧紧抱住笔记本：这是我的秘密...");
            }
        }

        public void StargazeTogether()
        {
            if (stats.favorability > 65f)
            {
                ChangeState(CharacterState.Happy);
                Debug.Log($"{nickname}温柔地说：和我一起看星星吧");
                ModifyFavorability(12f);
                introversionLevel = Mathf.Max(0f, introversionLevel - 15f);
            }
            else
            {
                Debug.Log($"{nickname}轻声说：我...还是喜欢一个人看星星");
            }
        }

        public void ShowVulnerability()
        {
            if (stats.favorability > 70f)
            {
                introversionLevel = Mathf.Max(0f, introversionLevel - 20f);
                ChangeState(CharacterState.Sad);
                Debug.Log($"{nickname}的眼眶微微泛红...");
                ModifyFavorability(10f);
            }
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "浪漫互动":
                    ModifyFavorability(15f);
                    romanceLevel = Mathf.Min(100f, romanceLevel + 5f);
                    break;
                case "静静陪伴":
                    ModifyFavorability(10f);
                    introversionLevel = Mathf.Max(0f, introversionLevel - 10f);
                    break;
                case "打破氛围":
                    ModifyFavorability(-10f);
                    romanceLevel = Mathf.Max(0f, romanceLevel - 15f);
                    break;
            }
        }

        #region 属性访问器

        public float RomanceLevel => romanceLevel;
        public float IntroversionLevel => introversionLevel;
        public float ImaginationLevel => imaginationLevel;
        public bool IsDaydreaming => isDaydreaming;

        #endregion
    }
}
