using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace 糟糕是心动鸭.Dialogue.Filter
{
    /// <summary>
    /// 敏感词检查器
    /// 检测和替换文本中的敏感词
    /// </summary>
    public class SensitiveWordsChecker
    {
        /// <summary>
        /// 敏感词列表
        /// </summary>
        private HashSet<string> sensitiveWords;

        /// <summary>
        /// 敏感词模式（用于正则匹配）
        /// </summary>
        private List<Regex> sensitivePatterns;

        /// <summary>
        /// 敏感词分类
        /// </summary>
        private Dictionary<string, List<string>> wordsByCategory;

        /// <summary>
        /// 是否区分大小写
        /// </summary>
        private bool caseSensitive;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SensitiveWordsChecker()
        {
            sensitiveWords = new HashSet<string>();
            sensitivePatterns = new List<Regex>();
            wordsByCategory = new Dictionary<string, List<string>>();
            caseSensitive = false;

            InitializeDefaultWords();
        }

        #region 初始化

        /// <summary>
        /// 初始化默认敏感词
        /// </summary>
        private void InitializeDefaultWords()
        {
            AddCategory("political", new[]
            {
                "台独", "港独", "藏独", "疆独",
                "分裂", "反动", "颠覆"
            });

            AddCategory("violence", new[]
            {
                "暴力", "恐怖", "袭击", "杀害",
                "武器", "炸弹", "枪支"
            });

            AddCategory("adult", new[]
            {
                "色情", "裸体", "性感",
                "成人内容"
            });

            AddCategory("gambling", new[]
            {
                "赌博", "赌场", "博彩",
                "投注", "下注"
            });

            AddCategory("discrimination", new[]
            {
                "歧视", "偏见",
                "种族主义"
            });
        }

        #endregion

        #region 敏感词管理

        /// <summary>
        /// 添加敏感词
        /// </summary>
        /// <param name="word">敏感词</param>
        public void AddWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return;

            string processedWord = caseSensitive ? word : word.ToLower();
            sensitiveWords.Add(processedWord);
        }

        /// <summary>
        /// 添加敏感词列表
        /// </summary>
        /// <param name="words">敏感词列表</param>
        public void AddWords(IEnumerable<string> words)
        {
            foreach (string word in words)
            {
                AddWord(word);
            }
        }

        /// <summary>
        /// 添加分类敏感词
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="words">敏感词列表</param>
        public void AddCategory(string category, IEnumerable<string> words)
        {
            if (!wordsByCategory.ContainsKey(category))
            {
                wordsByCategory[category] = new List<string>();
            }

            foreach (string word in words)
            {
                if (!wordsByCategory[category].Contains(word))
                {
                    wordsByCategory[category].Add(word);
                    AddWord(word);
                }
            }
        }

        /// <summary>
        /// 移除敏感词
        /// </summary>
        /// <param name="word">敏感词</param>
        public void RemoveWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return;

            string processedWord = caseSensitive ? word : word.ToLower();
            sensitiveWords.Remove(processedWord);

            foreach (var wordList in wordsByCategory.Values)
            {
                wordList.Remove(word);
            }
        }

        /// <summary>
        /// 清除所有敏感词
        /// </summary>
        public void ClearAll()
        {
            sensitiveWords.Clear();
            wordsByCategory.Clear();
            sensitivePatterns.Clear();
        }

        /// <summary>
        /// 获取敏感词数量
        /// </summary>
        /// <returns>数量</returns>
        public int GetWordCount()
        {
            return sensitiveWords.Count;
        }

        #endregion

        #region 检测操作

        /// <summary>
        /// 检查是否包含敏感词
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>是否包含</returns>
        public bool ContainsSensitiveWords(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            string processedText = caseSensitive ? text : text.ToLower();

            foreach (string word in sensitiveWords)
            {
                if (processedText.Contains(word))
                {
                    return true;
                }
            }

            foreach (Regex pattern in sensitivePatterns)
            {
                if (pattern.IsMatch(processedText))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 查找所有敏感词
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>敏感词列表</returns>
        public List<string> FindSensitiveWords(string text)
        {
            List<string> found = new List<string>();

            if (string.IsNullOrEmpty(text)) return found;

            string processedText = caseSensitive ? text : text.ToLower();

            foreach (string word in sensitiveWords)
            {
                if (processedText.Contains(word) && !found.Contains(word))
                {
                    found.Add(word);
                }
            }

            foreach (Regex pattern in sensitivePatterns)
            {
                MatchCollection matches = pattern.Matches(processedText);
                foreach (Match match in matches)
                {
                    if (!found.Contains(match.Value))
                    {
                        found.Add(match.Value);
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// 获取敏感词分类统计
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>分类统计字典</returns>
        public Dictionary<string, int> GetCategoryStatistics(string text)
        {
            Dictionary<string, int> stats = new Dictionary<string, int>();

            foreach (var category in wordsByCategory.Keys)
            {
                stats[category] = 0;
            }

            List<string> found = FindSensitiveWords(text);
            foreach (string word in found)
            {
                foreach (var kvp in wordsByCategory)
                {
                    if (kvp.Value.Contains(word))
                    {
                        stats[kvp.Key]++;
                    }
                }
            }

            return stats;
        }

        #endregion

        #region 替换操作

        /// <summary>
        /// 替换敏感词
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replacement">替换字符</param>
        /// <returns>替换后的文本</returns>
        public string ReplaceSensitiveWords(string text, char replacement = '*')
        {
            if (string.IsNullOrEmpty(text)) return text;

            string result = text;

            foreach (string word in sensitiveWords)
            {
                string replacePattern = new string(replacement, word.Length);
                result = caseSensitive
                    ? result.Replace(word, replacePattern)
                    : result.Replace(word, replacePattern, StringComparison.OrdinalIgnoreCase);
            }

            foreach (Regex pattern in sensitivePatterns)
            {
                result = pattern.Replace(result, new string(replacement, pattern.GetGroupNumbers().Length));
            }

            return result;
        }

        /// <summary>
        /// 替换敏感词（带星号数量控制）
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="replacement">替换字符</param>
        /// <param name="minStars">最少星号</param>
        /// <param name="maxStars">最多星号</param>
        /// <returns>替换后的文本</returns>
        public string ReplaceSensitiveWords(string text, char replacement, int minStars, int maxStars)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string result = text;

            foreach (string word in sensitiveWords)
            {
                int starCount = System.Math.Clamp(word.Length, minStars, maxStars);
                string replacePattern = new string(replacement, starCount);
                result = caseSensitive
                    ? result.Replace(word, replacePattern)
                    : result.Replace(word, replacePattern, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        #endregion

        #region 模式管理

        /// <summary>
        /// 添加正则模式
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public void AddPattern(string pattern)
        {
            try
            {
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                sensitivePatterns.Add(regex);
            }
            catch
            {
                // Invalid pattern, ignore
            }
        }

        /// <summary>
        /// 移除正则模式
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public void RemovePattern(string pattern)
        {
            sensitivePatterns.RemoveAll(r => r.ToString() == pattern);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取所有敏感词
        /// </summary>
        /// <returns>敏感词列表</returns>
        public List<string> GetAllWords()
        {
            return new List<string>(sensitiveWords);
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <returns>分类列表</returns>
        public List<string> GetCategories()
        {
            return new List<string>(wordsByCategory.Keys);
        }

        /// <summary>
        /// 获取指定分类的敏感词
        /// </summary>
        /// <param name="category">分类</param>
        /// <returns>敏感词列表</returns>
        public List<string> GetWordsByCategory(string category)
        {
            return wordsByCategory.ContainsKey(category)
                ? new List<string>(wordsByCategory[category])
                : new List<string>();
        }

        #endregion
    }
}
