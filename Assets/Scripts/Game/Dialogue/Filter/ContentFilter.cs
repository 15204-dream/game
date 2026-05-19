using System;
using System.Collections.Generic;

namespace 糟糕是心动鸭.Dialogue.Filter
{
    /// <summary>
    /// 内容过滤器
    /// 对AI生成的对话内容进行过滤和检查
    /// </summary>
    public class ContentFilter
    {
        /// <summary>
        /// 敏感词检查器
        /// </summary>
        private SensitiveWordsChecker sensitiveWordsChecker;

        /// <summary>
        /// 脏话过滤器
        /// </summary>
        private ProfanityFilter profanityFilter;

        /// <summary>
        /// 过滤配置
        /// </summary>
        private FilterConfig config;

        /// <summary>
        /// 过滤统计
        /// </summary>
        private FilterStatistics statistics;

        /// <summary>
        /// 是否启用
        /// </summary>
        private bool isEnabled;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ContentFilter()
        {
            sensitiveWordsChecker = new SensitiveWordsChecker();
            profanityFilter = new ProfanityFilter();
            config = new FilterConfig();
            statistics = new FilterStatistics();
            isEnabled = true;
        }

        #endregion

        #region 过滤操作

        /// <summary>
        /// 过滤内容
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <returns>过滤后的内容</returns>
        public string FilterContent(string content)
        {
            if (!isEnabled || string.IsNullOrEmpty(content))
            {
                return content;
            }

            string filtered = content;

            if (config.EnableProfanityFilter)
            {
                filtered = profanityFilter.Filter(filtered);
                if (filtered != content)
                {
                    statistics.RecordProfanityReplacement();
                }
            }

            if (config.EnableSensitiveWordsCheck)
            {
                filtered = sensitiveWordsChecker.ReplaceSensitiveWords(filtered, config.ReplacementChar);
                if (filtered != content)
                {
                    statistics.RecordSensitiveWordReplacement();
                }
            }

            if (config.EnableLengthCheck)
            {
                filtered = TruncateContent(filtered);
            }

            statistics.RecordFiltering();

            return filtered;
        }

        /// <summary>
        /// 检查内容是否安全
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>是否安全</returns>
        public bool IsContentSafe(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return true;
            }

            bool hasProfanity = profanityFilter.ContainsProfanity(content);
            bool hasSensitiveWords = sensitiveWordsChecker.ContainsSensitiveWords(content);

            return !hasProfanity && !hasSensitiveWords;
        }

