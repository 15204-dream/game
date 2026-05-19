using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Male
{
    [RequireComponent(typeof(Animator))]
    public class YanShen : MaleCharacter
    {
        [Header("言深专属属性")]
        [SerializeField] private float mysteryLevel = 90f;
        [SerializeField] private float schemingLevel = 85f;
        [SerializeField] private float hiddenWarmthLevel = 30f;
        [SerializeField] private bool isRevealingSecret = false;
        [SerializeField] private List<string> mysteriousDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "yanshen";
            chineseName = "傅言深";
            nickname = "言深";
            maleType = MaleCharacterType.神秘腹黑;
        }

        protected override void InitializeMaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.神秘,
                CharacterPersonalityTag.腹黑,
                CharacterPersonalityTag.高冷
            };

            appearanceDescription = "眼神深邃而危险，举止神秘莫测，嘴角常带着意味深长的微笑";

            playerInitialFavorability = 45f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("打探隐私", "绝不透露自己的秘密", 25f),
                new 雷区Info("虚伪", "最讨厌不真诚", 30f),
                new 雷区Info("愚蠢", "无法忍受智商低的人", 20f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("niannian", 65f, "天真的她让他想要保护"),
                new CpCompatibility("wanning", 80f, "聪明的她可以棋逢对手"),
                new CpCompatibility("zhuyi", 50f, "八卦精让他不耐烦"),
                new CpCompatibility("xingchen", 75f, "内敛的她和他有共鸣"),
                new CpCompatibility("youwei", 60f, "单纯的她让他想要逗弄"),
                new CpCompatibility("jiujiu", 85f, "强势的她让他感到挑战")
            };

            personalEndingLineName = "暗夜星辰";
            directorValueTag = "神秘男神";
            hiddenTraitDescription = "看似冷漠无情，其实一直在暗中守护";
            heartSignalDescription = "当他开始为你铺设未来的路时，说明你已是他的人";

            favoritePickupLines = new List<string>
            {
                "你很有趣",
                "我想看看你更多的表情",
                "跟我来，给你一个惊喜",
                "你相信我吗？"
            };

            rejectPickupLines = new List<string>
            {
                "无聊的人",
                "不要靠近我",
                "你的小心思我看穿了"
            };

            mysteriousDialogueOptions = new List<string>
            {
                "从见你第一面起，我就在布局",
                "你是我唯一的变数",
                "我愿意为你放下所有算计"
            };

            muscleScore = 60f;
            intelligenceScore = 95f;
            humorScore = 55f;
        }

        public override void Initialize()
        {
            base.Initialize();
            mysteryLevel = 90f;
            hiddenWarmthLevel = 30f;
            isRevealingSecret = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                hiddenWarmthLevel = Mathf.Min(100f, hiddenWarmthLevel + amount * 0.5f);
            }

            if (stats.favorability > 70f && !isRevealingSecret)
            {
                isRevealingSecret = true;
                OnSecretRevealed();
            }
        }

        protected virtual void OnSecretRevealed()
        {
            mysteryLevel = Mathf.Max(0f, mysteryLevel - 30f);
            ChangeState(CharacterState.Thinking);
            Debug.Log($"{nickname}开始向你展露真心");
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isRevealingSecret)
            {
                return mysteriousDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}意味深长地微笑着",
                $"{nickname}在暗处观察着一切",
                $"{nickname}漫不经心地把玩着手中的棋子",
                $"{nickname}深邃的眼眸中藏着秘密",
                $"{nickname}若有所思地注视着某个方向"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            if (isRevealingSecret)
            {
                string[] heartLines = {
                    $"{nickname}的眼神变得温柔而真挚",
                    $"他的心跳终于不再隐藏",
                    $"{nickname}的腹黑面具下是一颗真诚的心"
                };
                return heartLines[Random.Range(0, heartLines.Length)];
            }

            string[] mysteriousHeartLines = {
                $"{nickname}的心跳微微加速，但他不动声色",
                $"他的目光变得深邃而专注",
                $"他的手不自觉地握紧..."
            };
            return mysteriousHeartLines[Random.Range(0, mysteriousHeartLines.Length)];
        }

        public void PlayScheme()
        {
            if (stats.favorability > 50f)
            {
                ChangeState(CharacterState.Speaking);
                schemingLevel = Mathf.Min(100f, schemingLevel + 5f);
                Debug.Log($"{nickname}在暗中策划着什么...");
                ModifyFavorability(5f);
            }
        }

        public void RevealScheme()
        {
            if (stats.favorability > 65f)
            {
                ChangeState(CharacterState.Confident);
                Debug.Log($"{nickname}：一切都在我的计划之中——除了你");
                ModifyFavorability(15f);
                isRevealingSecret = true;
            }
        }

        public void ShowHiddenProtection()
        {
            if (stats.favorability > 75f)
            {
                hiddenWarmthLevel = Mathf.Min(100f, hiddenWarmthLevel + 20f);
                mysteryLevel = Mathf.Max(0f, mysteryLevel - 15f);
                Debug.Log($"{nickname}展现出了隐藏的温柔");
                ModifyFavorability(12f);
            }
        }

        public void MakeTeasingComment()
        {
            if (stats.favorability > 40f)
            {
                ChangeState(CharacterState.Happy);
                Debug.Log($"{nickname}意味深长地说：你的脸红了");
                ModifyFavorability(3f);
                schemingLevel = Mathf.Min(100f, schemingLevel + 2f);
            }
        }

        public override bool Check雷区Trigger(string triggerType)
        {
            bool triggered = base.Check雷区Trigger(triggerType);
            if (triggered)
            {
                ChangeState(CharacterState.Angry);
                mysteryLevel = Mathf.Min(100f, mysteryLevel + 10f);
                hiddenWarmthLevel = Mathf.Max(0f, hiddenWarmthLevel - 10f);
            }
            return triggered;
        }

        #region 属性访问器

        public float MysteryLevel => mysteryLevel;
        public float SchemingLevel => schemingLevel;
        public float HiddenWarmthLevel => hiddenWarmthLevel;
        public bool IsRevealingSecret => isRevealingSecret;

        #endregion
    }
}
