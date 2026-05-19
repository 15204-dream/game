using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Male
{
    [RequireComponent(typeof(Animator))]
    public class ZhouZhou : MaleCharacter
    {
        [Header("洲洲专属属性")]
        [SerializeField] private float fitnessLevel = 95f;
        [SerializeField] private float confidenceLevel = 85f;
        [SerializeField] private float selfieLevel = 80f;
        [SerializeField] private bool isFlexing = false;
        [SerializeField] private List<string> fitnessDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "zhouzhou";
            chineseName = "沈逸洲";
            nickname = "洲洲";
            maleType = MaleCharacterType.自信健身;
        }

        protected override void InitializeMaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.自信,
                CharacterPersonalityTag.阳光,
                CharacterPersonalityTag.活泼
            };

            appearanceDescription = "身材健硕的阳光大男孩，自信满满，浑身散发着健康的气息";

            playerInitialFavorability = 52f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("身材羞辱", "绝对不能被说胖", 30f),
                new 雷区Info("不健身", "讨厌不自律的人", 15f),
                new 雷区Info("负能量", "无法接受消极情绪", 20f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("niannian", 85f, "可爱女孩让他想要保护"),
                new CpCompatibility("wanning", 65f, "干练女性可能太强势"),
                new CpCompatibility("zhuyi", 75f, "八卦精和他很聊得来"),
                new CpCompatibility("xingchen", 70f, "浪漫的她让他心动"),
                new CpCompatibility("youwei", 80f, "邻家女孩让他放松"),
                new CpCompatibility("jiujiu", 90f, "酷飒御姐和他的气场很合")
            };

            personalEndingLineName = "活力人生";
            directorValueTag = "健身博主";
            hiddenTraitDescription = "过度自信的外表下，其实很在意别人的看法";
            heartSignalDescription = "当他不再自拍而是关注你时，说明你比他的照片更重要";

            favoritePickupLines = new List<string>
            {
                "要不要一起健身？",
                "你今天状态很好啊",
                "来，和我一起做运动",
                "看我新学的动作"
            };

            rejectPickupLines = new List<string>
            {
                "健身才能保持好身材",
                "你应该多运动",
                "来，跟着我做"
            };

            fitnessDialogueOptions = new List<string>
            {
                "我想和你一起变好",
                "你是我最想保护的人",
                "我会为你变得更优秀"
            };

            muscleScore = 95f;
            intelligenceScore = 55f;
            humorScore = 75f;
        }

        public override void Initialize()
        {
            base.Initialize();
            fitnessLevel = 95f;
            isFlexing = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                confidenceLevel = Mathf.Min(100f, confidenceLevel + amount * 0.2f);
            }
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (stats.favorability > 75f)
            {
                return fitnessDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}正在做引体向上",
                $"{nickname}对着镜子整理发型",
                $"{nickname}自信地展示着手臂线条",
                $"{nickname}在社交媒体上发布动态",
                $"{nickname}正在补充蛋白质"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳加速了，汗水顺着脸颊滑落",
                $"他想要在你面前展示最好的自己",
                $"{nickname}的目光紧紧跟随着你",
                $"他突然觉得锻炼更有动力了..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public void StartFlex()
        {
            isFlexing = true;
            ChangeState(CharacterState.Confident);
            Debug.Log($"{nickname}自信地展示着身材");
        }

        public void StopFlex()
        {
            isFlexing = false;
            ChangeState(CharacterState.Idle);
        }

        public void TakeSelfie()
        {
            if (stats.favorability > 50f)
            {
                ChangeState(CharacterState.Happy);
                Debug.Log($"{nickname}想要和你一起自拍");
                ModifyFavorability(3f);
                selfieLevel = Mathf.Min(100f, selfieLevel + 2f);
            }
        }

        public void InviteToGym()
        {
            if (stats.favorability > 40f)
            {
                Debug.Log($"{nickname}邀请你一起去健身房");
                ModifyFavorability(5f);
                fitnessLevel = Mathf.Min(100f, fitnessLevel + 3f);
            }
        }

        public void ShowVulnerability()
        {
            if (stats.favorability > 70f)
            {
                confidenceLevel = Mathf.Max(0f, confidenceLevel - 20f);
                ChangeState(CharacterState.Thinking);
                Debug.Log($"{nickname}展现出了脆弱的一面");
                ModifyFavorability(10f);
            }
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "夸奖外表":
                    ModifyFavorability(12f);
                    confidenceLevel = Mathf.Min(100f, confidenceLevel + 5f);
                    break;
                case "一起健身":
                    ModifyFavorability(8f);
                    fitnessLevel = Mathf.Min(100f, fitnessLevel + 5f);
                    break;
                case "鼓励":
                    ModifyFavorability(10f);
                    confidenceLevel = Mathf.Max(0f, confidenceLevel - 10f);
                    break;
            }
        }

        #region 属性访问器

        public float FitnessLevel => fitnessLevel;
        public float ConfidenceLevel => confidenceLevel;
        public float SelfieLevel => selfieLevel;
        public bool IsFlexing => isFlexing;

        #endregion
    }
}
