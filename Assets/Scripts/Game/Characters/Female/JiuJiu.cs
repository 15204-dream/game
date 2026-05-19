using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Female
{
    [RequireComponent(typeof(Animator))]
    public class JiuJiu : FemaleCharacter
    {
        [Header("酒酒专属属性")]
        [SerializeField] private float coolLevel = 95f;
        [SerializeField] private float sassyLevel = 90f;
        [SerializeField] private float hiddenSoftnessLevel = 20f;
        [SerializeField] private bool isShowingSoftness = false;
        [SerializeField] private List<string> coolDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "jiujiu";
            chineseName = "温酒酒";
            nickname = "酒酒";
            femaleType = FemaleCharacterType.酷飒御姐;
        }

        protected override void InitializeFemaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.酷飒,
                CharacterPersonalityTag.御姐,
                CharacterPersonalityTag.自信
            };

            appearanceDescription = "御姐范十足，气场强大，烈焰红唇下藏着柔软的内心";

            playerInitialFavorability = 48f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("软弱", "最讨厌软弱的人", 25f),
                new 雷区Info("犹豫", "欣赏果断干脆", 20f),
                new 雷区Info("纠缠", "讨厌拖泥带水", 25f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("lengleng", 70f, "高冷对高冷很有火花"),
                new CpCompatibility("sensenn", 60f, "阳光男孩让她觉得有点幼稚"),
                new CpCompatibility("chenge", 85f, "成熟男女的完美搭配"),
                new CpCompatibility("xiaoji", 65f, "傲娇对上傲娇很有趣"),
                new CpCompatibility("zhouzhou", 75f, "自信的人互相欣赏"),
                new CpCompatibility("yanshen", 90f, "势均力敌的对手最迷人")
            };

            personalEndingLineName = "烈焰红妆";
            directorValueTag = "御姐担当";
            hiddenTraitDescription = "看似坚强，实则渴望被保护";
            heartSignalDescription = "当她开始向你撒娇时，说明你已经攻破了她的防线";

            favoriteCompliments = new List<string>
            {
                "你好飒啊",
                "你太有气场了",
                "你好酷，我喜欢",
                "你真的好厉害"
            };

            dislikedComments = new List<string>
            {
                "你怎么这么凶",
                "能不能温柔一点",
                "你一个女孩子..."
            };

            coolDialogueOptions = new List<string>
            {
                "你不是挺能的吗？怎么不说话了",
                "你成功引起了我的注意",
                "我只对你一个人温柔"
            };

            eleganceScore = 90f;
            intelligenceScore = 80f;
            socialScore = 70f;
        }

        public override void Initialize()
        {
            base.Initialize();
            coolLevel = 95f;
            hiddenSoftnessLevel = 20f;
            isShowingSoftness = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                hiddenSoftnessLevel = Mathf.Min(100f, hiddenSoftnessLevel + amount * 0.5f);
            }
            else
            {
                hiddenSoftnessLevel = Mathf.Max(0f, hiddenSoftnessLevel + amount * 0.3f);
            }

            if (hiddenSoftnessLevel > 60f && !isShowingSoftness)
            {
                isShowingSoftness = true;
                OnSoftSideShown();
            }
            else if (hiddenSoftnessLevel < 40f && isShowingSoftness)
            {
                isShowingSoftness = false;
            }
        }

        protected virtual void OnSoftSideShown()
        {
            coolLevel = Mathf.Max(0f, coolLevel - 20f);
            sassyLevel = Mathf.Max(0f, sassyLevel - 15f);
            ChangeState(CharacterState.Flustered);
            Debug.Log($"{nickname}的冷酷外壳出现了裂痕...");
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isShowingSoftness)
            {
                return coolDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            if (isShowingSoftness)
            {
                string[] softLines = {
                    $"{nickname}的目光变得柔和",
                    $"{nickname}难得地露出了笑容",
                    $"{nickname}安静地靠在墙边",
                    $"{nickname}的手指无意识地搅动着酒杯"
                };
                return softLines[Random.Range(0, softLines.Length)];
            }

            string[] coolLines = {
                $"{nickname}气场全开地走过",
                $"{nickname}漫不经心地整理着发型",
                $"{nickname}用审视的目光打量着周围",
                $"{nickname}优雅地品尝着红酒",
                $"{nickname}玩味地勾起了嘴角"
            };
            return coolLines[Random.Range(0, coolLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            if (isShowingSoftness)
            {
                string[] softHeartLines = {
                    $"{nickname}的心跳乱了节奏",
                    $"她努力压下心中的悸动",
                    $"{nickname}的眼神中多了一丝柔情",
                    $"她的防线正在崩塌..."
                };
                return softHeartLines[Random.Range(0, softHeartLines.Length)];
            }

            string[] coolHeartLines = {
                $"{nickname}的视线不自觉地飘向你",
                $"她的心跳漏了一拍",
                $"{nickname}握紧了手中的酒杯...",
                $"她假装没在看你，但耳朵红了"
            };
            return coolHeartLines[Random.Range(0, coolHeartLines.Length)];
        }

        public void ShowDominance()
        {
            if (stats.favorability > 40f)
            {
                ChangeState(CharacterState.Confident);
                sassyLevel = Mathf.Min(100f, sassyLevel + 5f);
                Debug.Log($"{nickname}霸气地说：你是我的");
                ModifyFavorability(8f);
            }
        }

        public void ShowVulnerability()
        {
            if (stats.favorability > 70f)
            {
                ChangeState(CharacterState.Sad);
                hiddenSoftnessLevel = Mathf.Min(100f, hiddenSoftnessLevel + 25f);
                coolLevel = Mathf.Max(0f, coolLevel - 15f);
                Debug.Log($"{nickname}卸下了所有伪装...");
                ModifyFavorability(15f);
                isShowingSoftness = true;
            }
        }

        public void GiveCompliment()
        {
            if (stats.favorability > 55f)
            {
                ChangeState(CharacterState.Happy);
                Debug.Log($"{nickname}嘴角上扬：还不错");
                ModifyFavorability(5f);
                hiddenSoftnessLevel = Mathf.Min(100f, hiddenSoftnessLevel + 3f);
            }
        }

        public void Challenge()
        {
            if (stats.favorability > 50f)
            {
                ChangeState(CharacterState.Speaking);
                Debug.Log($"{nickname}挑衅地说：敢不敢？");
                ModifyFavorability(5f);
            }
        }

        public override bool Check雷区Trigger(string triggerType)
        {
            bool triggered = base.Check雷区Trigger(triggerType);
            if (triggered)
            {
                ChangeState(CharacterState.Angry);
                coolLevel = Mathf.Min(100f, coolLevel + 10f);
                hiddenSoftnessLevel = Mathf.Max(0f, hiddenSoftnessLevel - 10f);
                isShowingSoftness = false;
            }
            return triggered;
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "勇敢面对":
                    ModifyFavorability(15f);
                    hiddenSoftnessLevel = Mathf.Min(100f, hiddenSoftnessLevel + 10f);
                    break;
                case "示弱":
                    ModifyFavorability(-5f);
                    coolLevel = Mathf.Min(100f, coolLevel + 5f);
                    break;
                case "调侃她":
                    ModifyFavorability(8f);
                    sassyLevel = Mathf.Min(100f, sassyLevel + 3f);
                    break;
            }
        }

        #region 属性访问器

        public float CoolLevel => coolLevel;
        public float SassyLevel => sassyLevel;
        public float HiddenSoftnessLevel => hiddenSoftnessLevel;
        public bool IsShowingSoftness => isShowingSoftness;

        #endregion
    }
}
