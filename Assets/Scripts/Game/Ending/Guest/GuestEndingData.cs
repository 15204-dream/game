using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartHook.Game.Ending.Guest
{
    [CreateAssetMenu(fileName = "GuestEndingData", menuName = "HeartHook/嘉宾结局数据")]
    public class GuestEndingData : ScriptableObject
    {
        [Header("主结局配置")]
        public MainEndingConfig[] mainEndings;

        [Header("角色结局配置")]
        public CharacterEndingConfig[] characterEndings;

        [Header("彩蛋结局配置")]
        public SecretEndingConfig[] secretEndings;

        [Header("全局配置")]
        public int MaxFavorability = 100;
        public int MinFavorability = 0;
        public float EndingUnlockDelay = 2f;

        public MainEndingConfig GetMainEnding(string endingId)
        {
            if (mainEndings == null)
                return null;

            foreach (var config in mainEndings)
            {
                if (config != null && config.EndingId == endingId)
                    return config;
            }
            return null;
        }

        public CharacterEndingConfig GetCharacterEnding(string characterId)
        {
            if (characterEndings == null)
                return null;

            foreach (var config in characterEndings)
            {
                if (config != null && config.CharacterId == characterId)
                    return config;
            }
            return null;
        }

        public SecretEndingConfig GetSecretEnding(string endingId)
        {
            if (secretEndings == null)
                return null;

            foreach (var config in secretEndings)
            {
                if (config != null && config.EndingId == endingId)
                    return config;
            }
            return null;
        }

        public string[] GetAllMainEndingIds()
        {
            if (mainEndings == null)
                return Array.Empty<string>();

            var ids = new List<string>();
            foreach (var config in mainEndings)
            {
                if (config != null && !string.IsNullOrEmpty(config.EndingId))
                {
                    ids.Add(config.EndingId);
                }
            }
            return ids.ToArray();
        }

        public string[] GetAllCharacterIds()
        {
            if (characterEndings == null)
                return Array.Empty<string>();

            var ids = new List<string>();
            foreach (var config in characterEndings)
            {
                if (config != null && !string.IsNullOrEmpty(config.CharacterId))
                {
                    ids.Add(config.CharacterId);
                }
            }
            return ids.ToArray();
        }
    }

    [Serializable]
    public class MainEndingConfig
    {
        public string EndingId;
        public string EndingName;
        public string Description;
        public Sprite Background;
        public Sprite TitleImage;
        public AudioClip BGM;
        public Color TitleColor = Color.white;
        public EndingDisplayConfig DisplayConfig;
        public EndingRewardConfig RewardConfig;
    }

    [Serializable]
    public class CharacterEndingConfig
    {
        public string CharacterId;
        public string CharacterName;
        public string EndingId;
        public string EndingName;
        public string Description;
        public Sprite CharacterSprite;
        public Sprite CGImage;
        public AudioClip BGM;
        public int RequiredFavorability = 70;
        public bool RequireMutualChoice;
        public EndingDisplayConfig DisplayConfig;
        public EndingRewardConfig RewardConfig;
    }

    [Serializable]
    public class SecretEndingConfig
    {
        public string EndingId;
        public string EndingName;
        public string Description;
        public Sprite Background;
        public Sprite CGImage;
        public AudioClip BGM;
        public bool IsHidden;
        public string UnlockHint;
        public EndingDisplayConfig DisplayConfig;
        public EndingRewardConfig RewardConfig;
    }

    [Serializable]
    public class EndingDisplayConfig
    {
        public float FadeInDuration = 1f;
        public float TitleDisplayDuration = 2f;
        public float TextScrollSpeed = 30f;
        public float AutoSkipDelay = 5f;
        public bool AllowSkip = true;
        public bool ShowStats = true;
    }

    [Serializable]
    public class EndingRewardConfig
    {
        public int Diamonds;
        public int Experience;
        public string UnlockItemId;
        public string[] UnlockAchievementIds;
    }
}
