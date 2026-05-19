using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Male
{
    [RequireComponent(typeof(Animator))]
    public class SenSen : MaleCharacter
    {
        [Header("森森专属属性")]
        [SerializeField] private float sunshineLevel = 90f;
        [SerializeField] private float warmthLevel = 95f;
        [SerializeField] private bool isGivingHug = false;
        [SerializeField] private List<string> sunshineDialogueOptions = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            characterId = "sensenn";
            chineseName = "林屿森";
            nickname = "森森";
            maleType = MaleCharacterType.阳光暖男;
        }

        protected override void InitializeMaleSpecificData()
        {
            personalityTags = new List<CharacterPersonalityTag>
            {
                CharacterPersonalityTag.阳光,
                CharacterPersonalityTag.暖男,
                CharacterPersonalityTag.浪漫
            };

            appearanceDescription = "阳光帅气的邻家男孩，笑容温暖如春风，让人忍不住想要靠近";

            playerInitialFavorability = 55f;

            雷区列表 = new List<雷区Info>
            {
                new 雷区Info("悲伤情绪", "受不了别人难过", 15f),
                new 雷区Info("孤独", "讨厌孤独的氛围", 20f),
                new 雷区Info("冷漠", "无法接受冷漠对待", 25f)
            };

            cp相性列表 = new List<CpCompatibility>
            {
                new CpCompatibility("niannian", 90f, "两个阳光的人在一起更快乐"),
                new CpCompatibility("wanning", 70f, "可以照顾她偶尔的疲惫"),
                new CpCompatibility("zhuyi", 85f, "都是活泼性格很合拍"),
                new CpCompatibility("xingchen", 75f, "可以带给她更多快乐"),
                new CpCompatibility("youwei", 95f, "最理想的邻家组合"),
                new CpCompatibility("jiujiu", 60f, "可能跟不上她的节奏")
            };

            personalEndingLineName = "温暖港湾";
            directorValueTag = "阳光暖男";
            hiddenTraitDescription = "表面阳光，内心有时也会感到孤独";
            heartSignalDescription = "当他对你特别照顾时，说明你已经在他心里了";

            favoritePickupLines = new List<string>
            {
                "你今天看起来很开心呢",
                "有什么我可以帮忙的吗？",
                "这个送给你，希望你喜欢",
                "我可以叫你..."
            };

            rejectPickupLines = new List<string>
            {
                "你还好吗？发生什么事了？",
                "要不要出去透透气？",
                "我陪你聊聊吧"
            };

            sunshineDialogueOptions = new List<string>
            {
                "遇到你是我最幸运的事",
                "我想一直这样陪着你",
                "你的笑容是我最大的动力"
            };

            muscleScore = 65f;
            intelligenceScore = 70f;
            humorScore = 90f;
        }

        public override void Initialize()
        {
            base.Initialize();
            sunshineLevel = 90f;
            isGivingHug = false;
        }

        public override void ModifyFavorability(float amount)
        {
            base.ModifyFavorability(amount);

            if (amount > 0)
            {
                sunshineLevel = Mathf.Min(100f, sunshineLevel + amount * 0.3f);
            }
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            if (stats.favorability > 75f)
            {
                return sunshineDialogueOptions;
            }
            return base.GetCurrentDialogueOptions();
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}灿烂地笑着向你招手",
                $"{nickname}正在帮其他人倒水",
                $"{nickname}阳光地哼着小曲",
                $"{nickname}眼神温柔地看着周围",
                $"{nickname}细心整理着桌面"
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳加速，想要更靠近你",
                $"他的笑容变得更加灿烂了",
                $"{nickname}不自觉地想要牵起你的手",
                $"他的目光始终追随着你..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public void GiveHug()
        {
            isGivingHug = true;
            ChangeState(CharacterState.Happy);
            ModifyFavorability(5f);
            Debug.Log($"{nickname}温柔地抱住了你");
        }

        public void EndHug()
        {
            isGivingHug = false;
            ChangeState(CharacterState.Idle);
        }

        public override void OnInteractionComplete(string interactionType)
        {
            base.OnInteractionComplete(interactionType);

            switch (interactionType)
            {
                case "关心":
                    ModifyFavorability(8f);
                    sunshineLevel = Mathf.Min(100f, sunshineLevel + 3f);
                    break;
                case "陪伴":
                    ModifyFavorability(5f);
                    break;
                case "调侃":
                    ModifyFavorability(3f);
                    humorScore = Mathf.Min(100f, humorScore + 2f);
                    break;
            }
        }

        #region 属性访问器

        public float SunshineLevel => sunshineLevel;
        public float WarmthLevel => warmthLevel;
        public bool IsGivingHug => isGivingHug;

        #endregion
    }
}
