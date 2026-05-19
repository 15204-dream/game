using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingShow.API
{
    /// <summary>
    /// API响应解析类
    /// 负责解析DeepSeek API的JSON响应
    /// </summary>
    public static class APIResponseParser
    {
        /// <summary>
        /// 解析成功的API响应
        /// </summary>
        /// <param name="jsonResponse">JSON响应字符串</param>
        /// <returns>解析后的API响应对象</returns>
        public static APIResponse ParseSuccessResponse(string jsonResponse)
        {
            try
            {
                // 使用Unity的JsonUtility解析JSON
                var responseWrapper = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
                
                if (responseWrapper == null || responseWrapper.choices == null || responseWrapper.choices.Count == 0)
                {
                    return new APIResponse
                    {
                        success = false,
                        error = "响应格式错误：缺少choices字段"
                    };
                }
                
                // 提取消息内容
                string content = "";
                if (responseWrapper.choices[0].message != null)
                {
                    content = responseWrapper.choices[0].message.content;
                }
                else if (responseWrapper.choices[0].delta != null)
                {
                    content = responseWrapper.choices[0].delta.content;
                }
                
                return new APIResponse
                {
                    success = true,
                    content = content
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析API响应失败: {ex.Message}");
                return new APIResponse
                {
                    success = false,
                    error = $"解析响应失败: {ex.Message}"
                };
            }
        }
        
        /// <summary>
        /// 解析错误响应
        /// </summary>
        /// <param name="jsonResponse">错误响应JSON字符串</param>
        /// <returns>错误响应对象</returns>
        public static APIErrorResponse ParseErrorResponse(string jsonResponse)
        {
            try
            {
                var errorResponse = JsonUtility.FromJson<APIErrorResponse>(jsonResponse);
                return errorResponse ?? new APIErrorResponse();
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析错误响应失败: {ex.Message}");
                return new APIErrorResponse();
            }
        }
        
        /// <summary>
        /// 解析流式响应数据块
        /// </summary>
        /// <param name="chunkJson">数据块JSON字符串</param>
        /// <returns>解析的内容，如果解析失败返回null</returns>
        public static string ParseStreamChunk(string chunkJson)
        {
            try
            {
                var chunk = JsonUtility.FromJson<StreamChunk>(chunkJson);
                
                if (chunk == null || chunk.choices == null || chunk.choices.Count == 0)
                {
                    return null;
                }
                
                var delta = chunk.choices[0].delta;
                if (delta != null && !string.IsNullOrEmpty(delta.content))
                {
                    return delta.content;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"解析流式数据块失败: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 解析聊天完成响应（完整版本）
        /// </summary>
        /// <param name="jsonResponse">JSON响应字符串</param>
        /// <returns>聊天完成响应对象</returns>
        public static ChatCompletionResponse ParseChatCompletion(string jsonResponse)
        {
            try
            {
                var response = JsonUtility.FromJson<ChatCompletionResponse>(jsonResponse);
                return response ?? new ChatCompletionResponse();
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析聊天完成响应失败: {ex.Message}");
                return new ChatCompletionResponse();
            }
        }
        
        /// <summary>
        /// 提取响应中的使用统计信息
        /// </summary>
        /// <param name="jsonResponse">JSON响应字符串</param>
        /// <returns>使用统计对象</returns>
        public static UsageInfo ExtractUsageInfo(string jsonResponse)
        {
            try
            {
                var response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
                
                if (response?.usage != null)
                {
                    return response.usage;
                }
                
                return new UsageInfo();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"提取使用统计失败: {ex.Message}");
                return new UsageInfo();
            }
        }
        
        /// <summary>
        /// 验证响应是否包含有效内容
        /// </summary>
        /// <param name="response">API响应对象</param>
        /// <returns>是否有效</returns>
        public static bool IsValidResponse(APIResponse response)
        {
            return response != null && response.success && !string.IsNullOrEmpty(response.content);
        }
        
        /// <summary>
        /// 获取错误描述
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <returns>错误描述</returns>
        public static string GetErrorDescription(long? errorCode)
        {
            if (!errorCode.HasValue)
            {
                return "未知错误";
            }
            
            return errorCode.Value switch
            {
                400 => "请求参数错误",
                401 => "API密钥无效或缺失",
                403 => "没有访问权限",
                404 => "请求的资源不存在",
                429 => "请求频率超限",
                500 => "服务器内部错误",
                502 => "网关错误",
                503 => "服务暂时不可用",
                504 => "网关超时",
                _ => $"未知错误 (代码: {errorCode})"
            };
        }
        
        /// <summary>
        /// 解析响应ID
        /// </summary>
        /// <param name="jsonResponse">JSON响应字符串</param>
        /// <returns>响应ID</returns>
        public static string ExtractResponseId(string jsonResponse)
        {
            try
            {
                var response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
                return response?.id ?? "";
            }
            catch
            {
                return "";
            }
        }
        
        /// <summary>
        /// 解析响应创建时间
        /// </summary>
        /// <param name="jsonResponse">JSON响应字符串</param>
        /// <returns>创建时间戳</returns>
        public static long ExtractCreatedTimestamp(string jsonResponse)
        {
            try
            {
                var response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
                return response?.created ?? 0;
            }
            catch
            {
                return 0;
            }
        }
        
        /// <summary>
        /// 解析模型名称
        /// </summary>
        /// <param name="jsonResponse">JSON响应字符串</param>
        /// <returns>模型名称</returns>
        public static string ExtractModel(string jsonResponse)
        {
            try
            {
                var response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
                return response?.model ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
    
    /// <summary>
    /// DeepSeek API响应类
    /// </summary>
    [Serializable]
    public class DeepSeekResponse
    {
        public string id;
        public string object_type;
        public long created;
        public string model;
        public List<Choice> choices;
        public UsageInfo usage;
        public string finish_reason;
    }
    
    /// <summary>
    /// 选择类
    /// </summary>
    [Serializable]
    public class Choice
    {
        public int index;
        public MessageContent message;
        public MessageContent delta;
        public string finish_reason;
    }
    
    /// <summary>
    /// 消息内容类
    /// </summary>
    [Serializable]
    public class MessageContent
    {
        public string role;
        public string content;
    }
    
    /// <summary>
    /// 使用统计类
    /// </summary>
    [Serializable]
    public class UsageInfo
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
    
    /// <summary>
    /// 流式数据块类
    /// </summary>
    [Serializable]
    public class StreamChunk
    {
        public string id;
        public string object_type;
        public int created;
        public string model;
        public List<StreamChoice> choices;
    }
    
    /// <summary>
    /// 流式选择类
    /// </summary>
    [Serializable]
    public class StreamChoice
    {
        public int index;
        public MessageContent delta;
        public string finish_reason;
    }
    
    /// <summary>
    /// 聊天完成响应类
    /// </summary>
    [Serializable]
    public class ChatCompletionResponse
    {
        public string id;
        public string object_type = "chat.completion";
        public int created;
        public string model;
        public List<Choice> choices = new List<Choice>();
        public UsageInfo usage;
    }
}
