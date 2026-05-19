using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Manager
{
    public class CharacterFactory : MonoBehaviour
    {
        private static CharacterFactory instance;
        public static CharacterFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CharacterFactory");
                    instance = go.AddComponent<CharacterFactory>();
                }
                return instance;
            }
        }

        [Header("角色预设")]
        [SerializeField] private List<CharacterPreset> malePresets = new List<CharacterPreset>();
        [SerializeField] private List<CharacterPreset> femalePresets = new List<CharacterPreset>();

        [Serializable]
        public class CharacterPreset
        {
            public string characterId;
            public string prefabPath;
            public CharacterGender gender;
        }

        private Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public BaseCharacter CreateCharacter(string characterId, Vector3 position = default(Vector3), Transform parent = null)
        {
            GameObject prefab = GetCharacterPrefab(characterId);
            if (prefab == null)
            {
                Debug.LogError($"无法创建角色: {characterId}, 预制体未找到");
                return null;
            }

            GameObject charObj;
            if (parent != null)
            {
                charObj = Instantiate(prefab, position, Quaternion.identity, parent);
            }
            else
            {
                charObj = Instantiate(prefab, position, Quaternion.identity);
            }

            BaseCharacter character = charObj.GetComponent<BaseCharacter>();
            if (character == null)
            {
                character = charObj.AddComponent<MaleCharacter>();
                Debug.LogWarning($"角色 {characterId} 未找到 BaseCharacter 组件，已添加默认组件");
            }

            character.Initialize();

            if (CharacterManager.Instance != null)
            {
                CharacterManager.Instance.RegisterCharacter(character);
            }

            return character;
        }

        public BaseCharacter CreateCharacter(CharacterGender gender, Vector3 position = default(Vector3), Transform parent = null)
        {
            List<CharacterPreset> presets = gender == CharacterGender.Male ? malePresets : femalePresets;
            if (presets.Count == 0)
            {
                Debug.LogError($"没有找到 {gender} 角色的预设");
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, presets.Count);
            return CreateCharacter(presets[randomIndex].characterId, position, parent);
        }

        public List<BaseCharacter> CreateAllCharacters(Transform parent = null)
        {
            List<BaseCharacter> characters = new List<BaseCharacter>();

            foreach (var preset in malePresets)
            {
                BaseCharacter character = CreateCharacter(preset.characterId, default(Vector3), parent);
                if (character != null)
                {
                    characters.Add(character);
                }
            }

            foreach (var preset in femalePresets)
            {
                BaseCharacter character = CreateCharacter(preset.characterId, default(Vector3), parent);
                if (character != null)
                {
                    characters.Add(character);
                }
            }

            return characters;
        }

        public T CreateSpecificCharacter<T>(Vector3 position = default(Vector3), Transform parent = null) where T : BaseCharacter
        {
            GameObject charObj;
            if (parent != null)
            {
                charObj = Instantiate(new GameObject(typeof(T).Name), position, Quaternion.identity, parent);
            }
            else
            {
                charObj = Instantiate(new GameObject(typeof(T).Name), position, Quaternion.identity);
            }

            T character = charObj.AddComponent<T>();
            character.Initialize();

            if (CharacterManager.Instance != null)
            {
                CharacterManager.Instance.RegisterCharacter(character);
            }

            return character;
        }

        private GameObject GetCharacterPrefab(string characterId)
        {
            if (prefabCache.ContainsKey(characterId))
            {
                return prefabCache[characterId];
            }

            CharacterPreset preset = FindPreset(characterId);
            if (preset != null && !string.IsNullOrEmpty(preset.prefabPath))
            {
                GameObject prefab = Resources.Load<GameObject>(preset.prefabPath);
                if (prefab != null)
                {
                    prefabCache[characterId] = prefab;
                    return prefab;
                }
            }

            Debug.LogWarning($"未找到角色预制体: {characterId}，将创建空对象");
            return null;
        }

        private CharacterPreset FindPreset(string characterId)
        {
            foreach (var preset in malePresets)
            {
                if (preset.characterId == characterId)
                {
                    return preset;
                }
            }

            foreach (var preset in femalePresets)
            {
                if (preset.characterId == characterId)
                {
                    return preset;
                }
            }

            return null;
        }

        public void PreloadPrefabs()
        {
            foreach (var preset in malePresets)
            {
                if (!string.IsNullOrEmpty(preset.prefabPath))
                {
                    Resources.Load<GameObject>(preset.prefabPath);
                }
            }

            foreach (var preset in femalePresets)
            {
                if (!string.IsNullOrEmpty(preset.prefabPath))
                {
                    Resources.Load<GameObject>(preset.prefabPath);
                }
            }

            Debug.Log("角色预制体预加载完成");
        }

        public void ClearCache()
        {
            prefabCache.Clear();
        }

        public void AddPreset(CharacterPreset preset)
        {
            if (preset.gender == CharacterGender.Male)
            {
                if (!malePresets.Exists(p => p.characterId == preset.characterId))
                {
                    malePresets.Add(preset);
                }
            }
            else
            {
                if (!femalePresets.Exists(p => p.characterId == preset.characterId))
                {
                    femalePresets.Add(preset);
                }
            }
        }
    }
}
