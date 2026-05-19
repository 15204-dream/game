using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters
{
    public enum MaleCharacterType
    {
        高冷禁欲,
        阳光暖男,
        成熟稳重,
        艺术傲娇,
        自信健身,
        神秘腹黑
    }

    [RequireComponent(typeof(Animator))]
    public abstract class MaleCharacter : BaseCharacter
    {
        [Header("男嘉宾专属")]
        [SerializeField] protected MaleCharacterType maleType;
        [SerializeField] protected List<string> favoritePickupLines = new List<string>();
        [SerializeField] protected List<string> rejectPickupLines = new List<string>();
        [SerializeField] protected float muscleScore = 50f;
        [SerializeField] protected float intelligenceScore = 50f;
        [SerializeField] protected float humorScore = 50f;

        protected Animator characterAnimator;

        protected override void Awake()
        {
            base.Awake();
            gender = CharacterGender.Male;
            characterAnimator = GetComponent<Animator>();
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeMaleSpecificData();
        }

        protected virtual void InitializeMaleSpecificData()
        {
        }

        public override List<string> GetCurrentDialogueOptions()
        {
            List<string> options = new List<string>();

            if (stats.favorability < 30)
            {
                options.AddRange(rejectPickupLines);
            }
            else if (stats.favorability > 70)
            {
                options.AddRange(favoritePickupLines);
            }
            else
            {
                options.Add("普通问候");
                options.Add("夸奖外表");
                options.Add("询问兴趣");
            }

            return options;
        }

        public override void ChangeState(CharacterState newState, float duration = 0f)
        {
            base.ChangeState(newState, duration);
            UpdateMaleAnimation(newState);
        }

        protected virtual void UpdateMaleAnimation(CharacterState state)
        {
            if (characterAnimator == null) return;

            characterAnimator.SetBool("isIdle", state == CharacterState.Idle);
            characterAnimator.SetBool("isSpeaking", state == CharacterState.Speaking);
            characterAnimator.SetBool("isListening", state == CharacterState.Listening);
            characterAnimator.SetBool("isFlattered", state == CharacterState.Happy);
            characterAnimator.SetBool("isAngry", state == CharacterState.Angry);
            characterAnimator.SetBool("isSurprised", state == CharacterState.Surprised);
            characterAnimator.SetBool("isFlustered", state == CharacterState.Flustered);
        }

        public override string GetRandomIdleLine()
        {
            string[] idleLines = {
                $"{nickname}靠在沙发上，目光若有所思",
                $"{nickname}轻轻整理了一下衣领",
                $"{nickname}漫不经心地看着窗外",
                $"{nickname}似乎在想些什么..."
            };
            return idleLines[Random.Range(0, idleLines.Length)];
        }

        public override string GetRandomHeartRateLine()
        {
            string[] heartLines = {
                $"{nickname}的心跳突然加速了...",
                $"{nickname}不自觉地握紧了手中的杯子",
                $"{nickname}的耳根有些微微发红",
                $"空气中弥漫着{nickname}心动的气息..."
            };
            return heartLines[Random.Range(0, heartLines.Length)];
        }

        public virtual bool TryImpress(string impressType, float impressValue)
        {
            switch (impressType)
            {
                case "幽默":
                    if (humorScore > 60)
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
                case "外表":
                    if (muscleScore > 60)
                    {
                        ModifyFavorability(impressValue * 1.2f);
                        return true;
                    }
                    break;
            }
            ModifyFavorability(impressValue * 0.8f);
            return false;
        }

        #region 属性访问器

        public MaleCharacterType MaleType => maleType;
        public float MuscleScore => muscleScore;
        public float IntelligenceScore => intelligenceScore;
        public float HumorScore => humorScore;

        #endregion
    }
}