        /// <summary>
        /// 获取内容安全报告
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>安全报告</returns>
        public ContentSafetyReport GetSafetyReport(string content)
        {
            ContentSafetyReport report = new ContentSafetyReport
            {
                OriginalContent = content,
                IsSafe = true,
                Issues = new List<string>(),
                Warnings = new List<string>()
            };

            if (string.IsNullOrEmpty(content))
            {
                return report;
            }

            if (profanityFilter.ContainsProfanity(content))
            {
                report.IsSafe = false;
                report.Issues.Add("包含不当语言");
                statistics.RecordProfanityFound();
            }

            List<string> foundSensitive = sensitiveWordsChecker.FindSensitiveWords(content);
            if (foundSensitive.Count > 0)
            {
                report.Warnings.Add($"包含{foundSensitive.Count}个敏感词");
                statistics.RecordSensitiveWordFound(foundSensitive.Count);
            }

            if (content.Length > config.MaxLength)
            {
                report.Warnings.Add("内容过长");
            }

            return report;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 截断内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>截断后的内容</returns>
        private string TruncateContent(string content)
        {
            if (string.IsNullOrEmpty(content) || content.Length <= config.MaxLength)
            {
                return content;
            }

            int truncateIndex = config.MaxLength;

            if (config.TruncateAtSentence)
            {
                for (int i = config.MaxLength - 1; i >= 0; i--)
                {
                    if (content[i] == '。' || content[i] == '！' || content[i] == '？')
                    {
                        truncateIndex = i + 1;
                        break;
                    }
                }
            }

            return content.Substring(0, truncateIndex);
        }

        #endregion

        #region 配置管理

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="newConfig">新配置</param>
        public void SetConfig(FilterConfig newConfig)
        {
            config = newConfig ?? new FilterConfig();
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns>当前配置</returns>
        public FilterConfig GetConfig()
        {
            return config;
        }

        /// <summary>
        /// 启用过滤器
        /// </summary>
        public void Enable()
        {
            isEnabled = true;
        }

        /// <summary>
        /// 禁用过滤器
        /// </summary>
        public void Disable()
        {
            isEnabled = false;
        }

        /// <summary>
        /// 设置过滤等级
        /// </summary>
        /// <param name="level">等级</param>
        public void SetFilterLevel(FilterLevel level)
        {
            switch (level)
            {
                case FilterLevel.Strict:
                    config.EnableProfanityFilter = true;
                    config.EnableSensitiveWordsCheck = true;
                    config.EnableLengthCheck = true;
                    config.MaxLength = 200;
                    break;

                case FilterLevel.Moderate:
                    config.EnableProfanityFilter = true;
                    config.EnableSensitiveWordsCheck = true;
                    config.EnableLengthCheck = false;
                    break;

                case FilterLevel.Lenient:
                    config.EnableProfanityFilter = true;
                    config.EnableSensitiveWordsCheck = false;
                    config.EnableLengthCheck = false;
                    break;

                case FilterLevel.None:
                    config.EnableProfanityFilter = false;
                    config.EnableSensitiveWordsCheck = false;
                    config.EnableLengthCheck = false;
                    break;
            }
        }

        #endregion

        #region 统计信息

        /// <summary>
        /// 获取统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        public FilterStatistics GetStatistics()
        {
            return statistics;
        }

        /// <summary>
        /// 重置统计
        /// </summary>
        public void ResetStatistics()
        {
            statistics = new FilterStatistics();
        }

        #endregion
    }

    /// <summary>
    /// 过滤配置
    /// </summary>
    [Serializable]
    public class FilterConfig
    {
        /// <summary>
        /// 启用脏话过滤
        /// </summary>
        public bool EnableProfanityFilter = true;

        /// <summary>
        /// 启用敏感词检查
        /// </summary>
        public bool EnableSensitiveWordsCheck = true;

        /// <summary>
        /// 启用长度检查
        /// </summary>
        public bool EnableLengthCheck = true;

        /// <summary>
        /// 最大内容长度
        /// </summary>
        public int MaxLength = 500;

        /// <summary>
        /// 是否在句子处截断
        /// </summary>
        public bool TruncateAtSentence = true;

        /// <summary>
        /// 替换字符
        /// </summary>
        public char ReplacementChar = '*';
    }

    /// <summary>
    /// 过滤等级
    /// </summary>
    public enum FilterLevel
    {
        None,
        Lenient,
        Moderate,
        Strict
    }

    /// <summary>
    /// 内容安全报告
    /// </summary>
    [Serializable]
    public class ContentSafetyReport
    {
        /// <summary>
        /// 原始内容
        /// </summary>
        public string OriginalContent;

        /// <summary>
        /// 是否安全
        /// </summary>
        public bool IsSafe;

        /// <summary>
        /// 问题列表
        /// </summary>
        public List<string> Issues;

        /// <summary>
        /// 警告列表
        /// </summary>
        public List<string> Warnings;

        /// <summary>
        /// 获取问题数量
        /// </summary>
        public int IssueCount => Issues?.Count ?? 0;

        /// <summary>
        /// 获取警告数量
        /// </summary>
        public int WarningCount => Warnings?.Count ?? 0;
    }

    /// <summary>
    /// 过滤统计
    /// </summary>
    [Serializable]
    public class FilterStatistics
    {
        /// <summary>
        /// 总过滤次数
        /// </summary>
        public int TotalFiltered;

        /// <summary>
        /// 脏话替换次数
        /// </summary>
        public int ProfanityReplacements;

        /// <summary>
        /// 敏感词替换次数
        /// </summary>
        public int SensitiveWordReplacements;

        /// <summary>
        /// 发现脏话次数
        /// </summary>
        public int ProfanityFoundCount;

        /// <summary>
        /// 发现敏感词次数
        /// </summary>
        public int SensitiveWordsFoundCount;

        /// <summary>
        /// 记录过滤
        /// </summary>
        public void RecordFiltering()
        {
            TotalFiltered++;
        }

        /// <summary>
        /// 记录脏话替换
        /// </summary>
        public void RecordProfanityReplacement()
        {
            ProfanityReplacements++;
        }

        /// <summary>
        /// 记录敏感词替换
        /// </summary>
        public void RecordSensitiveWordReplacement()
        {
            SensitiveWordReplacements++;
        }

        /// <summary>
        /// 记录发现脏话
        /// </summary>
        public void RecordProfanityFound()
        {
            ProfanityFoundCount++;
        }

        /// <summary>
        /// 记录发现敏感词
        /// </summary>
        /// <param name="count">数量</param>
        public void RecordSensitiveWordFound(int count)
        {
            SensitiveWordsFoundCount += count;
        }

        /// <summary>
        /// 获取脏话替换率
        /// </summary>
        /// <returns>替换率</returns>
        public float GetProfanityReplacementRate()
        {
            return TotalFiltered > 0 ? (float)ProfanityReplacements / TotalFiltered : 0f;
        }

        /// <summary>
        /// 获取敏感词替换率
        /// </summary>
        /// <returns>替换率</returns>
        public float GetSensitiveWordReplacementRate()
        {
            return TotalFiltered > 0 ? (float)SensitiveWordReplacements / TotalFiltered : 0f;
        }
    }
}
