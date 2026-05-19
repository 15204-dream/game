using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Male
{
    [RequireComponent(typeof(Animator))]
    public class ChenGe : MaleCharacter
    {
        [Header("琛哥专属属性")]
        [SerializeField] private float maturityLevel = 95f;
        [SerializeField] private float stabilityLevel = 90f;
        [SerializeField] private float careLevel = 85f;
        [SerializeField] private bool isShowingCaring = false;
        [SerializeField] private List<string> matureDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "chenge";
            chineseName = "陆景琛";
            nickname = "琛哥";
            maleType = MaleCharacterType.成熟稳重;
        }

        protected override void InitializeMaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.成熟,
                CharacterPersonalityTag.稳重,
                CharacterPersonalityTag.自信
            };

            appearanceDescription = "身着深色西装，气质沉稳内敛，举手投足间尽显成熟男人的魅力";

            playerInitialFavorability = 50f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("幼稚", "无法忍受幼稚行为", 20f),
                new 雷区Info("冲动", "讨厌冲动行事", 25f),
                new 雷区Info("不负责任", "最在意责任感", 30f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("niannian", 70f, "想要保护她的纯真"),
                new CpCompatibility("wanning", 90f, "成熟女性的完美搭配"),
                new CpCompatibility("zhuyi", 60f, "可能觉得她有些聒噪"),
                new CpCompatibility("xingchen", 75f, "可以给她安全感"),
                new CpCompatibility("youwei", 80f, "想要照顾她的温柔"),
                new CpCompatibility("jiujiu", 85f, "御姐和熟男很般配")
            };

            personalEndingLineName = "携手余生";
            directorValueTag = "成熟大叔";
            hiddenTraitDescription = "成熟的外表下，有一颗渴望被依赖的心";
            heartSignalDescription = "当他开始为你规划未来时，说明你是认真的";

            favoritePickupLines = new List<string>
            {
                "你不用逞强，有我在",
                "这件事交给我处理",
                "饿了吗？我带你去吃饭",
                "累了吧，休息一下"
            };

            rejectPickupLines = new List<string>
            {
                "你还是个孩子",
                "这样做太冲动了",
                "你需要更成熟一点"
            };

            matureDialogueOptions = new List<string>
            {
                "我想给你一个家",
                "我会照顾你一辈子的",
                "你愿意和我一起走下去吗"
            };

            muscleScore = 75f;
            intelligenceScore = 85f;
            humorScore = 50f;
        }

        public override void Initialize()
        {
            base.Initialize();
            maturityLevel = 95f;
            isShowingCaring = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (stats.favorability > 65f && !isShowingCaring)
            {
                isShowingCaring = true;
                OnCaringSideShown();
            }
        }

        protected virtual void OnCaringSideShown()
        {
            Debug.Log($"{nickname}展现出了体贴的一面");
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isShowingCaring && stats.favorability > 75f)
            {
                return matureDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}优雅地品着红酒",
                $"{nickname}正在处理工作邮件",
                $"{nickname}安静地翻阅着书籍",
                $"{nickname}眼神深邃地望向远方",
                $"{nickname}沉稳地与工作人员交谈"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的眼神中多了一丝温柔",
                $"他想要伸手抚摸你的头发",
                $"{nickname}的心跳微微加速",
                $"他的嘴角不自觉地上扬..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public void ProvideComfort()
        {
            ChangeState(CharacterState.Confident);
            ModifyFavorability(10f);
            careLevel = Mathf.Min(100f, careLevel + 5f);
            Debug.Log($"{nickname}温柔地安慰着你");
        }

        public void MakePromise()
        {
            if (stats.favorability > 70f)
            {
                ChangeState(CharacterState.Speaking);
                Debug.Log($"{nickname}认真地看着你说：我会保护你的");
                ModifyFavorability(15f);
            }
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "依赖":
                    ModifyFavorability(10f);
                    careLevel = Mathf.Min(100f, careLevel + 3f);
                    break;
                case "撒娇":
                    ModifyFavorability(8f);
                    maturityLevel = Mathf.Max(0f, maturityLevel - 2f);
                    break;
                case "成熟对话":
                    ModifyFavorability(12f);
                    stabilityLevel = Mathf.Min(100f, stabilityLevel + 5f);
                    break;
            }
        }

        #region 属性访问器

        public float MaturityLevel => maturityLevel;
        public float StabilityLevel => stabilityLevel;
        public float CareLevel => careLevel;
        public bool IsShowingCaring => isShowingCaring;

        #endregion
    }
}
