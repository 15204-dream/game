using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters
{
    public enum FemaleCharacterType
    {
        甜美可爱,
        知性干练,
        活泼八卦,
        浪漫内敛,
        邻家甜美,
        酷飒御姐
    }

    [RequireComponent(typeof(Animator))]
    public abstract class FemaleCharacter : BaseCharacter
    {
        [Header("女嘉宾专属")]
        [SerializeField] protected FemaleCharacterType femaleType;
        [SerializeField] protected List<string> favoriteCompliments = new List<string>();
        [SerializeField] protected List<string> dislikedComments = new List<string>();
        [SerializeField] protected float eleganceScore = 50f;
        [SerializeField] protected float intelligenceScore = 50f;
        [SerializeField] protected float socialScore = 50f;
        [SerializeField] protected bool isWearingAccessory = false;
        [SerializeField] protected string accessoryDescription = "";

        protected Animator characterAnimator;

        protected override void Awake()
        {
            base.Awake();
            gender = CharacterGender.Female;
            characterAnimator = GetComponent<Animator>();
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeFemaleSpecificData();
        }

        protected virtual void InitializeFemaleSpecificData()
        {
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            List<string> options = new List<string>();

            if (stats.favorability < 30)
            {
                options.AddRange(dislikedComments);
            }
            else if (stats.favorability > 70)
            {
                options.AddRange(favoriteCompliments);
            }
            else
            {
                options.Add("礼貌问候");
                options.Add("称赞穿着");
                options.Add("聊兴趣爱好");
            }

            return options;
        }

        public override void ChangeState(CharacterState newState, float duration = 0f)
        {
            base.ChangeState(newState, duration);
            UpdateFemaleAnimation(newState);
        }

        protected virtual void UpdateFemaleAnimation(CharacterState state)
        {
            if (characterAnimator == null) return;

            characterAnimator.SetBool("isIdle", state == CharacterState.Idle);
            characterAnimator.SetBool("isSpeaking", state == CharacterState.Speaking);
            characterAnimator.SetBool("isListening", state == CharacterState.Listening);
            characterAnimator.SetBool("isHappy", state == CharacterState.Happy);
            characterAnimator.SetBool("isAngry", state == CharacterState.Angry);
            characterAnimator.SetBool("isSurprised", state == CharacterState.Surprised);
            characterAnimator.SetBool("isFlustered", state == CharacterState.Flustered);
            characterAnimator.SetBool("isConfident", state == CharacterState.Confident);
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}轻轻搅动着咖啡，若有所思",
                $"{nickname}正在整理自己的发型",
                $"{nickname}看着窗外的风景出神",
                $"{nickname}嘴角微微上扬，不知在想什么..."
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳突然加速了...",
                $"{nickname}不自觉地低下了头",
                $"{nickname}的脸颊泛起淡淡的红晕",
                $"空气中弥漫着{nickname}心动的气息..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public virtual bool TryImpress(string impressType, float impressValue)
        {
            switch (impressType)
            {
                case "优雅":
                    if (eleganceScore > 60)
                    {
                        ModifyFavorability(impressValue * 1.2f);
                        return true;
                    }
                    break;
                case "智慧":
                    if (intelligenceScore > 60)
                    {
                        ModifyFavorability(impressValue * 1.2f);
                        return true;
                    }
                    break;
                case "社交":
                    if (socialScore > 60)
                    {
                        ModifyFavorability(impressValue * 1.2f);
                        return true;
                    }
                    break;
            }
            ModifyFavorability(impressValue * 0.8f);
            return false;
        }

        public virtual void UpdateAccessory(bool hasAccessory, string description = "")
        {
            isWearingAccessory = hasAccessory;
            accessoryDescription = description;
        }

        #region 属性访问器

        public FemaleCharacterType FemaleType => femaleType;
        public float EleganceScore => eleganceScore;
        public float IntelligenceScore => intelligenceScore;
        public float SocialScore => socialScore;
        public bool IsWearingAccessory => isWearingAccessory;
        public string AccessoryDescription => accessoryDescription;

        #endregion
    }
}
