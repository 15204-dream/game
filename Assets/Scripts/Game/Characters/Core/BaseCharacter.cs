using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters
{
    public enum CharacterGender { Male, Female }

    public enum CharacterState
    {
        Idle,
        Speaking,
        Listening,
        Happy,
        Angry,
        Sad,
        Surprised,
        Thinking,
        Flustered,
        Confident,
        Nervous
    }

    public enum CharacterPersonalityTag
    {
        高冷, 阳光, 暖男, 成熟, 稳重, 艺术, 傲娇, 自信, 腹黑, 神秘,
        甜美, 可爱, 知性, 干练, 活泼, 八卦, 浪漫, 内敛, 邻家, 酷飒, 御姐
    }

    [Serializable]
    public class CharacterStats
    {
        [Range(0, 100)]
        public float favorability = 50f;
        [Range(0, 100)]
        public float charm = 50f;
        [Range(0, 100)]
        public float interactionInitiative = 50f;
        [Range(0, 100)]
        public float emotionalStability = 50f;

        public void Reset()
        {
            favorability = 50f;
            charm = 50f;
            interactionInitiative = 50f;
            emotionalStability = 50f;
        }

        public void ApplyModifier(float favMod, float charmMod, float initMod, float stableMod)
        {
            favorability = Mathf.Clamp(favorability + favMod, 0f, 100f);
            charm = Mathf.Clamp(charm + charmMod, 0f, 100f);
            interactionInitiative = Mathf.Clamp(interactionInitiative + initMod, 0f, 100f);
            emotionalStability = Mathf.Clamp(emotionalStability + stableMod, 0f, 100f);
        }
    }

    [Serializable]
    public class CharacterRelationship
    {
        public string characterId;
        public float relationshipValue;
        public bool isCP;
        public List<string> interactionHistory;

        public CharacterRelationship()
        {
            relationshipValue = 0f;
            isCP = false;
            interactionHistory = new List<string>();
        }

        public void AddInteraction(string interaction)
        {
            interactionHistory.Add(interaction);
            if (interactionHistory.Count > 20)
            {
                interactionHistory.RemoveAt(0);
            }
        }
    }

    [Serializable]
    public class雷区Info
    {
        public string triggerType;
        public string description;
        public float favorabilityPenalty;

        public 雷区Info(string type, string desc, float penalty)
        {
            triggerType = type;
            description = desc;
            favorabilityPenalty = penalty;
        }
    }

    [Serializable]
    public class CpCompatibility
    {
        public string characterId;
        public float compatibilityScore;
        public string compatibilityReason;

        public CpCompatibility(string id, float score, string reason)
        {
            characterId = id;
            compatibilityScore = score;
            compatibilityReason = reason;
        }
    }

    public abstract class BaseCharacter : MonoBehaviour
    {
        [Header("基础信息")]
        [SerializeField] protected string characterId;
        [SerializeField] protected string chineseName;
        [SerializeField] protected string nickname;
        [SerializeField] protected CharacterGender gender;
        [SerializeField] protected int age;
        [SerializeField] protected string occupation;
        [SerializeField] protected List<CharacterPersonalityTag> personalityTags = new List<CharacterPersonalityTag>();
        [SerializeField] protected string appearanceDescription;

        [Header("数值属性")]
        [SerializeField] protected CharacterStats stats = new CharacterStats();
        [SerializeField] protected float playerInitialFavorability = 50f;

        [Header("关系网络")]
        [SerializeField] protected List<雷区Info> 雷区列表 = new List<雷区Info>();
        [SerializeField] protected List<CpCompatibility> cp相性列表 = new List<CpCompatibility>();
        [SerializeField] protected Dictionary<string, CharacterRelationship> relationships = new Dictionary<string, CharacterRelationship>();

        [Header("剧情属性")]
        [SerializeField] protected string personalEndingLineName;
        [SerializeField] protected string directorValueTag;
        [SerializeField] protected string hiddenTraitDescription;
        [SerializeField] protected string heartSignalDescription;

        [Header("状态机")]
        [SerializeField] protected CharacterState currentState = CharacterState.Idle;
        [SerializeField] protected CharacterState previousState = CharacterState.Idle;
        [SerializeField] protected float stateTimer = 0f;

        protected virtual void Awake()
        {
            InitializeRelationships();
        }

        protected virtual void Update()
        {
            UpdateStateMachine();
        }

        protected virtual void InitializeRelationships()
        {
            relationships.Clear();
        }

        public virtual void Initialize()
        {
            stats.Reset();
            stats.favorability = playerInitialFavorability;
            currentState = CharacterState.Idle;
            relationships.Clear();
            InitializeRelationships();
        }

        protected virtual void UpdateStateMachine()
        {
            if (stateTimer > 0f)
            {
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    ReturnToIdleState();
                }
            }
        }

        protected virtual void ReturnToIdleState()
        {
            ChangeState(CharacterState.Idle);
        }

        public virtual void ChangeState(CharacterState newState, float duration = 0f)
        {
            if (currentState != newState)
            {
                previousState = currentState;
                currentState = newState;
                stateTimer = duration;
                OnStateChanged(newState);
            }
        }

        protected virtual void OnStateChanged(CharacterState newState)
        {
        }

        public virtual void ModifyFavorability(float amount)
        {
            stats.favorability = Mathf.Clamp(stats.favorability + amount, 0f, 100f);
        }

        public virtual bool Check雷区Trigger(string triggerType)
        {
            foreach (var 雷区 in 雷区列表)
            {
                if (雷区.triggerType == triggerType)
                {
                    ModifyFavorability(-雷区.favorabilityPenalty);
                    return true;
                }
            }
            return false;
        }

        public virtual float GetCompatibilityWith(string targetId)
        {
            foreach (var cp in cp相性列表)
            {
                if (cp.characterId == targetId)
                {
                    return cp.compatibilityScore;
                }
            }
            return 50f;
        }

        public virtual void RecordInteraction(string targetId, string interaction)
        {
            if (!relationships.ContainsKey(targetId))
            {
                relationships[targetId] = new CharacterRelationship { characterId = targetId };
            }
            relationships[targetId].AddInteraction(interaction);
        }

        public virtual string GetHeartSignal()
        {
            return heartSignalDescription;
        }

        public virtual List<string> GetCurrentDialogueOptions()
        {
            return new List<string>();
        }

        public virtual void OnInteractionComplete(string interactionType)
        {
        }

        public virtual string GetRandomIdleLine()
        {
            return $"{nickname}正在发呆...";
        }

        public virtual string GetRandomHeartRateLine()
        {
            return $"{nickname}心跳加速了...";
        }

        #region 属性访问器

        public string CharacterId => characterId;
        public string ChineseName => chineseName;
        public string Nickname => nickname;
        public CharacterGender Gender => gender;
        public int Age => age;
        public string Occupation => occupation;
        public List<CharacterPersonalityTag> PersonalityTags => personalityTags;
        public string AppearanceDescription => appearanceDescription;
        public CharacterStats Stats => stats;
        public CharacterState CurrentState => currentState;
        public CharacterState PreviousState => previousState;
        public string PersonalEndingLineName => personalEndingLineName;
        public string DirectorValueTag => directorValueTag;
        public string HiddenTraitDescription => hiddenTraitDescription;
        public List<雷区Info> 雷区 => 雷区列表;
        public List<CpCompatibility> Cp相性 => cp相性列表;
        public Dictionary<string, CharacterRelationship> Relationships => relationships;

        #endregion

        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public virtual void FromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}
