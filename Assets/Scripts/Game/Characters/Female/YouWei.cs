using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Female
{
    [RequireComponent(typeof(Animator))]
    public class YouWei : FemaleCharacter
    {
        [Header("幼薇专属属性")]
        [SerializeField] private float sweetnessLevel = 90f;
        [SerializeField] private float neighborlyLevel = 95f;
        [SerializeField] private float innocenceLevel = 80f;
        [SerializeField] private bool isCooking = false;
        [SerializeField] private List<string> caringDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "youwei";
            chineseName = "沈幼薇";
            nickname = "幼薇";
            femaleType = FemaleCharacterType.邻家甜美;
        }

        protected override void InitializeFemaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.邻家,
                CharacterPersonalityTag.甜美,
                CharacterPersonalityTag.温柔
            };

            appearanceDescription = "清新自然如邻家女孩，笑容温暖治愈，让人感到无比亲切";

            playerInitialFavorability = 55f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("争吵", "害怕冲突和矛盾", 20f),
                new 雷区Info("冷漠", "渴望被关心", 25f),
                new 雷区Info("挑剔", "受不了被苛责", 20f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("lengleng", 65f, "想要融化他的心"),
                new CpCompatibility("sensenn", 95f, "阳光男孩和她最配"),
                new CpCompatibility("chenge", 80f, "成熟男人让她有安全感"),
                new CpCompatibility("xiaoji", 75f, "艺术家的灵感来源"),
                new CpCompatibility("zhouzhou", 80f, "活力情侣组合"),
                new CpCompatibility("yanshen", 60f, "神秘感让她有点怕怕的")
            };

            personalEndingLineName = "温暖如你";
            directorValueTag = "治愈系女神";
            hiddenTraitDescription = "看似柔弱，实则内心坚强";
            heartSignalDescription = "当她开始为你做饭照顾你时，说明你已经成了她的家人";

            favoriteCompliments = new List<string>
            {
                "你好温柔啊",
                "你做的饭真好吃",
                "和你在一起很舒服",
                "你好贴心啊"
            };

            dislikedComments = new List<string>
            {
                "你怎么这么软弱",
                "能不能有点主见",
                "你太容易满足了"
            };

            caringDialogueOptions = new List<string>
            {
                "我给你做了便当哦",
                "你今天累了吧？我帮你捏捏肩",
                "我想一直照顾你"
            };

            eleganceScore = 70f;
            intelligenceScore = 70f;
            socialScore = 85f;
        }

        public override void Initialize()
        {
            base.Initialize();
            sweetnessLevel = 90f;
            neighborlyLevel = 95f;
            isCooking = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                sweetnessLevel = Mathf.Min(100f, sweetnessLevel + amount * 0.3f);
                neighborlyLevel = Mathf.Min(100f, neighborlyLevel + amount * 0.2f);
            }
        }

        public void StartCooking()
        {
            isCooking = true;
            ChangeState(CharacterState.Speaking);
            Debug.Log($"{nickname}系上围裙开始做饭");
        }

        public void StopCooking()
        {
            isCooking = false;
            ChangeState(CharacterState.Idle);
        }

        public void ServeMeal()
        {
            if (stats.favorability > 50f)
            {
                Debug.Log($"{nickname}端着热腾腾的饭菜：快尝尝好不好吃？");
                ModifyFavorability(10f);
                neighborlyLevel = Mathf.Min(100f, neighborlyLevel + 5f);
            }
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isCooking)
            {
                List<string> cookingLines = new List<string>
                {
                    "你想吃什么？我做给你",
                    "等着哦，马上就好",
                    "今天准备了惊喜哦"
                };
                return cookingLines;
            }

            if (stats.favorability > 75f)
            {
                return caringDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}正在整理房间",
                $"{nickname}温柔地浇着花",
                $"{nickname}在厨房忙碌着",
                $"{nickname}抱着抱枕窝在沙发里",
                $"{nickname}笑盈盈地看着你"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳加速，想要靠近你",
                $"她不自觉地整理了一下头发",
                $"{nickname}的目光充满了温柔",
                $"她想要牵住你的手..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public void OfferComfort()
        {
            if (stats.favorability > 45f)
            {
                ChangeState(CharacterState.Happy);
                Debug.Log($"{nickname}温柔地拍了拍旁边的位置：过来坐");
                ModifyFavorability(8f);
            }
        }

        public void MakePromise()
        {
            if (stats.favorability > 70f)
            {
                ChangeState(CharacterState.Speaking);
                Debug.Log($"{nickname}认真地说：无论发生什么，我都会在你身边");
                ModifyFavorability(15f);
                neighborlyLevel = Mathf.Min(100f, neighborlyLevel + 10f);
            }
        }

        public void ShowIndependence()
        {
            if (stats.favorability > 60f)
            {
                innocenceLevel = Mathf.Max(0f, innocenceLevel - 10f);
                Debug.Log($"{nickname}说：其实我也可以很独立的");
                ModifyFavorability(5f);
            }
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "接受照顾":
                    ModifyFavorability(12f);
                    neighborlyLevel = Mathf.Min(100f, neighborlyLevel + 3f);
                    break;
                case "表达感谢":
                    ModifyFavorability(8f);
                    sweetnessLevel = Mathf.Min(100f, sweetnessLevel + 2f);
                    break;
                case "伤害她":
                    ModifyFavorability(-15f);
                    innocenceLevel = Mathf.Max(0f, innocenceLevel - 10f);
                    break;
            }
        }

        #region 属性访问器

        public float SweetnessLevel => sweetnessLevel;
        public float NeighborlyLevel => neighborlyLevel;
        public float InnocenceLevel => innocenceLevel;
        public bool IsCooking => isCooking;

        #endregion
    }
}
