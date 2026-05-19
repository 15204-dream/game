using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Female
{
    [RequireComponent(typeof(Animator))]
    public class ZhiYi : FemaleCharacter
    {
        [Header("知意专属属性")]
        [SerializeField] private float gossipLevel = 95f;
        [SerializeField] private float socialLevel = 90f;
        [SerializeField] private float curiosityLevel = 85f;
        [SerializeField] private bool isInvestigating = false;
        [SerializeField] private List<string> gossipDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "zhiyi";
            chineseName = "夏知意";
            nickname = "知意";
            femaleType = FemaleCharacterType.活泼八卦;
        }

        protected override void InitializeFemaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.活泼,
                CharacterPersonalityTag.八卦,
                CharacterPersonalityTag.社交
            };

            appearanceDescription = "灵动的大眼睛，话多且信息量大，浑身散发着活泼的气息";

            playerInitialFavorability = 52f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("无聊", "无法忍受沉闷的气氛", 20f),
                new 雷区Info("隐瞒", "讨厌别人藏着掖着", 25f),
                new 雷区Info("独处", "最喜欢热闹的氛围", 15f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("lengleng", 50f, "高冷男让她很好奇"),
                new CpCompatibility("sensenn", 85f, "阳光暖男和她很合拍"),
                new CpCompatibility("chenge", 65f, "成熟男人有故事可挖"),
                new CpCompatibility("xiaoji", 70f, "艺术家有很多八卦素材"),
                new CpCompatibility("zhouzhou", 75f, "健身博主有很多话题"),
                new CpCompatibility("yanshen", 55f, "神秘男让她很感兴趣")
            };

            personalEndingLineName = "情报女王";
            directorValueTag = "八卦担当";
            hiddenTraitDescription = "看似八卦，实则是情报收集高手";
            heartSignalDescription = "当她开始只和你分享秘密时，说明你是她最信任的人";

            favoriteCompliments = new List<string>
            {
                "你的消息好灵通啊",
                "你怎么什么都知道",
                "和你聊天真有趣",
                "你的情报网真厉害"
            };

            dislikedComments = new List<string>
            {
                "你怎么这么爱打听",
                "能不能安静一点",
                "你管那么多干嘛"
            };

            gossipDialogueOptions = new List<string>
            {
                "我只告诉你一个人哦",
                "这件事我谁都没说过",
                "你是我最信任的人"
            };

            eleganceScore = 65f;
            intelligenceScore = 80f;
            socialScore = 95f;
        }

        public override void Initialize()
        {
            base.Initialize();
            gossipLevel = 95f;
            isInvestigating = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                socialLevel = Mathf.Min(100f, socialLevel + amount * 0.3f);
            }
        }

        public void StartInvestigating()
        {
            isInvestigating = true;
            curiosityLevel = Mathf.Min(100f, curiosityLevel + 5f);
            ChangeState(CharacterState.Surprised);
            Debug.Log($"{nickname}开始了她的情报收集模式");
        }

        public void StopInvestigating()
        {
            isInvestigating = false;
            if (currentState == CharacterState.Surprised)
            {
                ChangeState(CharacterState.Idle);
            }
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isInvestigating)
            {
                List<string> investigateLines = new List<string>
                {
                    "你知道吗？我听说...",
                    "这件事你绝对想不到",
                    "让我来告诉你一个小秘密"
                };
                return investigateLines;
            }

            if (stats.favorability > 75f)
            {
                return gossipDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}正在和旁边的人窃窃私语",
                $"{nickname}眼睛闪闪发光地听着八卦",
                $"{nickname}拿着手机不知道在看什么",
                $"{nickname}神秘兮兮地凑过来想说什么",
                $"{nickname}活力四射地蹦蹦跳跳"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳加速，想要靠近你",
                $"她的目光紧紧锁定在你身上",
                $"{nickname}激动地拉着你的手",
                $"她发现你比八卦更有趣..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public void ShareGossip()
        {
            if (stats.favorability > 45f)
            {
                ChangeState(CharacterState.Speaking);
                Debug.Log($"{nickname}兴奋地说：我告诉你一个秘密...");
                ModifyFavorability(3f);
                socialLevel = Mathf.Min(100f, socialLevel + 2f);
            }
        }

        public void KeepSecret()
        {
            if (stats.favorability > 60f)
            {
                Debug.Log($"{nickname}认真地说：这件事我只告诉你一个人");
                ModifyFavorability(8f);
                gossipDialogueOptions.Add("这个秘密我只告诉你哦");
            }
        }

        public void ShowJealousy()
        {
            if (stats.favorability > 55f)
            {
                ChangeState(CharacterState.Angry);
                Debug.Log($"{nickname}不满地说：你怎么不理我！");
                ModifyFavorability(-3f);
            }
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "倾听八卦":
                    ModifyFavorability(10f);
                    socialLevel = Mathf.Min(100f, socialLevel + 3f);
                    break;
                case "分享秘密":
                    ModifyFavorability(12f);
                    gossipLevel = Mathf.Min(100f, gossipLevel + 5f);
                    break;
                case "忽略她":
                    ModifyFavorability(-8f);
                    socialLevel = Mathf.Max(0f, socialLevel - 5f);
                    break;
            }
        }

        #region 属性访问器

        public float GossipLevel => gossipLevel;
        public float SocialLevel => socialLevel;
        public float CuriosityLevel => curiosityLevel;
        public bool IsInvestigating => isInvestigating;

        #endregion
    }
}
