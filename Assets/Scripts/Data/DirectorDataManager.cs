using System;
using System.Collections.Generic;
using UnityEngine;

namespace DatingSim.Scripts.Data
{
    /// <summary>
    /// 导演模式数据管理器 - 管理导演模式的所有数据操作
    /// </summary>
    public class DirectorDataManager
    {
        private DataManager dataManager;

        public DirectorDataManager(DataManager dm)
        {
            dataManager = dm;
        }

        #region 导演能力管理

        /// <summary>
        /// 升级导演能力
        /// </summary>
        public bool UpgradeAbility(DirectorAbilityType abilityType)
        {
            var ability = dataManager.directorPlayerData.abilities.Find(a => a.abilityType == abilityType);
            if (ability == null || !ability.isUnlocked)
            {
                return false;
            }

            if (dataManager.directorPlayerData.availableAbilityPoints <= 0)
            {
                return false;
            }

            if (ability.level >= ability.maxLevel)
            {
                return false;
            }

            ability.level++;
            dataManager.directorPlayerData.availableAbilityPoints--;
            dataManager.directorPlayerData.totalAbilityPoints++;

            OnAbilityUpgraded?.Invoke(abilityType, ability.level);
            return true;
        }

        /// <summary>
        /// 解锁导演能力
        /// </summary>
        public bool UnlockAbility(DirectorAbilityType abilityType)
        {
            var ability = dataManager.directorPlayerData.abilities.Find(a => a.abilityType == abilityType);
            if (ability == null || ability.isUnlocked)
            {
                return false;
            }

            ability.isUnlocked = true;
            ability.level = 1;

            OnAbilityUnlocked?.Invoke(abilityType);
            return true;
        }

        /// <summary>
        /// 添加经验值
        /// </summary>
        public void AddExperience(int exp)
        {
            dataManager.directorPlayerData.experience += exp;
            int newLevel = DirectorPlayerHelper.CalculateDirectorLevel(dataManager.directorPlayerData.experience);

            if (newLevel > dataManager.directorPlayerData.directorLevel)
            {
                dataManager.directorPlayerData.directorLevel = newLevel;
                dataManager.directorPlayerData.availableAbilityPoints += 3;

                OnDirectorLevelUp?.Invoke(newLevel);
            }
        }

        /// <summary>
        /// 获取能力等级
        /// </summary>
        public int GetAbilityLevel(DirectorAbilityType abilityType)
        {
            var ability = dataManager.directorPlayerData.abilities.Find(a => a.abilityType == abilityType);
            return ability?.level ?? 0;
        }

        /// <summary>
        /// 能力升级事件
        /// </summary>
        public event Action<DirectorAbilityType, int> OnAbilityUpgraded;

        /// <summary>
        /// 能力解锁事件
        /// </summary>
        public event Action<DirectorAbilityType> OnAbilityUnlocked;

        /// <summary>
        /// 导演等级提升事件
        /// </summary>
        public event Action<int> OnDirectorLevelUp;

        #endregion

        #region 行动点管理

        /// <summary>
        /// 执行行动
        /// </summary>
        public bool PerformAction(ActionData action)
        {
            if (!ActionPointHelper.CanPerformAction(dataManager.actionPointData, action))
            {
                return false;
            }

            int cost = ActionPointHelper.GetActionCost(dataManager.actionPointData, action);

            ActionResultData result = new ActionResultData
            {
                isSuccess = true,
                qualityScore = Random.Range(60, 100),
                audienceImpact = Random.Range(10, 50),
                dramaValue = Random.Range(5, 30)
            };

            ActionPointHelper.PerformAction(dataManager.actionPointData, action, result);

            OnActionPerformed?.Invoke(action.actionId, result);
            return true;
        }

