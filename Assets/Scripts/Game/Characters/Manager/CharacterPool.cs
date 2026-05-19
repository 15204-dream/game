using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveDashGame.Characters.Manager
{
    public class CharacterPool : MonoBehaviour
    {
        private static CharacterPool instance;
        public static CharacterPool Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CharacterPool");
                    instance = go.AddComponent<CharacterPool>();
                }
                return instance;
            }
        }

        [Serializable]
        public class PoolConfig
        {
            public string characterId;
            public int initialSize = 3;
            public int maxSize = 10;
            public bool allowExpand = true;
        }

        [Header("对象池配置")]
        [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();
        [SerializeField] private Transform poolParent;

        private Dictionary<string, Queue<BaseCharacter>> characterPools = new Dictionary<string, Queue<BaseCharacter>>();
        private Dictionary<string, PoolConfig> configDict = new Dictionary<string, PoolConfig>();
        private Dictionary<string, List<BaseCharacter>> allPooledCharacters = new Dictionary<string, List<BaseCharacter>>();

        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;

                if (poolParent == null)
                {
                    poolParent = transform;
                }

                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePools()
        {
            configDict.Clear();
            foreach (var config in poolConfigs)
            {
                configDict[config.characterId] = config;
                characterPools[config.characterId] = new Queue<BaseCharacter>();
                allPooledCharacters[config.characterId] = new List<BaseCharacter>();
            }
        }

        public BaseCharacter GetCharacter(string characterId, Vector3 position = default(Vector3), Transform parent = null)
        {
            if (!characterPools.ContainsKey(characterId))
            {
                Debug.LogWarning($"角色池不存在: {characterId}，将直接创建");
                return CharacterFactory.Instance.CreateCharacter(characterId, position, parent);
            }

            BaseCharacter character;

            if (characterPools[characterId].Count > 0)
            {
                character = characterPools[characterId].Dequeue();
                if (parent != null)
                {
                    character.transform.SetParent(parent);
                }
                character.transform.position = position;
                character.gameObject.SetActive(true);
                character.Initialize();
            }
            else
            {
                PoolConfig config = configDict[characterId];
                if (allPooledCharacters[characterId].Count < config.maxSize)
                {
                    character = CharacterFactory.Instance.CreateCharacter(characterId, position, parent);
                    if (character != null)
                    {
                        allPooledCharacters[characterId].Add(character);
                    }
                }
                else if (config.allowExpand)
                {
                    character = CharacterFactory.Instance.CreateCharacter(characterId, position, parent);
                    if (character != null)
                    {
                        allPooledCharacters[characterId].Add(character);
                    }
                }
                else
                {
                    Debug.LogWarning($"角色池已达到最大容量: {characterId}");
                    return null;
                }
            }

            if (CharacterManager.Instance != null)
            {
                CharacterManager.Instance.AddActiveCharacter(character);
            }

            return character;
        }

        public void ReturnCharacter(BaseCharacter character)
        {
            if (character == null) return;

            string characterId = character.CharacterId;

            if (!characterPools.ContainsKey(characterId))
            {
                Debug.LogWarning($"角色池不存在: {characterId}，将直接销毁");
                Destroy(character.gameObject);
                return;
            }

            if (CharacterManager.Instance != null)
            {
                CharacterManager.Instance.RemoveActiveCharacter(character);
            }

            character.gameObject.SetActive(false);
            character.transform.SetParent(poolParent);
            characterPools[characterId].Enqueue(character);

            Debug.Log($"角色已回收至对象池: {character.ChineseName}");
        }

        public void Prewarm(string characterId, int count)
        {
            if (!configDict.ContainsKey(characterId))
            {
                Debug.LogWarning($"角色池配置不存在: {characterId}");
                return;
            }

            PoolConfig config = configDict[characterId];
            int currentCount = allPooledCharacters[characterId].Count;
            int toCreate = Mathf.Min(count, config.maxSize) - currentCount;

            for (int i = 0; i < toCreate; i++)
            {
                BaseCharacter character = CharacterFactory.Instance.CreateCharacter(characterId);
                if (character != null)
                {
                    character.gameObject.SetActive(false);
                    character.transform.SetParent(poolParent);
                    characterPools[characterId].Enqueue(character);
                    allPooledCharacters[characterId].Add(character);
                }
            }

            Debug.Log($"角色池预热完成: {characterId}, 数量: {toCreate}");
        }

        public void PrewarmAll()
        {
            foreach (var config in poolConfigs)
            {
                Prewarm(config.characterId, config.initialSize);
            }
        }

        public int GetAvailableCount(string characterId)
        {
            if (characterPools.ContainsKey(characterId))
            {
                return characterPools[characterId].Count;
            }
            return 0;
        }

        public int GetTotalCount(string characterId)
        {
            if (allPooledCharacters.ContainsKey(characterId))
            {
                return allPooledCharacters[characterId].Count;
            }
            return 0;
        }

        public void ClearPool(string characterId)
        {
            if (!characterPools.ContainsKey(characterId)) return;

            while (characterPools[characterId].Count > 0)
            {
                BaseCharacter character = characterPools[characterId].Dequeue();
                if (character != null)
                {
                    Destroy(character.gameObject);
                }
            }

            foreach (var character in allPooledCharacters[characterId])
            {
                if (character != null)
                {
                    Destroy(character.gameObject);
                }
            }

            allPooledCharacters[characterId].Clear();
            Debug.Log($"角色池已清空: {characterId}");
        }

        public void ClearAllPools()
        {
            foreach (var characterId in characterPools.Keys)
            {
                ClearPool(characterId);
            }
            Debug.Log("所有角色池已清空");
        }

        public void SetPoolConfig(string characterId, int maxSize, bool allowExpand)
        {
            if (configDict.ContainsKey(characterId))
            {
                configDict[characterId].maxSize = maxSize;
                configDict[characterId].allowExpand = allowExpand;
            }
        }

        public PoolConfig GetPoolConfig(string characterId)
        {
            if (configDict.ContainsKey(characterId))
            {
                return configDict[characterId];
            }
            return null;
        }

        public Dictionary<string, int> GetPoolStatus()
        {
            Dictionary<string, int> status = new Dictionary<string, int>();
            foreach (var kvp in characterPools)
            {
                status[kvp.Key] = kvp.Value.Count;
            }
            return status;
        }

        private void OnDestroy()
        {
            ClearAllPools();
        }
    }
}
