using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Manager
{
    public class CharacterManager : MonoBehaviour
    {
        private static CharacterManager instance;
        public static CharacterManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CharacterManager");
                    instance = go.AddComponent<CharacterManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("角色注册")]
        [SerializeField] private List<BaseCharacter> registeredCharacters = new List<BaseCharacter>();
        private Dictionary<string, BaseCharacter> characterDict = new Dictionary<string, BaseCharacter>();

        [Header("当前场景角色")]
        [SerializeField] private List<BaseCharacter> activeCharacters = new List<BaseCharacter>();
        [SerializeField] private BaseCharacter currentFocusCharacter;

        [Header("玩家数据")]
        [SerializeField] private string playerCharacterId = "player";
        [SerializeField] private float playerFavorability = 50f;

        public event Action<BaseCharacter> OnCharacterRegistered;
        public event Action<BaseCharacter> OnCharacterRemoved;
        public event Action<BaseCharacter, float> OnFavorabilityChanged;
        public event Action<BaseCharacter, CharacterState> OnCharacterStateChanged;

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeManager()
        {
            characterDict.Clear();
            registeredCharacters.Clear();
            activeCharacters.Clear();
        }

        public void RegisterCharacter(BaseCharacter character)
        {
            if (character == null) return;

            if (!characterDict.ContainsKey(character.CharacterId))
            {
                characterDict[character.CharacterId] = character;
                registeredCharacters.Add(character);

                OnCharacterRegistered?.Invoke(character);
                Debug.Log($"角色注册成功: {character.ChineseName} ({character.CharacterId})");
            }
        }

        public void UnregisterCharacter(string characterId)
        {
            if (characterDict.ContainsKey(characterId))
            {
                BaseCharacter character = characterDict[characterId];
                characterDict.Remove(characterId);
                registeredCharacters.Remove(character);
                activeCharacters.Remove(character);

                OnCharacterRemoved?.Invoke(character);
                Debug.Log($"角色注销成功: {characterId}");
            }
        }

        public BaseCharacter GetCharacter(string characterId)
        {
            if (characterDict.ContainsKey(characterId))
            {
                return characterDict[characterId];
            }
            return null;
        }

        public List<BaseCharacter> GetAllCharacters()
        {
            return new List<BaseCharacter>(registeredCharacters);
        }

        public List<BaseCharacter> GetCharactersByGender(CharacterGender gender)
        {
            List<BaseCharacter> result = new List<BaseCharacter>();
            foreach (var character in registeredCharacters)
            {
                if (character.Gender == gender)
                {
                    result.Add(character);
                }
            }
            return result;
        }

        public void AddActiveCharacter(BaseCharacter character)
        {
            if (character != null && !activeCharacters.Contains(character))
            {
                activeCharacters.Add(character);
            }
        }

        public void RemoveActiveCharacter(BaseCharacter character)
        {
            if (character != null)
            {
                activeCharacters.Remove(character);
            }
        }

        public void SetCurrentFocusCharacter(BaseCharacter character)
        {
            currentFocusCharacter = character;
        }

        public BaseCharacter GetCurrentFocusCharacter()
        {
            return currentFocusCharacter;
        }

        public List<BaseCharacter> GetActiveCharacters()
        {
            return new List<BaseCharacter>(activeCharacters);
        }

        public void ModifyFavorability(string characterId, float amount)
        {
            BaseCharacter character = GetCharacter(characterId);
            if (character != null)
            {
                float oldFavorability = character.Stats.favorability;
                character.ModifyFavorability(amount);
                float newFavorability = character.Stats.favorability;

                OnFavorabilityChanged?.Invoke(character, newFavorability - oldFavorability);
                Debug.Log($"{character.ChineseName} 好感度变化: {oldFavorability} -> {newFavorability}");
            }
        }

        public void InteractWithCharacter(string characterId, string interactionType)
        {
            BaseCharacter character = GetCharacter(characterId);
            if (character != null)
            {
                character.OnInteractionComplete(interactionType);
                character.RecordInteraction(playerCharacterId, interactionType);

                if (currentFocusCharacter == null || currentFocusCharacter.CharacterId != characterId)
                {
                    SetCurrentFocusCharacter(character);
                }
            }
        }

        public void ChangeCharacterState(string characterId, CharacterState newState, float duration = 0f)
        {
            BaseCharacter character = GetCharacter(characterId);
            if (character != null)
            {
                character.ChangeState(newState, duration);
                OnCharacterStateChanged?.Invoke(character, newState);
            }
        }

        public bool Check雷区Trigger(string characterId, string triggerType)
        {
            BaseCharacter character = GetCharacter(characterId);
            if (character != null)
            {
                return character.Check雷区Trigger(triggerType);
            }
            return false;
        }

        public float GetCompatibility(string characterId1, string characterId2)
        {
            BaseCharacter character1 = GetCharacter(characterId1);
            if (character1 != null)
            {
                return character1.GetCompatibilityWith(characterId2);
            }
            return 50f;
        }

        public List<CpCompatibility> GetTopCompatibleCharacters(string characterId, int count = 3)
        {
            BaseCharacter character = GetCharacter(characterId);
            if (character == null) return new List<CpCompatibility>();

            List<CpCompatibility> allCompatibility = new List<CpCompatibility>(character.Cp相性);
            allCompatibility.Sort((a, b) => b.compatibilityScore.CompareTo(a.compatibilityScore));

            List<CpCompatibility> topCompatibility = new List<CpCompatibility>();
            for (int i = 0; i < Mathf.Min(count, allCompatibility.Count); i++)
            {
                topCompatibility.Add(allCompatibility[i]);
            }
            return topCompatibility;
        }

        public Dictionary<string, float> GetAllFavorabilities()
        {
            Dictionary<string, float> favorabilities = new Dictionary<string, float>();
            foreach (var character in registeredCharacters)
            {
                favorabilities[character.CharacterId] = character.Stats.favorability;
            }
            return favorabilities;
        }

        public List<BaseCharacter> GetCharactersByPersonalityTag(CharacterPersonalityTag tag)
        {
            List<BaseCharacter> result = new List<BaseCharacter>();
            foreach (var character in registeredCharacters)
            {
                if (character.PersonalityTags.Contains(tag))
                {
                    result.Add(character);
                }
            }
            return result;
        }

        public void SaveCharacterData()
        {
            foreach (var character in registeredCharacters)
            {
                string json = character.ToJson();
                PlayerPrefs.SetString($"CharacterData_{character.CharacterId}", json);
            }
            PlayerPrefs.Save();
            Debug.Log("角色数据已保存");
        }

        public void LoadCharacterData()
        {
            foreach (var character in registeredCharacters)
            {
                if (PlayerPrefs.HasKey($"CharacterData_{character.CharacterId}"))
                {
                    string json = PlayerPrefs.GetString($"CharacterData_{character.CharacterId}");
                    character.FromJson(json);
                }
            }
            Debug.Log("角色数据已加载");
        }

        public void ResetAllCharacters()
        {
            foreach (var character in registeredCharacters)
            {
                character.Initialize();
            }
            Debug.Log("所有角色已重置");
        }

        public string GenerateRelationshipReport()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== 角色关系报告 ===");
            sb.AppendLine();

            foreach (var character in registeredCharacters)
            {
                sb.AppendLine($"【{character.ChineseName}】");
                sb.AppendLine($"  好感度: {character.Stats.favorability:F1}");
                sb.AppendLine($"  当前状态: {character.CurrentState}");
                sb.AppendLine($"  性格标签: {string.Join(", ", character.PersonalityTags)}");
                sb.AppendLine();

                sb.AppendLine("  CP相性:");
                foreach (var cp in character.Cp相性)
                {
                    BaseCharacter target = GetCharacter(cp.characterId);
                    if (target != null)
                    {
                        sb.AppendLine($"    - {target.ChineseName}: {cp.compatibilityScore:F1} ({cp.compatibilityReason})");
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
