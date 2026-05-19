using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DatingShow.API
{
    /// <summary>
    /// DeepSeek API请求处理类
    /// 负责处理所有与DeepSeek API的通信
    /// </summary>
    public class DeepSeekRequestHandler : MonoBehaviour
    {
        // API客户端引用
        private APIClient apiClient;
        
        // 请求队列（用于频率限制）
        private Queue<long> requestTimestamps = new Queue<long>();
        
        // 同步锁
        private readonly object rateLimitLock = new object();
        
        // 取消令牌源
        private CancellationTokenSource cancellationTokenSource;
        
        /// <summary>
        /// 初始化请求处理器
        /// </summary>
        public void Initialize()
        {
            apiClient = APIClient.Instance;
            cancellationTokenSource = new CancellationTokenSource();
        }
        
        /// <summary>
        /// 发送对话生成请求
        /// </summary>
        /// <param name="messages">消息列表</param>
        /// <param name="useStreaming">是否使用流式响应</param>
        /// <returns>API响应</returns>
        public async Task<APIResponse> SendChatRequest(List<ChatMessage> messages, bool useStreaming = false)
        {
            if (apiClient == null)
            {
                Initialize();
            }
            
            // 应用频率限制
            await ApplyRateLimit();
            
            // 构建请求数据
            var requestData = new ChatCompletionRequest
            {
                model = apiClient.Model,
                messages = messages.Select(m => new Message { role = m.Role, content = m.Content }).ToList(),
                max_tokens = apiClient.MaxTokens,
                temperature = apiClient.Temperature,
                top_p = apiClient.TopP,
                stream = useStreaming
            };
            
            // 发送请求并进行重试
            return await SendRequestWithRetry(requestData, useStreaming);
        }
        
        /// <summary>
        /// 发送带重试机制的请求
        /// </summary>
        /// <param name="requestData">请求数据</param>
        /// <param name="useStreaming">是否使用流式响应</param>
        /// <returns>API响应</returns>
        private async Task<APIResponse> SendRequestWithRetry(ChatCompletionRequest requestData, bool useStreaming)
        {
            int retryCount = 0;
            int maxRetries = apiClient.MaxRetries;
            
            while (retryCount <= maxRetries)
            {
                try
                {
                    // 发送实际请求
                    return await SendRequest(requestData, useStreaming);
                }
                catch (APIException ex) when (ex.IsRetryable && retryCount < maxRetries)
                {
                    retryCount++;
                    Debug.LogWarning($"API请求失败，正在进行第{retryCount}次重试 (最大{maxRetries}次)");
                    
                    // 指数退避策略
                    int delayMs = (int)Math.Pow(2, retryCount) * 1000;
                    await Task.Delay(delayMs);
                }
                catch (APIException ex)
                {
                    // 非可重试错误，直接抛出
                    Debug.LogError($"API请求失败: {ex.Message}");
                    return new APIResponse
                    {
                        success = false,
                        error = ex.Message,
                        errorCode = ex.ErrorCode
                    };
                }
                catch (Exception ex)
                {
                    Debug.LogError($"发生未知错误: {ex.Message}");
                    return new APIResponse
                    {
                        success = false,
                        error = ex.Message
                    };
                }
            }
            
            return new APIResponse
            {
                success = false,
                error = "达到最大重试次数，请求失败"
            };
        }
        
        /// <summary>
        /// 发送实际HTTP请求
        /// </summary>
        /// <param name="requestData">请求数据</param>
        /// <param name="useStreaming">是否使用流式响应</param>
        /// <returns>API响应</returns>
        private async Task<APIResponse> SendRequest(ChatCompletionRequest requestData, bool useStreaming)
        {
            // 转换为JSON
            string jsonData = JsonUtility.ToJson(requestData);
            
            // 创建UnityWebRequest
            using (UnityWebRequest request = new UnityWebRequest(apiClient.GetEndpoint(), "POST"))
            {
                // 设置请求头
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", apiClient.GetAuthorizationHeader());
                
                // 设置请求体
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                
                // 设置下载处理器
                request.downloadHandler = new DownloadHandlerBuffer();
                
                // 设置超时
                request.timeout = apiClient.TimeoutSeconds;
                
                // 发送请求
                var asyncOperation = request.SendWebRequest();
                
                // 等待请求完成
                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                    
                    // 检查是否已取消
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        request.Abort();
                        throw new OperationCanceledException("请求已被取消");
                    }
                }
                
                // 检查HTTP错误
                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMessage = $"HTTP错误: {request.responseCode} - {request.error}";
                    string errorContent = request.downloadHandler.text;
                    
                    // 解析错误响应
                    var errorResponse = APIResponseParser.ParseErrorResponse(errorContent);
                    
                    throw new APIException(errorMessage, request.responseCode, errorResponse);
                }
                
                // 解析成功响应
                string responseJson = request.downloadHandler.text;
                return APIResponseParser.ParseSuccessResponse(responseJson);
            }
        }
        
        /// <summary>
        /// 应用请求频率限制
        /// </summary>
        private async Task ApplyRateLimit()
        {
            int requestsPerSecond = apiClient.RequestsPerSecond;
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            lock (rateLimitLock)
            {
                // 清理超过1秒的记录
                long oneSecondAgo = currentTime - 1000;
                while (requestTimestamps.Count > 0 && requestTimestamps.Peek() < oneSecondAgo)
                {
                    requestTimestamps.Dequeue();
                }
                
                // 如果超过限制，等待
                if (requestTimestamps.Count >= requestsPerSecond)
                {
                    long waitTime = 1000 - (currentTime - requestTimestamps.Peek());
                    if (waitTime > 0)
                    {
                        // 释放锁后等待
                        Monitor.Exit(rateLimitLock);
                        Task.Delay((int)waitTime).Wait();
                        Monitor.Enter(rateLimitLock);
                    }
                }
                
                // 记录当前请求时间
                requestTimestamps.Enqueue(currentTime);
            }
        }
        
        /// <summary>
        /// 发送流式请求
        /// </summary>
        /// <param name="messages">消息列表</param>
        /// <param name="onChunkReceived">接收到数据块时的回调</param>
        /// <returns>任务</returns>
        public async Task SendStreamingRequest(List<ChatMessage> messages, Action<string> onChunkReceived)
        {
            if (apiClient == null)
            {
                Initialize();
            }
            
            // 应用频率限制
            await ApplyRateLimit();
            
            // 构建请求数据
            var requestData = new ChatCompletionRequest
            {
                model = apiClient.Model,
                messages = messages.Select(m => new Message { role = m.Role, content = m.Content }).ToList(),
                max_tokens = apiClient.MaxTokens,
                temperature = apiClient.Temperature,
                top_p = apiClient.TopP,
                stream = true
            };
            
            // 转换为JSON
            string jsonData = JsonUtility.ToJson(requestData);
            
            // 创建UnityWebRequest
            using (UnityWebRequest request = new UnityWebRequest(apiClient.GetStreamingEndpoint(), "POST"))
            {
                // 设置请求头
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", apiClient.GetAuthorizationHeader());
                
                // 设置请求体
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                
                // 设置下载处理器（流式）
                request.downloadHandler = new DownloadHandlerStream();
                
                // 设置超时
                request.timeout = apiClient.TimeoutSeconds;
                
                // 发送请求
                var asyncOperation = request.SendWebRequest();
                
                // 等待请求开始
                while (!asyncOperation.isDone && !request.isDone)
                {
                    await Task.Yield();
                    
                    // 处理接收到的数据
                    DownloadHandlerStream streamHandler = (DownloadHandlerStream)request.downloadHandler;
                    string receivedData = streamHandler.Text;
                    
                    if (!string.IsNullOrEmpty(receivedData))
                    {
                        // 解析SSE数据
                        string[] lines = receivedData.Split('\n');
                        foreach (string line in lines)
                        {
                            if (line.StartsWith("data: "))
                            {
                                string json = line.Substring(6);
                                if (json != "[DONE]")
                                {
                                    var chunk = APIResponseParser.ParseStreamChunk(json);
                                    if (chunk != null)
                                    {
                                        onChunkReceived?.Invoke(chunk);
                                    }
                                }
                            }
                        }
                    }
                    
                    // 检查是否已取消
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        request.Abort();
                        return;
                    }
                }
                
                // 检查HTTP错误
                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new APIException($"流式请求失败: {request.error}", request.responseCode);
                }
            }
        }
        
        /// <summary>
        /// 取消所有待处理的请求
        /// </summary>
        public void CancelAllRequests()
        {
            cancellationTokenSource?.Cancel();
        }
        
        /// <summary>
        /// 重新初始化取消令牌
        /// </summary>
        public void ResetCancellationToken()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }
        
        /// <summary>
        /// 清理资源
        /// </summary>
        private void OnDestroy()
        {
            CancelAllRequests();
            cancellationTokenSource?.Dispose();
        }
    }
    
    /// <summary>
    /// 聊天消息类
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        public string Role;
        public string Content;
        
        public ChatMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }
    
    /// <summary>
    /// API聊天完成请求类
    /// </summary>
    [Serializable]
    public class ChatCompletionRequest
    {
        public string model;
        public List<Message> messages;
        public int max_tokens;
        public float temperature;
        public float top_p;
        public bool stream;
    }
    
    /// <summary>
    /// 消息类
    /// </summary>
    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
    
    /// <summary>
    /// API响应类
    /// </summary>
    [Serializable]
    public class APIResponse
    {
        public bool success;
        public string content;
        public string error;
        public long? errorCode;
    }
    
    /// <summary>
    /// API异常类
    /// </summary>
    public class APIException : Exception
    {
        public long? ErrorCode { get; }
        public bool IsRetryable { get; }
        
        public APIException(string message, long? errorCode = null, APIErrorResponse errorResponse = null) 
            : base(message)
        {
            ErrorCode = errorCode;
            
            // 判断是否可重试
            IsRetryable = errorCode switch
            {
                429 => true,  // Rate limit
                500 => true,  // Server error
                502 => true,  // Bad gateway
                503 => true,  // Service unavailable
                504 => true,  // Gateway timeout
                _ => errorResponse?.IsRetryable ?? false
            };
        }
    }
    
    /// <summary>
    /// API错误响应类
    /// </summary>
    [Serializable]
    public class APIErrorResponse
    {
        public ErrorDetail error;
        
        public bool IsRetryable
        {
            get
            {
                if (error == null) return false;
                return error.code == "rate_limit_error" || 
                       error.code == "server_error";
            }
        }
    }
    
    [Serializable]
    public class ErrorDetail
    {
        public string message;
        public string type;
        public string code;
    }
}
