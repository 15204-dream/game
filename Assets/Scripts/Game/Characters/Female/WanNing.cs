using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Female
{
    [RequireComponent(typeof(Animator))]
    public class WanNing : FemaleCharacter
    {
        [Header("晚宁专属属性")]
        [SerializeField] private float intellectLevel = 95f;
        [SerializeField] private float efficiencyLevel = 90f;
        [SerializeField] private float workFocusLevel = 85f;
        [SerializeField] private bool isRelaxing = false;
        [SerializeField] private List<string> intellectualDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "wanning";
            chineseName = "姜晚宁";
            nickname = "晚宁";
            femaleType = FemaleCharacterType.知性干练;
        }

        protected override void InitializeFemaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.知性,
                CharacterPersonalityTag.干练,
                CharacterPersonalityTag.成熟
            };

            appearanceDescription = "身着干练的职业装，举止优雅得体，眼神中透着智慧与自信";

            playerInitialFavorability = 50f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("拖延", "最讨厌效率低下", 20f),
                new 雷区Info("情绪化", "欣赏理性处事", 25f),
                new 雷区Info("不专业", "对专业性要求很高", 25f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("lengleng", 80f, "两个理性的人可以深入交流"),
                new CpCompatibility("sensenn", 70f, "他可以让她放松"),
                new CpCompatibility("chenge", 90f, "成熟男女的完美组合"),
                new CpCompatibility("xiaoji", 65f, "艺术和理性可以互补"),
                new CpCompatibility("zhouzhou", 60f, "可能觉得有点不够聪明"),
                new CpCompatibility("yanshen", 85f, "聪明的她可以看穿他的心思")
            };

            personalEndingLineName = "并肩作战";
            directorValueTag = "职场精英";
            hiddenTraitDescription = "理性外表下，有一颗渴望被理解的心";
            heartSignalDescription = "当她开始向你倾诉工作时，说明她已经开始信任你";

            favoriteCompliments = new List<string>
            {
                "你真的很专业",
                "你的逻辑思维很清晰",
                "和你一起工作很高效",
                "你是我见过最聪明的人"
            };

            dislikedComments = new List<string>
            {
                "你怎么这么感情用事",
                "别那么矫情好吗",
                "你就不能理性一点吗"
            };

            intellectualDialogueOptions = new List<string>
            {
                "工作的事我都可以和你说",
                "我愿意和你分享我的想法",
                "谢谢你愿意听我倾诉"
            };

            eleganceScore = 90f;
            intelligenceScore = 95f;
            socialScore = 75f;
        }

        public override void Initialize()
        {
            base.Initialize();
            intellectLevel = 95f;
            isRelaxing = false;
            workFocusLevel = 85f;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (stats.favorability > 70f && !isRelaxing)
            {
                isRelaxing = true;
                OnRelaxMode();
            }
        }

        protected virtual void OnRelaxMode()
        {
            workFocusLevel = Mathf.Max(0f, workFocusLevel - 20f);
            ChangeState(CharacterState.Happy);
            Debug.Log($"{nickname}放下了工作模式，展现出柔软的一面");
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isRelaxing && stats.favorability > 75f)
            {
                return intellectualDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            if (isRelaxing)
            {
                string[] relaxLines = {
                    $"{nickname}正在泡一杯花草茶",
                    $"{nickname}放下电脑，静静地看着窗外",
                    $"{nickname}嘴角浮现出一丝温柔的微笑",
                    $"{nickname}难得地露出了放松的神情"
                };
                return relaxLines[Random.Range(0, relaxLines.Length)];
            }

            string[] workLines = {
                $"{nickname}专注地处理着工作邮件",
                $"{nickname}优雅地在平板上做着笔记",
                $"{nickname}有条不紊地安排日程",
                $"{nickname}用审视的目光分析着问题"
            };
            return workLines[Random.Range(0, workLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            if (isRelaxing)
            {
                string[] heartLines = {
                    $"{nickname}的眼神变得柔和",
                    $"她不自觉地靠近了你",
                    $"{nickname}的心跳微微加速",
                    $"她想要依靠在你身上..."
                };
                return heartLines[Random.Range(0, heartLines.Length)];
            }

            string[] workHeartLines = {
                $"{nickname}的视线在你身上停留了一秒",
                $"她努力压下心中的涟漪",
                $"{nickname}发现自己在想你..."
            };
            return workHeartLines[Random.Range(0, workHeartLines.Length)];
        }

        public void ShowWorkCompetence()
        {
            ChangeState(CharacterState.Speaking);
            intellectLevel = Mathf.Min(100f, intellectLevel + 5f);
            Debug.Log($"{nickname}展现出了专业的能力");
            ModifyFavorability(5f);
        }

        public void AcceptRelaxInvitation()
        {
            if (stats.favorability > 55f)
            {
                isRelaxing = true;
                OnRelaxMode();
                Debug.Log($"{nickname}答应了你的邀请");
                ModifyFavorability(10f);
            }
            else
            {
                Debug.Log($"{nickname}礼貌地说：现在还要工作，晚点再说吧");
            }
        }

        public void SeekWorkAdvice()
        {
            if (stats.favorability > 50f)
            {
                ChangeState(CharacterState.Thinking);
                Debug.Log($"{nickname}向你寻求建议");
                ModifyFavorability(8f);
            }
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "理性交流":
                    ModifyFavorability(12f);
                    intellectLevel = Mathf.Min(100f, intellectLevel + 3f);
                    break;
                case "体贴关心":
                    ModifyFavorability(10f);
                    isRelaxing = true;
                    break;
                case "帮助工作":
                    ModifyFavorability(8f);
                    efficiencyLevel = Mathf.Min(100f, efficiencyLevel + 5f);
                    break;
            }
        }

        #region 属性访问器

        public float IntellectLevel => intellectLevel;
        public float EfficiencyLevel => efficiencyLevel;
        public float WorkFocusLevel => workFocusLevel;
        public bool IsRelaxing => isRelaxing;

        #endregion
    }
}