        /// <summary>
        /// 消耗行动点
        /// </summary>
        public bool ConsumeActionPoints(int amount)
        {
            if (dataManager.actionPointData.currentPoints >= amount)
            {
                dataManager.actionPointData.currentPoints -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 恢复行动点
        /// </summary>
        public void RestoreActionPoints(int amount)
        {
            ActionPointHelper.RegeneratePoints(dataManager.actionPointData, amount, RegenerationType.Item);
        }

        /// <summary>
        /// 检查行动是否可用
        /// </summary>
        public bool IsActionAvailable(ActionData action)
        {
            return ActionPointHelper.CanPerformAction(dataManager.actionPointData, action);
        }

        /// <summary>
        /// 获取行动成本
        /// </summary>
        public int GetActionCost(ActionData action)
        {
            return ActionPointHelper.GetActionCost(dataManager.actionPointData, action);
        }

        /// <summary>
        /// 行动执行事件
        /// </summary>
        public event Action<string, ActionResultData> OnActionPerformed;

        #endregion

        #region 热度管理

        /// <summary>
        /// 增加热度
        /// </summary>
        public void AddHeat(int amount, HeatChangeReason reason, string description = "")
        {
            int previousHeat = dataManager.heatData.totalHeat;
            dataManager.heatData.totalHeat += amount;
            dataManager.heatData.dailyHeatGain += amount;

            if (dataManager.heatData.totalHeat > dataManager.heatData.peakHeat)
            {
                dataManager.heatData.peakHeat = dataManager.heatData.totalHeat;
            }

            HeatLevel previousLevel = dataManager.heatData.currentHeatLevel;
            dataManager.heatData.currentHeatLevel = HeatLevelHelper.GetHeatLevel(
                dataManager.heatData.totalHeat,
                dataManager.heatData.config);

            var record = HeatLevelHelper.CreateHeatChangeRecord(previousHeat, dataManager.heatData.totalHeat, reason, description);
            dataManager.heatData.heatHistory.Add(record);

            if (previousLevel != dataManager.heatData.currentHeatLevel)
            {
                OnHeatLevelChanged?.Invoke(previousLevel, dataManager.heatData.currentHeatLevel);
            }

            if (amount > 0)
            {
                OnHeatIncreased?.Invoke(amount);
            }
        }

        /// <summary>
        /// 减少热度
        /// </summary>
        public void ReduceHeat(int amount, HeatChangeReason reason, string description = "")
        {
            int previousHeat = dataManager.heatData.totalHeat;
            dataManager.heatData.totalHeat = Mathf.Max(0, dataManager.heatData.totalHeat - amount);
            dataManager.heatData.dailyHeatLoss += amount;

            var record = HeatLevelHelper.CreateHeatChangeRecord(previousHeat, dataManager.heatData.totalHeat, reason, description);
            dataManager.heatData.heatHistory.Add(record);

            OnHeatDecreased?.Invoke(amount);
        }

        /// <summary>
        /// 发布内容增加热度
        /// </summary>
        public void PublishContent(PlatformPost post)
        {
            int heatGenerated = CalculatePostHeat(post);
            AddHeat(heatGenerated, HeatChangeReason.ViralClip, $"发布内容: {post.content}");
        }

        /// <summary>
        /// 计算帖子热度
        /// </summary>
        private int CalculatePostHeat(PlatformPost post)
        {
            int baseHeat = post.views / 1000;
            int engagementHeat = (post.likes / 100) * 5 + (post.comments / 50) * 10 + (post.shares / 20) * 20;

            if (post.isPromoted)
            {
                engagementHeat = Mathf.RoundToInt(engagementHeat * 1.5f);
            }

            return baseHeat + engagementHeat;
        }

        /// <summary>
        /// 获取当前热度等级名称
        /// </summary>
        public string GetHeatLevelName()
        {
            return HeatLevelHelper.GetHeatLevelName(dataManager.heatData.currentHeatLevel);
        }

        /// <summary>
        /// 热度增加事件
        /// </summary>
        public event Action<int> OnHeatIncreased;

        /// <summary>
        /// 热度减少事件
        /// </summary>
        public event Action<int> OnHeatDecreased;

        /// <summary>
        /// 热度等级变化事件
        /// </summary>
        public event Action<HeatLevel, HeatLevel> OnHeatLevelChanged;

        #endregion

        #region 危机管理

        /// <summary>
        /// 检测潜在危机
        /// </summary>
        public List<CrisisWarning> DetectPotentialCrises()
        {
            List<CrisisWarning> warnings = new List<CrisisWarning>();

            if (dataManager.heatData.averageHeat < dataManager.heatData.config.warmThreshold)
            {
                warnings.Add(new CrisisWarning
                {
                    warningId = Guid.NewGuid().ToString(),
                    crisisId = "low_heat",
                    type = CrisisType.Ratings,
                    message = "热度持续走低，需要采取措施提升观众兴趣",
                    severityLevel = 3,
                    warningTime = DateTime.Now,
                    timeToAct = 1440
                });
            }

            return warnings;
        }

        /// <summary>
        /// 开始处理危机
        /// </summary>
        public bool StartCrisisResolution(string crisisId, string solutionId)
        {
            var crisis = dataManager.crisisData.activeCrises.Find(c => c.crisisId == crisisId);
            if (crisis == null) return false;

            var solution = crisis.possibleSolutions.Find(s => s.solutionId == solutionId);
            if (solution == null || solution.hasBeenTried) return false;

            if (!CrisisHelper.CanResolveCrisis(crisis, solution,
                dataManager.actionPointData.currentPoints,
                dataManager.directorPlayerData.productionBudget))
            {
                return false;
            }

            crisis.currentSolution = solution;
            solution.hasBeenTried = true;

            if (UnityEngine.Random.value <= solution.successRate)
            {
                crisis.resolutionProgress = 1f;
                crisis.isSolved = true;
                crisis.status = CrisisStatus.Resolved;
                crisis.endTime = DateTime.Now;

                dataManager.heatData.totalHeat = Mathf.Max(0, dataManager.heatData.totalHeat + solution.heatRestored);
                dataManager.directorPlayerData.productionBudget -= solution.cost;

                dataManager.crisisData.resolvedCrises++;

                OnCrisisResolved?.Invoke(crisisId);
                return true;
            }
            else
            {
                crisis.resolutionProgress += 0.3f;
                crisis.actionsTaken++;

                if (crisis.resolutionProgress >= 1f)
                {
                    crisis.status = CrisisStatus.Escalated;
                    dataManager.crisisData.escalatedCrises++;
                    OnCrisisEscalated?.Invoke(crisisId);
                }

                OnCrisisResolutionFailed?.Invoke(crisisId);
                return false;
            }
        }

        /// <summary>
        /// 被动触发危机
        /// </summary>
        public void TriggerRandomCrisis()
        {
            if (dataManager.crisisData.activeCrises.Count >= dataManager.crisisData.config.maxActiveCrises)
            {
                return;
            }

            CrisisType[] types = (CrisisType[])Enum.GetValues(typeof(CrisisType));
            CrisisType randomType = types[Random.Range(0, types.Length)];

            float probability = CrisisHelper.CalculateCrisisProbability(
                dataManager.crisisData,
                randomType,
                dataManager.crisisData.config);

            if (Random.value <= probability)
            {
                CrisisData newCrisis = GenerateCrisis(randomType);
                dataManager.crisisData.activeCrises.Add(newCrisis);
                dataManager.crisisData.totalCrises++;

                OnCrisisStarted?.Invoke(newCrisis.crisisId);
            }
        }

        /// <summary>
        /// 生成危机
        /// </summary>
        private CrisisData GenerateCrisis(CrisisType type)
        {
            CrisisSeverity[] severities = (CrisisSeverity[])Enum.GetValues(typeof(CrisisSeverity));
            int roll = Random.Range(0, 100);
            int cumulative = 0;
            CrisisSeverity severity = CrisisSeverity.Minor;

            int[] probs = {
                dataManager.crisisData.config.minorProbability,
                dataManager.crisisData.config.moderateProbability,
                dataManager.crisisData.config.seriousProbability,
                dataManager.crisisData.config.severeProbability,
                dataManager.crisisData.config.criticalProbability
            };

            for (int i = 0; i < probs.Length; i++)
            {
                cumulative += probs[i];
                if (roll < cumulative)
                {
                    severity = severities[i];
                    break;
                }
            }

            return new CrisisData
            {
                crisisId = Guid.NewGuid().ToString(),
                crisisName = GetCrisisName(type),
                crisisType = type,
                severity = severity,
                status = CrisisStatus.Emerging,
                startTime = DateTime.Now,
                warningMinutes = 60,
                resolutionProgress = 0f,
                actionsTaken = 0,
                maxActions = 5
            };
        }

        /// <summary>
        /// 获取危机名称
        /// </summary>
        private string GetCrisisName(CrisisType type)
        {
            switch (type)
            {
                case CrisisType.PublicRelations: return "公关危机";
                case CrisisType.GuestConflict: return "嘉宾冲突";
                case CrisisType.Technical: return "技术故障";
                case CrisisType.Production: return "制作问题";
                case CrisisType.Legal: return "法律纠纷";
                case CrisisType.Financial: return "财务危机";
                case CrisisType.Safety: return "安全隐患";
                case CrisisType.Reputation: return "声誉受损";
                case CrisisType.Ratings: return "收视率危机";
                case CrisisType.SocialMedia: return "社交媒体风波";
                default: return "未知危机";
            }
        }

        /// <summary>
        /// 危机开始事件
        /// </summary>
        public event Action<string> OnCrisisStarted;

        /// <summary>
        /// 危机解决事件
        /// </summary>
        public event Action<string> OnCrisisResolved;

        /// <summary>
        /// 危机升级事件
        /// </summary>
        public event Action<string> OnCrisisEscalated;

        /// <summary>
        /// 危机解决失败事件
        /// </summary>
        public event Action<string> OnCrisisResolutionFailed;

        #endregion

        #region 剧本管理

        /// <summary>
        /// 创建新剧本
        /// </summary>
        public ScriptData CreateScript(int episodeNumber)
        {
            var script = new ScriptData
            {
                scriptId = Guid.NewGuid().ToString(),
                scriptName = $"第{episodeNumber}集剧本",
                episodeNumber = episodeNumber,
                status = ScriptStatus.Draft
            };

            dataManager.directorPlayerData.scripts.Add(script);
            dataManager.directorPlayerData.currentScript = script;

            OnScriptCreated?.Invoke(script.scriptId);
            return script;
        }

        /// <summary>
        /// 提交剧本审核
        /// </summary>
        public bool SubmitScriptForReview(string scriptId)
        {
            var script = dataManager.directorPlayerData.scripts.Find(s => s.scriptId == scriptId);
            if (script == null || script.status != ScriptStatus.Draft)
            {
                return false;
            }

            script.status = ScriptStatus.InReview;
            script.approvalRating = Random.Range(50, 90);

            OnScriptSubmitted?.Invoke(scriptId);
            return true;
        }

        /// <summary>
        /// 批准剧本
        /// </summary>
        public bool ApproveScript(string scriptId)
        {
            var script = dataManager.directorPlayerData.scripts.Find(s => s.scriptId == scriptId);
            if (script == null || script.status != ScriptStatus.InReview)
            {
                return false;
            }

            script.status = ScriptStatus.Approved;

            int budgetCost = Random.Range(5000, 15000);
            dataManager.directorPlayerData.productionBudget -= budgetCost;

            OnScriptApproved?.Invoke(scriptId);
            return true;
        }

        /// <summary>
        /// 剧本创建事件
        /// </summary>
        public event Action<string> OnScriptCreated;

        /// <summary>
        /// 剧本提交事件
        /// </summary>
        public event Action<string> OnScriptSubmitted;

        /// <summary>
        /// 剧本批准事件
        /// </summary>
        public event Action<string> OnScriptApproved;

        #endregion

        #region 嘉宾管理

        /// <summary>
        /// 安排嘉宾出场
        /// </summary>
        public bool ArrangeGuestAppearance(string guestId, string episodeId)
        {
            if (!dataManager.directorPlayerData.guestManagementDict.ContainsKey(guestId))
            {
                return false;
            }

            var guest = dataManager.directorPlayerData.guestManagementDict[guestId];
            guest.scheduledAppearances.Add(episodeId);

            OnGuestArranged?.Invoke(guestId, episodeId);
            return true;
        }

        /// <summary>
        /// 淘汰嘉宾
        /// </summary>
        public bool EliminateGuest(string guestId, int episode)
        {
            if (!dataManager.directorPlayerData.guestManagementDict.ContainsKey(guestId))
            {
                return false;
            }

            var guest = dataManager.directorPlayerData.guestManagementDict[guestId];
            guest.isEliminated = true;
            guest.eliminationEpisode = episode;
            guest.status = GuestManagementStatus.Eliminated;

            AddHeat(-1000, HeatChangeReason.Elimination, $"嘉宾{guestId}被淘汰");

            OnGuestEliminated?.Invoke(guestId, episode);
            return true;
        }

        /// <summary>
        /// 嘉宾出场安排事件
        /// </summary>
        public event Action<string, string> OnGuestArranged;

        /// <summary>
        /// 嘉宾淘汰事件
        /// </summary>
        public event Action<string, int> OnGuestEliminated;

        #endregion

        #region 话题管理

        /// <summary>
        /// 选择话题
        /// </summary>
        public TopicData SelectTopic(TopicType type, List<string> participantTags)
        {
            return TopicHelper.GetRandomTopic(dataManager.topicLibrary, type, participantTags);
        }

        /// <summary>
        /// 记录话题使用
        /// </summary>
        public void RecordTopicUsage(string topicId, List<string> participantIds, string selectedOptionId, TopicOutcome outcome)
        {
            var record = new TopicUsageRecord
            {
                recordId = Guid.NewGuid().ToString(),
                topicId = topicId,
                usageTime = DateTime.Now,
                participantIds = participantIds,
                selectedOptionId = selectedOptionId,
                outcome = outcome,
                engagementScore = Random.Range(50, 100)
            };

            dataManager.topicLibrary.usageHistory.Add(record);

            if (!dataManager.topicLibrary.topicUsageCount.ContainsKey(topicId))
            {
                dataManager.topicLibrary.topicUsageCount[topicId] = 0;
            }
            dataManager.topicLibrary.topicUsageCount[topicId]++;

            if (outcome == TopicOutcome.Success || outcome == TopicOutcome.Romantic)
            {
                if (!dataManager.topicLibrary.topicSuccessCount.ContainsKey(topicId))
                {
                    dataManager.topicLibrary.topicSuccessCount[topicId] = 0;
                }
                dataManager.topicLibrary.topicSuccessCount[topicId]++;
            }

            dataManager.topicLibrary.totalTopicsUsed++;

            TopicHelper.AddEngagementToHeat(dataManager, record.engagementScore);
        }

        #endregion
    }

    /// <summary>
    /// 话题帮助类扩展
    /// </summary>
    public static class TopicHelperExtensions
    {
        public static void AddEngagementToHeat(DataManager dm, int engagement)
        {
            dm.heatData.totalHeat += engagement / 10;
        }
    }
}
