using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace 糟糕是心动鸭.Dialogue.DirectorMode
{
    /// <summary>
    /// 剧情生成器
    /// 使用AI生成恋综节目剧情
    /// </summary>
    public class PlotGenerator
    {
        /// <summary>
        /// API客户端
        /// </summary>
        private API.APIClient apiClient;

        /// <summary>
        /// 剧情模板
        /// </summary>
        private Dictionary<DirectorActionType, PlotTemplate> templates;

        /// <summary>
        /// 生成缓存
        /// </summary>
        private Dictionary<string, PlotEvent> eventCache;

        /// <summary>
        /// 最大缓存大小
        /// </summary>
        private const int MAX_CACHE_SIZE = 30;

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PlotGenerator()
        {
            apiClient = API.APIClient.Instance;
            eventCache = new Dictionary<string, PlotEvent>();
            InitializeTemplates();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化剧情模板
        /// </summary>
        private void InitializeTemplates()
        {
            templates = new Dictionary<DirectorActionType, PlotTemplate>();

            templates[DirectorActionType.ScheduleActivity] = new PlotTemplate
            {
                Type = DirectorActionType.ScheduleActivity,
                TitleTemplate = "{0}活动安排",
                DescriptionTemplate = "导演组安排了{0}环节，{1}将参与其中",
                HeatImpact = 15f
            };

            templates[DirectorActionType.ArrangeDate] = new PlotTemplate
            {
                Type = DirectorActionType.ArrangeDate,
                TitleTemplate = "{0}的约会",
                DescriptionTemplate = "特别约会环节：{0}和{1}将单独相处",
                HeatImpact = 25f
            };

            templates[DirectorActionType.CreateConflict] = new PlotTemplate
            {
                Type = DirectorActionType.CreateConflict,
                TitleTemplate = "矛盾冲突",
                DescriptionTemplate = "{0}和{1}之间产生了微妙的竞争",
                HeatImpact = 20f
            };

            templates[DirectorActionType.TriggerConfession] = new PlotTemplate
            {
                Type = DirectorActionType.TriggerConfession,
                TitleTemplate = "心动时刻",
                DescriptionTemplate = "{0}似乎对{1}有了特别的感觉...",
                HeatImpact = 35f
            };

            templates[DirectorActionType.SurpriseEvent] = new PlotTemplate
            {
                Type = DirectorActionType.SurpriseEvent,
                TitleTemplate = "惊喜环节",
                DescriptionTemplate = "意想不到的惊喜降临：{0}",
                HeatImpact = 30f
            };

            templates[DirectorActionType.PrivateConversation] = new PlotTemplate
            {
                Type = DirectorActionType.PrivateConversation,
                TitleTemplate = "私下谈话",
                DescriptionTemplate = "{0}和{1}找了个安静的地方聊天",
                HeatImpact = 10f
            };

            templates[DirectorActionType.GroupActivity] = new PlotTemplate
            {
                Type = DirectorActionType.GroupActivity,
                TitleTemplate = "集体活动",
                DescriptionTemplate = "所有嘉宾参与：{0}",
                HeatImpact = 12f
            };

            templates[DirectorActionType.RevealSecret] = new PlotTemplate
            {
                Type = DirectorActionType.RevealSecret,
                TitleTemplate = "秘密曝光",
                DescriptionTemplate = "{0}的秘密被揭露：{1}",
                HeatImpact = 28f
            };
        }

        #endregion

        #region 剧情生成

        /// <summary>
        /// 生成剧情事件
        /// </summary>
        /// <param name="action">导演行动</param>
        /// <param name="showData">节目数据</param>
        /// <param name="round">当前回合</param>
        /// <returns>剧情事件</returns>
        public async Task<PlotEvent> GeneratePlotEvent(DirectorAction action, DirectorPlayerData showData, int round)
        {
            try
            {
                string cacheKey = GenerateCacheKey(action, round);
                if (eventCache.TryGetValue(cacheKey, out PlotEvent cached))
                {
                    return cached;
                }

                if (apiClient != null && round % 2 == 0)
                {
                    string aiPlot = await GenerateAIPlot(action, showData, round);
                    PlotEvent eventData = ParseAIPlot(aiPlot, action);
                    AddToCache(cacheKey, eventData);
                    return eventData;
                }

                PlotEvent templateEvent = GenerateFromTemplate(action, showData, round);
                AddToCache(cacheKey, templateEvent);
                return templateEvent;
            }
            catch (Exception e)
            {
                Debug.LogError($"Plot generation error: {e.Message}");
                return GenerateFallbackEvent(action, round);
            }
        }

        /// <summary>
        /// 生成AI剧情
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="showData">数据</param>
        /// <param name="round">回合</param>
        /// <returns>剧情描述</returns>
        private async Task<string> GenerateAIPlot(DirectorAction action, DirectorPlayerData showData, int round)
        {
            string prompt = BuildPlotPrompt(action, showData, round);
            return await apiClient.SendRequestAsync(prompt);
        }

        /// <summary>
        /// 构建剧情提示词
        /// </summary>
        <param name="action">行动</param>
        <param name="showData">数据</param>
        <param name="round">回合</param>
        /// <returns>提示词</returns>
        private string BuildPlotPrompt(DirectorAction action, DirectorPlayerData showData, int round)
        {
            StringBuilder prompt = new StringBuilder();

            prompt.AppendLine("你是《糟糕！是心动鸭！》恋综节目的导演AI。");
            prompt.AppendLine("请根据以下信息生成一个有趣的节目剧情事件。");
            prompt.AppendLine();
            prompt.AppendLine($"当前回合: {round}");
            prompt.AppendLine($"行动类型: {action.ActionType}");
            prompt.AppendLine($"行动描述: {action.Description}");
            prompt.AppendLine();

            if (showData != null)
            {
                prompt.AppendLine($"节目名称: {showData.ShowName}");
                prompt.AppendLine($"嘉宾数量: {showData.CharacterCount}");
            }

            prompt.AppendLine();
            prompt.AppendLine("请生成一个简短有趣的剧情描述（50-100字），包括：");
            prompt.AppendLine("1. 事件标题");
            prompt.AppendLine("2. 详细描述");
            prompt.AppendLine("3. 涉及的角色互动");

            return prompt.ToString();
        }

        /// <summary>
        /// 解析AI生成的剧情
        /// </summary>
        /// <param name="rawPlot">原始剧情</param>
        /// <param name="action">行动</param>
        /// <returns>剧情事件</returns>
        private PlotEvent ParseAIPlot(string rawPlot, DirectorAction action)
        {
            PlotEvent eventData = new PlotEvent
            {
                EventId = Guid.NewGuid().ToString(),
                Title = ExtractTitle(rawPlot),
                Description = rawPlot,
                Phase = DeterminePhase(action),
                Effects = new Dictionary<string, float>()
            };

            eventData.Effects["heat"] = templates.ContainsKey(action.ActionType)
                ? templates[action.ActionType].HeatImpact
                : 15f;

            return eventData;
        }

        /// <summary>
        /// 从模板生成
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="showData">数据</param>
        /// <param name="round">回合</param>
        /// <returns>剧情事件</returns>
        private PlotEvent GenerateFromTemplate(DirectorAction action, DirectorPlayerData showData, int round)
        {
            if (!templates.ContainsKey(action.ActionType))
            {
                return GenerateFallbackEvent(action, round);
            }

            PlotTemplate template = templates[action.ActionType];

            string title = string.Format(template.TitleTemplate, GetTemplateParam(action, 0));
            string description = string.Format(template.DescriptionTemplate,
                GetTemplateParam(action, 0),
                GetTemplateParam(action, 1));

            return new PlotEvent
            {
                EventId = Guid.NewGuid().ToString(),
                Title = title,
                Description = description,
                Phase = DeterminePhase(action),
                HeatImpact = template.HeatImpact
            };
        }

        /// <summary>
        /// 获取模板参数
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="index">索引</param>
        /// <returns>参数值</returns>
        private string GetTemplateParam(DirectorAction action, int index)
        {
            if (action.Parameters.Count > index)
            {
                int i = 0;
                foreach (var param in action.Parameters.Values)
                {
                    if (i == index) return param;
                    i++;
                }
            }

            string[] defaults = { "神秘嘉宾", "另一位嘉宾" };
            return defaults[index];
        }

        /// <summary>
        /// 生成备用事件
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="round">回合</param>
        /// <returns>剧情事件</returns>
        private PlotEvent GenerateFallbackEvent(DirectorAction action, int round)
        {
            return new PlotEvent
            {
                EventId = Guid.NewGuid().ToString(),
                Title = $"回合{round}事件",
                Description = $"导演安排了{action.Description}",
                Phase = PlotPhase.Development
            };
        }

        /// <summary>
        /// 提取标题
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns>标题</returns>
        private string ExtractTitle(string text)
        {
            if (string.IsNullOrEmpty(text)) return "无标题事件";

            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length < 30)
                {
                    return trimmed;
                }
            }

            return text.Length > 20 ? text.Substring(0, 20) + "..." : text;
        }

        /// <summary>
        /// 确定阶段
        /// </summary>
        /// <param name="action">行动</param>
        /// <returns>阶段</returns>
        private PlotPhase DeterminePhase(DirectorAction action)
        {
            return action.ActionType switch
            {
                DirectorActionType.TriggerConfession => PlotPhase.Climax,
                DirectorActionType.SurpriseEvent => PlotPhase.Climax,
                DirectorActionType.CreateConflict => PlotPhase.Development,
                _ => PlotPhase.Development
            };
        }

        #endregion

        #region 特殊事件

        /// <summary>
        /// 生成特殊事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="showData">节目数据</param>
        /// <returns>剧情事件</returns>
        public async Task<PlotEvent> GenerateSpecialEvent(string eventId, DirectorPlayerData showData)
        {
            await Task.Delay(1);

            return new PlotEvent
            {
                EventId = eventId,
                Title = "特殊事件",
                Description = "触发了特殊剧情事件",
                Phase = PlotPhase.Climax
            };
        }

        #endregion

        #region 缓存管理

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="round">回合</param>
        /// <returns>缓存键</returns>
        private string GenerateCacheKey(DirectorAction action, int round)
        {
            return $"{action.ActionType}_{round}";
        }

        /// <summary>
        /// 添加到缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="eventData">事件</param>
        private void AddToCache(string key, PlotEvent eventData)
        {
            if (eventCache.Count >= MAX_CACHE_SIZE)
            {
                RemoveOldestCache();
            }
            eventCache[key] = eventData;
        }

        /// <summary>
        /// 移除最旧的缓存
        /// </summary>
        private void RemoveOldestCache()
        {
            if (eventCache.Count == 0) return;

            var firstKey = System.Linq.Enumerable.First(eventCache.Keys);
            eventCache.Remove(firstKey);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            eventCache.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 剧情模板
    /// </summary>
    [Serializable]
    public class PlotTemplate
    {
        public DirectorActionType Type;
        public string TitleTemplate;
        public string DescriptionTemplate;
        public float HeatImpact;
    }
}
