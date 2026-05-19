using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DatingShow.API
{
    /// <summary>
    /// 提示词管理器
    /// 负责加载和管理游戏提示词
    /// </summary>
    public class PromptManager : MonoBehaviour
    {
        // 提示词文件路径
        private const string GUEST_PROMPT_FILE = "GuestModePrompt.txt";
        private const string DIRECTOR_PROMPT_FILE = "DirectorModePrompt.txt";
        
        // 缓存的提示词
        private static string cachedGuestPrompt;
        private static string cachedDirectorPrompt;
        
        // 提示词是否已加载
        private static bool isGuestPromptLoaded = false;
        private static bool isDirectorPromptLoaded = false;
        
        /// <summary>
        /// 加载嘉宾模式提示词
        /// </summary>
        /// <returns>提示词内容</returns>
        public static async Task<string> LoadGuestPrompt()
        {
            if (isGuestPromptLoaded && !string.IsNullOrEmpty(cachedGuestPrompt))
            {
                return cachedGuestPrompt;
            }
            
            cachedGuestPrompt = await LoadPromptFile(GUEST_PROMPT_FILE);
            isGuestPromptLoaded = true;
            
            return cachedGuestPrompt;
        }
        
        /// <summary>
        /// 加载导演模式提示词
        /// </summary>
        /// <returns>提示词内容</returns>
        public static async Task<string> LoadDirectorPrompt()
        {
            if (isDirectorPromptLoaded && !string.IsNullOrEmpty(cachedDirectorPrompt))
            {
                return cachedDirectorPrompt;
            }
            
            cachedDirectorPrompt = await LoadPromptFile(DIRECTOR_PROMPT_FILE);
            isDirectorPromptLoaded = true;
            
            return cachedDirectorPrompt;
        }
        
        /// <summary>
        /// 从文件加载提示词
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>提示词内容</returns>
        private static async Task<string> LoadPromptFile(string fileName)
        {
            try
            {
                // 尝试多种路径
                string[] possiblePaths = new string[]
                {
                    Path.Combine(Application.dataPath, "Scripts", "Core", "API", fileName),
                    Path.Combine(Application.dataPath, "Scripts", "Core", "API", "Prompts", fileName),
                    Path.Combine(Application.streamingAssetsPath, "Prompts", fileName),
                    Path.Combine(Application.streamingAssetsPath, fileName),
                    Path.Combine(Application.persistentDataPath, "Prompts", fileName)
                };
                
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        string content = await ReadFileAsync(path);
                        Debug.Log($"成功加载提示词文件: {path}");
                        return content;
                    }
                }
                
                // 如果所有路径都找不到，使用Resources加载
                string resourcesContent = await LoadFromResources(fileName);
                if (!string.IsNullOrEmpty(resourcesContent))
                {
                    return resourcesContent;
                }
                
                Debug.LogWarning($"未找到提示词文件: {fileName}");
                return GetDefaultPrompt(fileName);
            }
            catch (Exception ex)
            {
                Debug.LogError($"加载提示词文件失败: {ex.Message}");
                return GetDefaultPrompt(fileName);
            }
        }
        
        /// <summary>
        /// 异步读取文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件内容</returns>
        private static async Task<string> ReadFileAsync(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (StreamReader reader = new StreamReader(fs))
            {
                return await reader.ReadToEndAsync();
            }
        }
        
        /// <summary>
        /// 从Resources目录加载
        /// </summary>
        /// <param name="fileName">文件名（不含扩展名）</param>
        /// <returns>文件内容</returns>
        private static async Task<string> LoadFromResources(string fileName)
        {
            try
            {
                // 移除扩展名用于Resources.Load
                string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                
                TextAsset textAsset = Resources.Load<TextAsset>(nameWithoutExt);
                if (textAsset != null)
                {
                    return textAsset.text;
                }
                
                // 尝试带路径
                TextAsset textAssetWithPath = Resources.Load<TextAsset>($"Prompts/{nameWithoutExt}");
                if (textAssetWithPath != null)
                {
                    return textAssetWithPath.text;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"从Resources加载失败: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 获取默认提示词
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>默认提示词</returns>
        private static string GetDefaultPrompt(string fileName)
        {
            if (fileName.Contains("Guest"))
            {
                return GetDefaultGuestPrompt();
            }
            else if (fileName.Contains("Director"))
            {
                return GetDefaultDirectorPrompt();
            }
            
            return "你是《糟糕！是心动鸭！》恋综模拟游戏中的角色。";
        }
        
        /// <summary>
        /// 获取默认嘉宾提示词
        /// </summary>
        /// <returns>默认提示词内容</returns>
        private static string GetDefaultGuestPrompt()
        {
            return @"你是《糟糕！是心动鸭！》恋综模拟游戏中的AI嘉宾角色。

## 性格要求
- 活泼开朗，真诚友善
- 幽默风趣，有主见
- 说话自然，不做作

## 说话风格
- 轻松幽默，适度使用网络用语
- 每条回复控制在100字以内
- 有情感表达，有画面感

## 场景
- 豪华别墅、心动小屋
- 入住第一天，浪漫氛围
- 初次见面，互相了解

请生成符合上述要求的对话内容。";
        }
        
        /// <summary>
        /// 获取默认导演提示词
        /// </summary>
        /// <returns>默认提示词内容</returns>
        private static string GetDefaultDirectorPrompt()
        {
            return @"你是《糟糕！是心动鸭！》恋综模拟游戏中的AI导演。

## 职责
- 引导剧情发展
- 执行游戏规则
- 制造戏剧冲突
- 管理游戏节奏

## 风格
- 专业大气
- 幽默风趣
- 引导性强
- 制造悬念

## 环节
- 入营仪式
- 初次约会
- 心动选择
- 真心话大冒险
- 告白之夜

请生成符合导演身份的引导内容。";
        }
        
        /// <summary>
        /// 重新加载所有提示词
        /// </summary>
        public static async Task ReloadAllPrompts()
        {
            isGuestPromptLoaded = false;
            isDirectorPromptLoaded = false;
            cachedGuestPrompt = null;
            cachedDirectorPrompt = null;
            
            await Task.WhenAll(
                LoadGuestPrompt(),
                LoadDirectorPrompt()
            );
        }
        
        /// <summary>
        /// 清除提示词缓存
        /// </summary>
        public static void ClearCache()
        {
            cachedGuestPrompt = null;
            cachedDirectorPrompt = null;
            isGuestPromptLoaded = false;
            isDirectorPromptLoaded = false;
        }
        
        /// <summary>
        /// 检查提示词是否已加载
        /// </summary>
        /// <param name="mode">模式（guest/director）</param>
        /// <returns>是否已加载</returns>
        public static bool IsPromptLoaded(string mode)
        {
            if (mode.Equals("guest", StringComparison.OrdinalIgnoreCase))
            {
                return isGuestPromptLoaded && !string.IsNullOrEmpty(cachedGuestPrompt);
            }
            else if (mode.Equals("director", StringComparison.OrdinalIgnoreCase))
            {
                return isDirectorPromptLoaded && !string.IsNullOrEmpty(cachedDirectorPrompt);
            }
            
            return false;
        }
        
        /// <summary>
        /// 获取当前缓存的提示词（同步版本）
        /// </summary>
        /// <param name="mode">模式</param>
        /// <returns>提示词内容，如果未加载返回null</returns>
        public static string GetCachedPrompt(string mode)
        {
            if (mode.Equals("guest", StringComparison.OrdinalIgnoreCase))
            {
                return cachedGuestPrompt;
            }
            else if (mode.Equals("director", StringComparison.OrdinalIgnoreCase))
            {
                return cachedDirectorPrompt;
            }
            
            return null;
        }
        
        /// <summary>
        /// 验证提示词文件是否存在
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否存在</returns>
        public static bool ValidatePromptFile(string fileName)
        {
            string[] possiblePaths = new string[]
            {
                Path.Combine(Application.dataPath, "Scripts", "Core", "API", fileName),
                Path.Combine(Application.dataPath, "Scripts", "Core", "API", "Prompts", fileName),
                Path.Combine(Application.streamingAssetsPath, "Prompts", fileName),
                Path.Combine(Application.streamingAssetsPath, fileName)
            };
            
            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 获取提示词文件位置
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件路径，如果未找到返回空字符串</returns>
        public static string GetPromptFileLocation(string fileName)
        {
            string[] possiblePaths = new string[]
            {
                Path.Combine(Application.dataPath, "Scripts", "Core", "API", fileName),
                Path.Combine(Application.dataPath, "Scripts", "Core", "API", "Prompts", fileName),
                Path.Combine(Application.streamingAssetsPath, "Prompts", fileName),
                Path.Combine(Application.streamingAssetsPath, fileName)
            };
            
            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            
            return "";
        }
    }
}
