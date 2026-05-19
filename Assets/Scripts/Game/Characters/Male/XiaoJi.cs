using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Male
{
    [RequireComponent(typeof(Animator))]
    public class XiaoJi : MaleCharacter
    {
        [Header("小季专属属性")]
        [SerializeField] private float artisticLevel = 95f;
        [SerializeField] private float prideLevel = 80f;
        [SerializeField] private float tsundereLevel = 75f;
        [SerializeField] private bool isShowingTrueFeelings = false;
        [SerializeField] private List<string> artisticDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "xiaoji";
            chineseName = "季淮序";
            nickname = "小季";
            maleType = MaleCharacterType.艺术傲娇;
        }

        protected override void InitializeMaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.艺术,
                CharacterPersonalityTag.傲娇,
                CharacterPersonalityTag.自信
            };

            appearanceDescription = "穿着考究的文艺服饰，举止优雅独特，眼中有星辰大海般的梦想";

            playerInitialFavorability = 48f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("庸俗", "最讨厌俗气的东西", 25f),
                new 雷区Info("批评艺术", "无法接受对艺术的质疑", 30f),
                new 雷区Info("攀比", "讨厌世俗的比较", 20f),
                new 雷区Info("粗鲁", "追求优雅的生活方式", 25f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("niannian", 80f, "甜甜的女孩子很可爱"),
                new CpCompatibility("wanning", 65f, "理性的她可能不太理解艺术"),
                new CpCompatibility("zhuyi", 70f, "八卦精让他头疼"),
                new CpCompatibility("xingchen", 90f, "同样浪漫的灵魂伴侣"),
                new CpCompatibility("youwei", 75f, "单纯的她让他想要保护"),
                new CpCompatibility("jiujiu", 60f, "强势的她让他想要争锋相对")
            };

            personalEndingLineName = "灵魂共鸣";
            directorValueTag = "艺术系男神";
            hiddenTraitDescription = "表面骄傲，内心其实很渴望被理解";
            heartSignalDescription = "当他为你展示自己的作品时，说明你已经走进了他的心";

            favoritePickupLines = new List<string>
            {
                "你懂得欣赏艺术吗？",
                "这幅画送给你",
                "只有你能理解我的世界",
                "你的眼睛像星星一样美"
            };

            rejectPickupLines = new List<string>
            {
                "你根本不懂艺术",
                "无聊透顶",
                "俗不可耐"
            };

            artisticDialogueOptions = new List<string>
            {
                "你是我唯一的缪斯",
                "我想为你画一幅画",
                "只有你能懂我的浪漫"
            };

            muscleScore = 30f;
            intelligenceScore = 85f;
            humorScore = 45f;
        }

        public override void Initialize()
        {
            base.Initialize();
            artisticLevel = 95f;
            isShowingTrueFeelings = false;
            tsundereLevel = 75f;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount < 0)
            {
                tsundereLevel = Mathf.Min(100f, tsundereLevel + Mathf.Abs(amount) * 0.5f);
            }
            else
            {
                tsundereLevel = Mathf.Max(0f, tsundereLevel - amount * 0.3f);
            }

            if (stats.favorability > 70f && !isShowingTrueFeelings)
            {
                isShowingTrueFeelings = true;
                OnTrueFeelingsShown();
            }
        }

        protected virtual void OnTrueFeelingsShown()
        {
            tsundereLevel = 30f;
            Debug.Log($"{nickname}卸下了骄傲的面具");
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (isShowingTrueFeelings)
            {
                return artisticDialogueOptions;
            }

            List<string> tsundereLines = new List<string>
            {
                "哼，别以为我会理你",
                "我只是随便看看而已",
                "才不是因为你..."
            };

            if (Random.value > 0.5f)
            {
                return tsundereLines;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}专注地画着素描",
                $"{nickname}优雅地弹奏着钢琴",
                $"{nickname}深情地凝视着自己的作品",
                $"{nickname}漫不经心地翻阅着画册",
                $"{nickname}假装不在意地偷瞄你"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            if (isShowingTrueFeelings)
            {
                string[] heartLines = {
                    $"{nickname}的眼神中充满了柔情",
                    $"他的心跳为你而加速",
                    $"{nickname}想要拥抱你"
                };
                return heartLines[Random.Range(0, heartLines.Length)];
            }

            string[] tsundereHeartLines = {
                $"他假装没在看你，但你发现他的耳尖红了",
                $"{nickname}的心跳加速了，但他嘴硬说没有",
                $"他的画笔微微颤抖..."
            };
            return tsundereHeartLines[Random.Range(0, tsundereHeartLines.Length)];
        }

        public void CreatePortrait()
        {
            if (stats.favorability > 50f)
            {
                ChangeState(CharacterState.Speaking);
                Debug.Log($"{nickname}正在为你画一幅肖像画");
                ModifyFavorability(10f);
                artisticLevel = Mathf.Min(100f, artisticLevel + 5f);
            }
        }

        public void GiveArtwork()
        {
            if (stats.favorability > 60f)
            {
                Debug.Log($"{nickname}将自己最珍贵的画作送给了你");
                ModifyFavorability(15f);
                isShowingTrueFeelings = true;
            }
            else
            {
                Debug.Log($"{nickname}傲慢地说：我的作品不是谁都能得到的");
            }
        }

        public override bool Check雷区Trigger(string triggerType)
        {
            bool triggered = base.Check雷区Trigger(triggerType);
            if (triggered)
            {
                ChangeState(CharacterState.Angry);
                prideLevel = Mathf.Min(100f, prideLevel + 10f);
                tsundereLevel = Mathf.Min(100f, tsundereLevel + 15f);
            }
            return triggered;
        }

        #region 属性访问器

        public float ArtisticLevel => artisticLevel;
        public float PrideLevel => prideLevel;
        public float TsundereLevel => tsundereLevel;
        public bool IsShowingTrueFeelings => isShowingTrueFeelings;

        #endregion
    }
}
