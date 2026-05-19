using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 导演引导数据配置
    /// </summary>
    [CreateAssetMenu(fileName = "DirectorGuideData", menuName = "LoveBeat/Guide/DirectorGuideData")]
    public class DirectorGuideData : ScriptableObject
    {
        [Header("阶段配置")]
        [SerializeField] private List<DirectorGuidePhaseConfig> phaseConfigs = new List<DirectorGuidePhaseConfig>();

        [Header("对话配置")]
        [SerializeField] private List<DirectorPhaseDialogue> phaseDialogues = new List<DirectorPhaseDialogue>();

        [Header("引导助手配置")]
        [SerializeField] private GuideAssistantConfig assistantConfig;

        public List<DirectorGuidePhaseConfig> PhaseConfigs => phaseConfigs;
        public GuideAssistantConfig AssistantConfig => assistantConfig;

        /// <summary>
        /// 获取阶段对话
        /// </summary>
        public DirectorPhaseDialogue GetPhaseDialogue(DirectorGuidePhase phase)
        {
            return phaseDialogues.Find(d => d.phase == phase);
        }

        /// <summary>
        /// 获取阶段配置
        /// </summary>
        public DirectorGuidePhaseConfig GetPhaseConfig(DirectorGuidePhase phase)
        {
            return phaseConfigs.Find(c => c.phase == phase);
        }

        /// <summary>
        /// 获取所有阶段
        /// </summary>
        public List<DirectorGuidePhase> GetAllPhases()
        {
            List<DirectorGuidePhase> phases = new List<DirectorGuidePhase>();
            foreach (var config in phaseConfigs)
            {
                phases.Add(config.phase);
            }
            return phases;
        }

        /// <summary>
        /// 创建默认配置
        /// </summary>
        public static DirectorGuideData CreateDefaultConfig()
        {
            DirectorGuideData data = CreateInstance<DirectorGuideData>();

            data.phaseConfigs = new List<DirectorGuidePhaseConfig>
            {
                Create导演上任Phase(),
                Create首期策划Phase(),
                Create热度监控Phase(),
                Create活动安排Phase(),
                Create危机公关Phase()
            };

            data.phaseDialogues = new List<DirectorPhaseDialogue>
            {
                Create导演上任Dialogue(),
                Create首期策划Dialogue(),
                Create热度监控Dialogue()
            };

            data.assistantConfig = CreateDefaultAssistantConfig();

            return data;
        }

        /// <summary>
        /// 创建导演上任阶段
        /// </summary>
        private static DirectorGuidePhaseConfig Create导演上任Phase()
        {
            DirectorGuidePhaseConfig config = new DirectorGuidePhaseConfig
            {
                phase = DirectorGuidePhase.导演上任,
                phaseName = "导演上任",
                description = "了解导演职责，审阅嘉宾资料",
                canSkip = false,
                steps = new List<DirectorGuideStepInfo>
                {
                    new DirectorGuideStepInfo
                    {
                        stepId = "director_1",
                        stepName = "节目组邀请",
                        description = "接收节目组的导演邀请",
                        stepType = DirectorGuideStepType.Dialogue,
                        targetPath = "UI/InvitationPanel",
                        canSkip = false
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "director_2",
                        stepName = "了解导演职责",
                        description = "学习作为导演需要做的事情",
                        stepType = DirectorGuideStepType.Tutorial,
                        targetPath = "UI/DirectorRolePanel",
                        canSkip = false
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "director_3",
                        stepName = "审阅嘉宾资料",
                        description = "查看12位嘉宾的详细资料",
                        stepType = DirectorGuideStepType.Assignment,
                        targetPath = "UI/GuestProfilesPanel",
                        canSkip = false
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "director_4",
                        stepName = "完成上任",
                        description = "确认导演身份，正式上任",
                        stepType = DirectorGuideStepType.Dialogue,
                        targetPath = "UI/ConfirmButton",
                        canSkip = false
                    }
                },
                rewards = new List<GuideReward>
                {
                    new GuideReward { rewardType = GuideRewardType.Currency, currencyType = "directors_coins", amount = 500 }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建首期策划阶段
        /// </summary>
        private static DirectorGuidePhaseConfig Create首期策划Phase()
        {
            DirectorGuidePhaseConfig config = new DirectorGuidePhaseConfig
            {
                phase = DirectorGuidePhase.首期策划,
                phaseName = "首期策划",
                description = "查看数据面板，安排首期活动",
                canSkip = false,
                steps = new List<DirectorGuideStepInfo>
                {
                    new DirectorGuideStepInfo
                    {
                        stepId = "planning_1",
                        stepName = "查看数据面板",
                        description = "了解节目数据概览",
                        stepType = DirectorGuideStepType.Tutorial,
                        targetPath = "UI/DataDashboardPanel",
                        canSkip = false
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "planning_2",
                        stepName = "了解嘉宾资料",
                        description = "查看12位嘉宾的详细资料",
                        stepType = DirectorGuideStepType.Assignment,
                        targetPath = "UI/GuestListPanel",
                        canSkip = false
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "planning_3",
                        stepName = "安排首期活动",
                        description = "为第一天的节目安排活动",
                        stepType = DirectorGuideStepType.Assignment,
                        targetPath = "UI/ActivityPlannerPanel",
                        canSkip = false
                    }
                },
                rewards = new List<GuideReward>
                {
                    new GuideReward { rewardType = GuideRewardType.Item, itemId = "golden_producer_pass", amount = 1 }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建热度监控阶段
        /// </summary>
        private static DirectorGuidePhaseConfig Create热度监控Phase()
        {
            DirectorGuidePhaseConfig config = new DirectorGuidePhaseConfig
            {
                phase = DirectorGuidePhase.热度监控,
                phaseName = "热度监控",
                description = "监控节目热度，优化内容策略",
                canSkip = true,
                steps = new List<DirectorGuideStepInfo>
                {
                    new DirectorGuideStepInfo
                    {
                        stepId = "heat_1",
                        stepName = "查看热度数据",
                        description = "监控节目的实时热度",
                        stepType = DirectorGuideStepType.Monitoring,
                        targetPath = "UI/HeatMonitorPanel",
                        canSkip = true
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "heat_2",
                        stepName = "分析观众反馈",
                        description = "查看观众的评论和反馈",
                        stepType = DirectorGuideStepType.Monitoring,
                        targetPath = "UI/FeedbackPanel",
                        canSkip = true
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "heat_3",
                        stepName = "调整节目策略",
                        description = "根据数据调整节目安排",
                        stepType = DirectorGuideStepType.Assignment,
                        targetPath = "UI/StrategyPanel",
                        canSkip = true
                    }
                },
                rewards = new List<GuideReward>
                {
                    new GuideReward { rewardType = GuideRewardType.Currency, currencyType = "directors_coins", amount = 200 }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建活动安排阶段
        /// </summary>
        private static DirectorGuidePhaseConfig Create活动安排Phase()
        {
            DirectorGuidePhaseConfig config = new DirectorGuidePhaseConfig
            {
                phase = DirectorGuidePhase.活动安排,
                phaseName = "活动安排",
                description = "策划和安排各种心动活动",
                canSkip = true,
                steps = new List<DirectorGuideStepInfo>
                {
                    new DirectorGuideStepInfo
                    {
                        stepId = "activity_1",
                        stepName = "查看活动模板",
                        description = "了解可用的活动模板",
                        stepType = DirectorGuideStepType.Tutorial,
                        targetPath = "UI/ActivityTemplatesPanel",
                        canSkip = true
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "activity_2",
                        stepName = "创建新活动",
                        description = "策划一个心动活动",
                        stepType = DirectorGuideStepType.Assignment,
                        targetPath = "UI/ActivityCreatorPanel",
                        canSkip = true
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "activity_3",
                        stepName = "安排活动时间",
                        description = "设置活动的时间和规则",
                        stepType = DirectorGuideStepType.Assignment,
                        targetPath = "UI/SchedulePanel",
                        canSkip = true
                    }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建危机公关阶段
        /// </summary>
        private static DirectorGuidePhaseConfig Create危机公关Phase()
        {
            DirectorGuidePhaseConfig config = new DirectorGuidePhaseConfig
            {
                phase = DirectorGuidePhase.危机公关,
                phaseName = "危机公关",
                description = "处理突发情况，保护节目声誉",
                canSkip = true,
                steps = new List<DirectorGuideStepInfo>
                {
                    new DirectorGuideStepInfo
                    {
                        stepId = "crisis_1",
                        stepName = "查看关系网络",
                        description = "了解嘉宾之间的关系",
                        stepType = DirectorGuideStepType.Tutorial,
                        targetPath = "UI/RelationshipNetworkPanel",
                        canSkip = true
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "crisis_2",
                        stepName = "识别潜在问题",
                        description = "发现可能出现的危机",
                        stepType = DirectorGuideStepType.Monitoring,
                        targetPath = "UI/CrisisAlertPanel",
                        canSkip = true
                    },
                    new DirectorGuideStepInfo
                    {
                        stepId = "crisis_3",
                        stepName = "处理模拟危机",
                        description = "练习处理一次小危机",
                        stepType = DirectorGuideStepType.Crisis,
                        targetPath = "UI/CrisisHandlerPanel",
                        canSkip = true
                    }
                },
                rewards = new List<GuideReward>
                {
                    new GuideReward { rewardType = GuideRewardType.Item, itemId = "crisis_management_manual", amount = 1 }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建导演上任对话
        /// </summary>
        private static DirectorPhaseDialogue Create导演上任Dialogue()
        {
            return new DirectorPhaseDialogue
            {
                phase = DirectorGuidePhase.导演上任,
                dialogues = new List<GuideDialogueLine>
                {
                    new GuideDialogueLine
                    {
                        speaker = "制作人",
                        text = "恭喜你被选中成为《糟糕！是心动鸭！》的新导演！",
                        displayTime = 3f
                    },
                    new GuideDialogueLine
                    {
                        speaker = "制作人",
                        text = "作为导演，你需要管理节目流程、安排活动、监控热度，还要处理各种突发情况。",
                        displayTime = 4f
                    },
                    new GuideDialogueLine
                    {
                        speaker = "制作人",
                        text = "让我来给你介绍一下作为导演的具体职责吧~",
                        displayTime = 3f
                    }
                }
            };
        }

        /// <summary>
        /// 创建首期策划对话
        /// </summary>
        private static DirectorPhaseDialogue Create首期策划Dialogue()
        {
            return new DirectorPhaseDialogue
            {
                phase = DirectorGuidePhase.首期策划,
                dialogues = new List<GuideDialogueLine>
                {
                    new GuideDialogueLine
                    {
                        speaker = "制作人",
                        text = "作为新导演，你需要为节目的第一期做策划。",
                        displayTime = 3f
                    },
                    new GuideDialogueLine
                    {
                        speaker = "制作人",
                        text = "先来了解一下节目的数据面板，以及12位嘉宾的详细资料吧~",
                        displayTime = 4f
                    }
                }
            };
        }

        /// <summary>
        /// 创建热度监控对话
        /// </summary>
        private static DirectorPhaseDialogue Create热度监控Dialogue()
        {
            return new DirectorPhaseDialogue
            {
                phase = DirectorGuidePhase.热度监控,
                dialogues = new List<GuideDialogueLine>
                {
                    new GuideDialogueLine
                    {
                        speaker = "制作人",
                        text = "作为导演，你需要时刻关注节目的热度变化。",
                        displayTime = 3f
                    },
                    new GuideDialogueLine
                    {
                        speaker = "制作人",
                        text = "根据观众的反馈和数据，及时调整节目策略，才能让节目更受欢迎哦~",
                        displayTime = 4f
                    }
                }
            };
        }

        /// <summary>
        /// 创建默认引导助手配置
        /// </summary>
        private static GuideAssistantConfig CreateDefaultAssistantConfig()
        {
            return new GuideAssistantConfig
            {
                assistantName = "小Nova",
                assistantSprite = null,
                showBubbleOnStart = true,
                bubbleDisplayTime = 3f,
                enableVoice = false
            };
        }
    }
}
