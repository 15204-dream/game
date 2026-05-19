using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Female
{
    [RequireComponent(typeof(Animator))]
    public class NianNian : FemaleCharacter
    {
        [Header("念念专属属性")]
        [SerializeField] private float sweetnessLevel = 95f;
        [SerializeField] private float innocenceLevel = 80f;
        [SerializeField] private float cutenessLevel = 90f;
        [SerializeField] private bool isBlushing = false;
        [SerializeField] private List<string> sweetDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "niannian";
            chineseName = "苏念念";
            nickname = "念念";
            femaleType = FemaleCharacterType.甜美可爱;
        }

        protected override void InitializeFemaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.甜美,
                CharacterPersonalityTag.可爱,
                CharacterPersonalityTag.活泼
            };

            appearanceDescription = "圆圆的脸蛋，水汪汪的大眼睛，说话软糯可爱，让人忍不住想要保护";

            playerInitialFavorability = 55f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("嘲讽", "无法接受被嘲笑", 25f),
                new 雷区Info("威胁", "害怕恐怖的氛围", 20f),
                new 雷区Info("冷漠", "受不了被忽视", 25f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("lengleng", 70f, "高冷男神和可爱女孩的最萌年龄差"),
                new CpCompatibility("sensenn", 90f, "两个阳光的人在一起更快乐"),
                new CpCompatibility("chenge", 75f, "被大叔宠爱的感觉真好"),
                new CpCompatibility("xiaoji", 80f, "艺术家的缪斯女神"),
                new CpCompatibility("zhouzhou", 85f, "阳光健身男和甜心女孩"),
                new CpCompatibility("yanshen", 65f, "神秘感让她好奇")
            };

            personalEndingLineName = "甜蜜邂逅";
            directorValueTag = "甜心担当";
            hiddenTraitDescription = "看似天真，内心其实很敏感细腻";
            heartSignalDescription = "当她开始主动找你说话时，说明你已经走进她心里了";

            favoriteCompliments = new List<string>
            {
                "你好可爱呀",
                "念念今天真漂亮",
                "想要保护你",
                "你的笑容真甜"
            };

            dislikedComments = new List<string>
            {
                "你怎么这么幼稚",
                "别那么天真好吗",
                "你懂什么呀"
            };

            sweetDialogueOptions = new List<string>
            {
                "我...我好像有点喜欢你",
                "你可以一直陪着我吗？",
                "遇到你真的很幸运呢"
            };

            eleganceScore = 60f;
            intelligenceScore = 65f;
            socialScore = 75f;
        }

        public override void Initialize()
        {
            base.Initialize();
            sweetnessLevel = 95f;
            innocenceLevel = 80f;
            isBlushing = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                sweetnessLevel = Mathf.Min(100f, sweetnessLevel + amount * 0.3f);
                cutenessLevel = Mathf.Min(100f, cutenessLevel + amount * 0.2f);
            }

            if (amount > 10f)
            {
                TriggerBlush();
            }
        }

        protected virtual void TriggerBlush()
        {
            isBlushing = true;
            ChangeState(CharacterState.Flustered);
            Debug.Log($"{nickname}的脸突然红了");
        }

        public void StopBlush()
        {
            isBlushing = false;
            if (currentState == CharacterState.Flustered)
            {
                ChangeState(CharacterState.Idle);
            }
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (stats.favorability > 75f)
            {
                return sweetDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}正在吃着小零食",
                $"{nickname}可爱地歪着头思考",
                $"{nickname}轻轻摇晃着双脚",
                $"{nickname}正在给玩偶整理衣服",
                $"{nickname}眼睛亮晶晶地看着你"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳加速，脸红到了耳根",
                $"她的目光变得羞羞的",
                $"{nickname}不自觉地靠近了你一点",
                $"她的双手紧张地握在一起..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public void DoCuteAction()
        {
            ChangeState(CharacterState.Happy);
            cutenessLevel = Mathf.Min(100f, cutenessLevel + 5f);
            Debug.Log($"{nickname}做了一连串可爱的动作");
        }

        public void AskForHug()
        {
            if (stats.favorability > 65f)
            {
                ChangeState(CharacterState.Flustered);
                Debug.Log($"{nickname}小声地问：可以抱抱吗？");
                ModifyFavorability(10f);
            }
            else
            {
                Debug.Log($"{nickname}委屈地说：你不爱我了吗...");
            }
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "夸奖":
                    ModifyFavorability(12f);
                    sweetnessLevel = Mathf.Min(100f, sweetnessLevel + 3f);
                    break;
                case "陪伴":
                    ModifyFavorability(8f);
                    innocenceLevel = Mathf.Min(100f, innocenceLevel + 2f);
                    break;
                case "欺负":
                    ModifyFavorability(-5f);
                    innocenceLevel = Mathf.Max(0f, innocenceLevel - 3f);
                    break;
            }
        }

        #region 属性访问器

        public float SweetnessLevel => sweetnessLevel;
        public float InnocenceLevel => innocenceLevel;
        public float CutenessLevel => cutenessLevel;
        public bool IsBlushing => isBlushing;

        #endregion
    }
}
