using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace 糟糕是心动鸭.Dialogue.GuestMode
{
    /// <summary>
    /// 对话响应生成器
    /// 使用AI生成角色对话响应
    /// </summary>
    public class DialogueResponseGenerator
    {
        /// <summary>
        /// API客户端
        /// </summary>
        private API.APIClient apiClient;

        /// <summary>
        /// 响应缓存
        /// </summary>
        private Dictionary<string, CachedResponse> responseCache;

        /// <summary>
        /// 最大缓存大小
        /// </summary>
        private const int MAX_CACHE_SIZE = 50;

        /// <summary>
        /// 响应超时时间（秒）
        /// </summary>
        private const int RESPONSE_TIMEOUT = 10;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DialogueResponseGenerator()
        {
            apiClient = API.APIClient.Instance;
            responseCache = new Dictionary<string, CachedResponse>();
        }

        #endregion

        #region 响应生成

        /// <summary>
        /// 生成角色响应
        /// </summary>
        /// <param name="character">角色数据</param>
        /// <param name="playerOption">玩家选择的选项</param>
        /// <param name="controller">对话控制器</param>
        /// <returns>响应文本</returns>
        public async Task<string> GenerateResponse(
            CharacterData character,
            DialogueOption playerOption,
            CharacterDialogueController controller)
        {
            try
            {
                string cacheKey = GenerateCacheKey(character, playerOption, controller);
                if (responseCache.TryGetValue(cacheKey, out CachedResponse cached))
                {
                    if (!cached.IsExpired())
                    {
                        return cached.Response;
                    }
                }

                string prompt = BuildPrompt(character, playerOption, controller);
                string response = await GenerateAIResponse(prompt);

                AddToCache(cacheKey, response);

                return response;
            }
            catch (Exception e)
            {
                Debug.LogError($"Response generation error: {e.Message}");
                return GetFallbackResponse(playerOption);
            }
        }

        /// <summary>
        /// 生成AI响应
        /// </summary>
        /// <param name="prompt">提示词</param>
        /// <returns>响应文本</returns>
        private async Task<string> GenerateAIResponse(string prompt)
        {
            try
            {
                string response = await apiClient.SendRequestAsync(prompt);
                return ParseAIResponse(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"AI request error: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 解析AI响应
        /// </summary>
        /// <param name="rawResponse">原始响应</param>
        /// <returns>解析后的文本</returns>
        private string ParseAIResponse(string rawResponse)
        {
            if (string.IsNullOrEmpty(rawResponse))
            {
                return "...";
            }

            rawResponse = rawResponse.Trim();

            if (rawResponse.Length > 200)
            {
                int truncateIndex = rawResponse.LastIndexOf('。');
                if (truncateIndex > 100)
                {
                    rawResponse = rawResponse.Substring(0, truncateIndex + 1);
                }
            }

            return rawResponse;
        }

        /// <summary>
        /// 构建提示词
        /// </summary>
        /// <param name="character">角色数据</param>
        /// <param name="playerOption">玩家选项</param>
        /// <param name="controller">控制器</param>
        /// <returns>提示词</returns>
        private string BuildPrompt(
            CharacterData character,
            DialogueOption playerOption,
            CharacterDialogueController controller)
        {
            StringBuilder prompt = new StringBuilder();

            prompt.AppendLine("你是《糟糕！是心动鸭！》恋综游戏中的角色。");
            prompt.AppendLine();
            prompt.AppendLine($"角色信息:");
            prompt.AppendLine($"- 名字: {character.Name}");
            prompt.AppendLine($"- 性格: {character.GetPersonalityTagsString()}");
            prompt.AppendLine($"- 兴趣: {character.GetInterestTagsString()}");
            prompt.AppendLine($"- 背景: {character.Backstory}");
            prompt.AppendLine();
            prompt.AppendLine($"当前状态:");
            prompt.AppendLine($"- 好感度: {controller.CurrentAffection}");
            prompt.AppendLine($"- 关系阶段: {controller.GetRelationshipStage()}");
            prompt.AppendLine();
            prompt.AppendLine($"对话历史:");
            prompt.AppendLine(controller.GetHistoryText());
            prompt.AppendLine();
            prompt.AppendLine($"玩家说: {playerOption.Text}");

            if (!string.IsNullOrEmpty(playerOption.Description))
            {
                prompt.AppendLine($"玩家意图: {playerOption.Description}");
            }

            prompt.AppendLine();
            prompt.AppendLine("请用角色的口吻回复，注意:");
            prompt.AppendLine("1. 回复要符合角色性格");
            prompt.AppendLine("2. 长度适中（20-100字）");
            prompt.AppendLine("3. 要有情感变化");
            prompt.AppendLine("4. 不要太长");

            return prompt.ToString();
        }

        #endregion

        #region 缓存管理

        /// <summary>
        /// 生成缓存键
        /// </summary>
        <param name="character">角色</param>
        <param name="option">选项</param>
        <param name="controller">控制器</param>
        /// <returns>缓存键</returns>
        private string GenerateCacheKey(
            CharacterData character,
            DialogueOption option,
            CharacterDialogueController controller)
        {
            return $"{character.Id}_{option.OptionId}_{controller.CurrentAffection}_{controller.DialogueRound}";
        }

        /// <summary>
        /// 添加到缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="response">响应</param>
        private void AddToCache(string key, string response)
        {
            if (responseCache.Count >= MAX_CACHE_SIZE)
            {
                RemoveOldestCache();
            }

            responseCache[key] = new CachedResponse(response);
        }

        /// <summary>
        /// 移除最旧的缓存
        /// </summary>
        private void RemoveOldestCache()
        {
            if (responseCache.Count == 0) return;

            string oldestKey = null;
            DateTime oldestTime = DateTime.MaxValue;

            foreach (var kvp in responseCache)
            {
                if (kvp.Value.Timestamp < oldestTime)
                {
                    oldestTime = kvp.Value.Timestamp;
                    oldestKey = kvp.Key;
                }
            }

            if (oldestKey != null)
            {
                responseCache.Remove(oldestKey);
            }
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            responseCache.Clear();
        }

        #endregion

        #region 备用响应

        /// <summary>
        /// 获取备用响应
        /// </summary>
        /// <param name="option">选项</param>
        /// <returns>响应文本</returns>
        private string GetFallbackResponse(DialogueOption option)
        {
            if (option.AffectionChange > 10)
            {
                return GetPositiveResponse();
            }
            else if (option.AffectionChange > 0)
            {
                return GetNeutralPositiveResponse();
            }
            else if (option.AffectionChange < -5)
            {
                return GetNegativeResponse();
            }
            return GetNeutralResponse();
        }

        /// <summary>
        /// 获取积极响应
        /// </summary>
        /// <returns>响应文本</returns>
        private string GetPositiveResponse()
        {
            string[] responses = new[]
            {
                "哇，你真的很了解我呢！",
                "真的吗？我好开心！",
                "你这样说，让我有点害羞呢~",
                "太棒了！我也有同感！",
                "哈哈，你真会说话~"
            };
            return responses[UnityEngine.Random.Range(0, responses.Length)];
        }

        /// <summary>
        /// 获取中性积极响应
        /// </summary>
        /// <returns>响应文本</returns>
        private string GetNeutralPositiveResponse()
        {
            string[] responses = new[]
            {
                "嗯，说得对呢~",
                "我也有点这么觉得",
                "确实是这样",
                "有道理~",
                "嗯嗯，不错"
            };
            return responses[UnityEngine.Random.Range(0, responses.Length)];
        }

        /// <summary>
        /// 获取中性响应
        /// </summary>
        /// <returns>响应文本</returns>
        private string GetNeutralResponse()
        {
            string[] responses = new[]
            {
                "原来是这样啊",
                "嗯，我知道了",
                "这样啊...",
                "好的呢",
                "好的好的"
            };
            return responses[UnityEngine.Random.Range(0, responses.Length)];
        }

        /// <summary>
        /// 获取消极响应
        /// </summary>
        /// <returns>响应文本</returns>
        private string GetNegativeResponse()
        {
            string[] responses = new[]
            {
                "嗯...是吗？",
                "这样啊...",
                "我不太同意呢",
                "有点意外...",
                "嗯...我再想想"
            };
            return responses[UnityEngine.Random.Range(0, responses.Length)];
        }

        #endregion
    }

    /// <summary>
    /// 缓存的响应
    /// </summary>
    [Serializable]
    public class CachedResponse
    {
        /// <summary>
        /// 响应文本
        /// </summary>
        public string Response;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp;

        /// <summary>
        /// 过期时间（分钟）
        /// </summary>
        private const int EXPIRE_MINUTES = 5;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="response">响应</param>
        public CachedResponse(string response)
        {
            Response = response;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 检查是否过期
        /// </summary>
        /// <returns>是否过期</returns>
        public bool IsExpired()
        {
            return (DateTime.Now - Timestamp).TotalMinutes >= EXPIRE_MINUTES;
        }
    }
}
