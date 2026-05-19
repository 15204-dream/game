using System;
using UnityEngine;

namespace DatingShow.API
{
    /// <summary>
    /// DeepSeek API配置类
    /// 存储API连接所需的所有配置信息
    /// </summary>
    [Serializable]
    public class APIClient : MonoBehaviour
    {
        // API基础URL
        [SerializeField]
        private string baseUrl = "https://api.deepseek.com/v1";
        
        // 使用的模型
        [SerializeField]
        private string model = "deepseek-chat";
        
        // API密钥
        [SerializeField]
        private string apiKey = "sk-fe8fdf06158c44b5b6304a8e30dce5a8";
        
        // 最大Token数
        [SerializeField]
        private int maxTokens = 2048;
        
        // Temperature参数（控制随机性）
        [Range(0.0f, 2.0f)]
        [SerializeField]
        private float temperature = 0.8f;
        
        // TopP参数（控制采样范围）
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float topP = 0.9f;
        
        // 请求超时时间（秒）
        [SerializeField]
        private int timeoutSeconds = 30;
        
        // 最大重试次数
        [SerializeField]
        private int maxRetries = 3;
        
        // 请求频率限制（每秒请求数）
        [SerializeField]
        private int requestsPerSecond = 2;
        
        // 属性访问器
        public string BaseUrl => baseUrl;
        public string Model => model;
        public string APIKey => apiKey;
        public int MaxTokens => maxTokens;
        public float Temperature => temperature;
        public float TopP => topP;
        public int TimeoutSeconds => timeoutSeconds;
        public int MaxRetries => maxRetries;
        public int RequestsPerSecond => requestsPerSecond;
        
        // 单例模式实例
        private static APIClient instance;
        public static APIClient Instance
        {
            get
            {
                if (instance == null)
                {
                    // 在场景中查找现有实例
                    instance = FindObjectOfType<APIClient>();
                    
                    // 如果没有找到，创建一个新的
                    if (instance == null)
                    {
                        GameObject go = new GameObject("APIClient");
                        instance = go.AddComponent<APIClient>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        /// <summary>
        /// 验证API配置是否有效
        /// </summary>
        /// <returns>配置是否有效</returns>
        public bool IsConfigurationValid()
        {
            return !string.IsNullOrEmpty(baseUrl) &&
                   !string.IsNullOrEmpty(model) &&
                   !string.IsNullOrEmpty(apiKey) &&
                   maxTokens > 0 &&
                   temperature >= 0 && temperature <= 2 &&
                   topP >= 0 && topP <= 1;
        }
        
        /// <summary>
        /// 获取API端点URL
        /// </summary>
        /// <returns>完整的API端点URL</returns>
        public string GetEndpoint()
        {
            return $"{baseUrl}/chat/completions";
        }
        
        /// <summary>
        /// 获取流式API端点URL
        /// </summary>
        /// <returns>流式API端点URL</returns>
        public string GetStreamingEndpoint()
        {
            return $"{baseUrl}/chat/completions";
        }
        
        /// <summary>
        /// 获取认证头信息
        /// </summary>
        /// <returns>Authorization头</returns>
        public string GetAuthorizationHeader()
        {
            return $"Bearer {apiKey}";
        }
        
        /// <summary>
        /// Awake生命周期方法 - 初始化单例
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 重置配置到默认值
        /// </summary>
        public void ResetToDefaults()
        {
            baseUrl = "https://api.deepseek.com/v1";
            model = "deepseek-chat";
            maxTokens = 2048;
            temperature = 0.8f;
            topP = 0.9f;
            timeoutSeconds = 30;
            maxRetries = 3;
            requestsPerSecond = 2;
        }
        
        /// <summary>
        /// 验证API密钥格式是否正确
        /// </summary>
        /// <returns>密钥格式是否正确</returns>
        public bool ValidateAPIKeyFormat()
        {
            return apiKey.StartsWith("sk-") && apiKey.Length > 20;
        }
    }
}
