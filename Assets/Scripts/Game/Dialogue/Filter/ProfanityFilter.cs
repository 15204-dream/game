using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace 糟糕是心动鸭.Dialogue.Filter
{
    /// <summary>
    /// 脏话过滤器
    /// 检测和过滤不当语言
    /// </summary>
    public class ProfanityFilter
    {
        /// <summary>
        /// 脏话词库
        /// </summary>
        private HashSet<string> profanityWords;

        /// <summary>
        /// 脏话模式
        /// </summary>
        private List<Regex> profanityPatterns;

        /// <summary>
        /// 替换规则
        /// </summary>
        private Dictionary<string, string> replacementRules;

        /// <summary>
        /// 过滤器模式
        /// </summary>
        private ProfanityFilterMode mode;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProfanityFilter()
        {
            profanityWords = new HashSet<string>();
            profanityPatterns = new List<Regex>();
            replacementRules = new Dictionary<string, string>();
            mode = ProfanityFilterMode.Replace;

            InitializeDefaultProfanity();
        }

        #region 初始化

        /// <summary>
        /// 初始化默认脏话库
        /// </summary>
        private void InitializeDefaultProfanity()
        {
            string[] mildProfanity = new[]
            {
                "笨蛋", "傻瓜", "白痴", "傻子",
                "神经病", "有病", "智障",
                "废物", "垃圾"
            };

            string[] moderateProfanity = new[]
            {
                "恶心", "讨厌", "烦人",
                "无聊", "虚伪", "假惺惺"
            };

            foreach (string word in mildProfanity)
            {
                AddWord(word, GetMildReplacement(word));
            }

            foreach (string word in moderateProfanity)
            {
                AddWord(word, GetModerateReplacement(word));
            }
        }

        /// <summary>
        /// 获取轻度替换词
        /// </summary>
        private string GetMildReplacement(string word)
        {
            return "可爱";
        }

        /// <summary>
        /// 获取中度替换词
        /// </summary>
        private string GetModerateReplacement(string word)
        {
            return "有趣";
        }

        #endregion

        #region 脏话管理

        /// <summary>
        /// 添加脏话词
        /// </summary>
        /// <param name="word">脏话词</param>
        /// <param name="replacement">替换词</param>
        public void AddWord(string word, string replacement = null)
        {
            if (string.IsNullOrEmpty(word)) return;

            string lowerWord = word.ToLower();
            profanityWords.Add(lowerWord);

            if (!string.IsNullOrEmpty(replacement))
            {
                replacementRules[lowerWord] = replacement;
            }
        }

        /// <summary>
        /// 添加脏话词列表
        /// </summary>
        /// <param name="words">脏话词列表</param>
        public void AddWords(IEnumerable<string> words)
        {
            foreach (string word in words)
            {
                AddWord(word);
            }
        }

        /// <summary>
        /// 移除脏话词
        /// </summary>
        /// <param name="word">脏话词</param>
        public void RemoveWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return;

            string lowerWord = word.ToLower();
            profanityWords.Remove(lowerWord);
            replacementRules.Remove(lowerWord);
        }

        /// <summary>
        /// 清除所有脏话词
        /// </summary>
        public void ClearAll()
        {
            profanityWords.Clear();
            replacementRules.Clear();
            profanityPatterns.Clear();
        }

        /// <summary>
        /// 设置过滤模式
        /// </summary>
        /// <param name="newMode">新模式</param>
        public void SetMode(ProfanityFilterMode newMode)
        {
            mode = newMode;
        }

        /// <summary>
        /// 获取脏话词数量
        /// </summary>
        /// <returns>数量</returns>
        public int GetWordCount()
        {
            return profanityWords.Count;
        }

        #endregion

        #region 检测操作

        /// <summary>
        /// 检查是否包含脏话
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>是否包含</returns>
        public bool ContainsProfanity(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            string lowerText = text.ToLower();

            foreach (string word in profanityWords)
            {
                if (lowerText.Contains(word))
                {
                    return true;
                }
            }

            foreach (Regex pattern in profanityPatterns)
            {
                if (pattern.IsMatch(lowerText))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 查找所有脏话
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>脏话列表</returns>
        public List<string> FindProfanity(string text)
        {
            List<string> found = new List<string>();

            if (string.IsNullOrEmpty(text)) return found;

            string lowerText = text.ToLower();

            foreach (string word in profanityWords)
            {
                if (lowerText.Contains(word))
                {
                    found.Add(word);
                }
            }

            foreach (Regex pattern in profanityPatterns)
            {
                MatchCollection matches = pattern.Matches(lowerText);
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
        /// 获取脏话数量
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>数量</returns>
        public int CountProfanity(string text)
        {
            return FindProfanity(text).Count;
        }

        #endregion

        #region 过滤操作

        /// <summary>
        /// 过滤脏话
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <returns>过滤后的文本</returns>
        public string Filter(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            switch (mode)
            {
                case ProfanityFilterMode.Replace:
                    return ReplaceProfanity(text);

                case ProfanityFilterMode.Remove:
                    return RemoveProfanity(text);

                case ProfanityFilterMode.Censor:
                    return CensorProfanity(text);

                case ProfanityFilterMode.None:
                    return text;

                default:
                    return text;
            }
        }

        /// <summary>
        /// 替换脏话
        /// </summary>
        private string ReplaceProfanity(string text)
        {
            string result = text;

            foreach (string word in profanityWords)
            {
                string replacement = replacementRules.ContainsKey(word)
                    ? replacementRules[word]
                    : GetDefaultReplacement(word);

                result = Regex.Replace(
                    result,
                    Regex.Escape(word),
                    replacement,
                    RegexOptions.IgnoreCase
                );
            }

            return result;
        }

        /// <summary>
        /// 移除脏话
        /// </summary>
        private string RemoveProfanity(string text)
        {
            string result = text;

            foreach (string word in profanityWords)
            {
                result = Regex.Replace(
                    result,
                    Regex.Escape(word),
                    "",
                    RegexOptions.IgnoreCase
                );
            }

            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                @"\s+",
                " "
            ).Trim();

            return result;
        }

        /// <summary>
        /// 审查脏话（用星号替换）
        /// </summary>
        private string CensorProfanity(string text)
        {
            string result = text;

            foreach (string word in profanityWords)
            {
                string replacement = new string('*', word.Length);

                result = Regex.Replace(
                    result,
                    Regex.Escape(word),
                    replacement,
                    RegexOptions.IgnoreCase
                );
            }

            return result;
        }

        /// <summary>
        /// 获取默认替换词
        /// </summary>
        private string GetDefaultReplacement(string word)
        {
            return "[已过滤]";
        }

        #endregion

        #region 模式管理

        /// <summary>
        /// 添加脏话模式
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public void AddPattern(string pattern)
        {
            try
            {
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                profanityPatterns.Add(regex);
            }
            catch
            {
                // Invalid pattern
            }
        }

        /// <summary>
        /// 移除脏话模式
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public void RemovePattern(string pattern)
        {
            profanityPatterns.RemoveAll(r => r.ToString() == pattern);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取所有脏话词
        /// </summary>
        /// <returns>脏话词列表</returns>
        public List<string> GetAllWords()
        {
            return new List<string>(profanityWords);
        }

        /// <summary>
        /// 获取替换规则
        /// </summary>
        /// <param name="word">脏话词</param>
        /// <returns>替换词</returns>
        public string GetReplacement(string word)
        {
            string lowerWord = word.ToLower();
            return replacementRules.ContainsKey(lowerWord)
                ? replacementRules[lowerWord]
                : GetDefaultReplacement(word);
        }

        /// <summary>
        /// 获取过滤统计
        /// </summary>
        /// <returns>统计信息</returns>
        public ProfanityFilterStatistics GetStatistics()
        {
            return new ProfanityFilterStatistics
            {
                WordCount = profanityWords.Count,
                PatternCount = profanityPatterns.Count,
                ReplacementRuleCount = replacementRules.Count,
                CurrentMode = mode
            };
        }

        #endregion
    }

    /// <summary>
    /// 脏话过滤器模式
    /// </summary>
    public enum ProfanityFilterMode
    {
        /// <summary>
        /// 不进行过滤
        /// </summary>
        None,

        /// <summary>
        /// 替换脏话
        /// </summary>
        Replace,

        /// <summary>
        /// 移除脏话
        /// </summary>
        Remove,

        /// <summary>
        /// 审查脏话（星号）
        /// </summary>
        Censor
    }

    /// <summary>
    /// 脏话过滤器统计
    /// </summary>
    [Serializable]
    public class ProfanityFilterStatistics
    {
        /// <summary>
        /// 脏话词数量
        /// </summary>
        public int WordCount;

        /// <summary>
        /// 模式数量
        /// </summary>
        public int PatternCount;

        /// <summary>
        /// 替换规则数量
        /// </summary>
        public int ReplacementRuleCount;

        /// <summary>
        /// 当前模式
        /// </summary>
        public ProfanityFilterMode CurrentMode;
    }
}
