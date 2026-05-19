using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveBeatGuide
{
    /// <summary>
    /// 嘉宾引导数据配置
    /// </summary>
    [CreateAssetMenu(fileName = "GuestGuideData", menuName = "LoveBeat/Guide/GuestGuideData")]
    public class GuestGuideData : ScriptableObject
    {
        [Header("阶段配置")]
        [SerializeField] private List<GuestGuidePhaseConfig> phaseConfigs = new List<GuestGuidePhaseConfig>();

        [Header("对话配置")]
        [SerializeField] private List<GuestPhaseDialogue> phaseDialogues = new List<GuestPhaseDialogue>();

        [Header("引导助手配置")]
        [SerializeField] private GuideAssistantConfig assistantConfig;

        public List<GuestGuidePhaseConfig> PhaseConfigs => phaseConfigs;
        public GuideAssistantConfig AssistantConfig => assistantConfig;

        /// <summary>
        /// 获取阶段对话
        /// </summary>
        public GuestPhaseDialogue GetPhaseDialogue(GuestGuidePhase phase)
        {
            return phaseDialogues.Find(d => d.phase == phase);
        }

        /// <summary>
        /// 获取阶段配置
        /// </summary>
        public GuestGuidePhaseConfig GetPhaseConfig(GuestGuidePhase phase)
        {
            return phaseConfigs.Find(c => c.phase == phase);
        }

        /// <summary>
        /// 获取所有阶段
        /// </summary>
        public List<GuestGuidePhase> GetAllPhases()
        {
            List<GuestGuidePhase> phases = new List<GuestGuidePhase>();
            foreach (var config in phaseConfigs)
            {
                phases.Add(config.phase);
            }
            return phases;
        }

        /// <summary>
        /// 创建默认配置
        /// </summary>
        public static GuestGuideData CreateDefaultConfig()
        {
            GuestGuideData data = CreateInstance<GuestGuideData>();

            data.phaseConfigs = new List<GuestGuidePhaseConfig>
            {
                Create入场准备Phase(),
                Create入住心动小屋Phase(),
                Create心动信箱Phase(),
                Create任务系统Phase(),
                Create社交互动Phase()
            };

            data.phaseDialogues = new List<GuestPhaseDialogue>
            {
                Create入场准备Dialogue(),
                Create入住心动小屋Dialogue(),
                Create心动信箱Dialogue()
            };

            data.assistantConfig = CreateDefaultAssistantConfig();

            return data;
        }

        /// <summary>
        /// 创建入场准备阶段
        /// </summary>
        private static GuestGuidePhaseConfig Create入场准备Phase()
        {
            GuestGuidePhaseConfig config = new GuestGuidePhaseConfig
            {
                phase = GuestGuidePhase.入场准备,
                phaseName = "入场准备",
                description = "了解节目规则，创建角色档案",
                canSkip = false,
                steps = new List<GuestGuideStepInfo>
                {
                    new GuestGuideStepInfo
                    {
                        stepId = "entry_1",
                        stepName = "节目组来电",
                        description = "接听节目组的邀请电话",
                        stepType = GuestGuideStepType.Dialogue,
                        targetPath = "UI/PhoneCallPanel",
                        canSkip = false
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "entry_2",
                        stepName = "了解节目规则",
                        description = "学习心动小屋的基本规则",
                        stepType = GuestGuideStepType.Tutorial,
                        targetPath = "UI/RulesPanel",
                        canSkip = false
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "entry_3",
                        stepName = "创建角色档案",
                        description = "填写你的个人信息",
                        stepType = GuestGuideStepType.Interaction,
                        targetPath = "UI/ProfileEditor",
                        canSkip = false
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "entry_4",
                        stepName = "完成入场",
                        description = "确认信息，准备入住心动小屋",
                        stepType = GuestGuideStepType.Task,
                        targetPath = "UI/ConfirmButton",
                        canSkip = false
                    }
                },
                rewards = new List<GuideReward>
                {
                    new GuideReward { rewardType = GuideRewardType.Item, itemId = "welcome_gift", amount = 1 }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建入住心动小屋阶段
        /// </summary>
        private static GuestGuidePhaseConfig Create入住心动小屋Phase()
        {
            GuestGuidePhaseConfig config = new GuestGuidePhaseConfig
            {
                phase = GuestGuidePhase.入住心动小屋,
                phaseName = "入住心动小屋",
                description = "认识新朋友，探索心动小屋",
                canSkip = false,
                steps = new List<GuestGuideStepInfo>
                {
                    new GuestGuideStepInfo
                    {
                        stepId = "checkin_1",
                        stepName = "自我介绍",
                        description = "在大家面前介绍自己",
                        stepType = GuestGuideStepType.Interaction,
                        targetPath = "UI/IntroductionPanel",
                        canSkip = false
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "checkin_2",
                        stepName = "认识第一位嘉宾",
                        description = "和新室友打个招呼",
                        stepType = GuestGuideStepType.Interaction,
                        targetPath = "UI/GuestList/FirstGuest",
                        canSkip = false
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "checkin_3",
                        stepName = "探索心动小屋",
                        description = "熟悉小屋的各个区域",
                        stepType = GuestGuideStepType.Tutorial,
                        targetPath = "UI/MapPanel",
                        canSkip = true
                    }
                },
                rewards = new List<GuideReward>
                {
                    new GuideReward { rewardType = GuideRewardType.Currency, currencyType = "hearts", amount = 100 }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建心动信箱阶段
        /// </summary>
        private static GuestGuidePhaseConfig Create心动信箱Phase()
        {
            GuestGuidePhaseConfig config = new GuestGuidePhaseConfig
            {
                phase = GuestGuidePhase.心动信箱,
                phaseName = "心动信箱",
                description = "学习使用心动信箱，写出第一封信",
                canSkip = false,
                steps = new List<GuestGuideStepInfo>
                {
                    new GuestGuideStepInfo
                    {
                        stepId = "mail_1",
                        stepName = "学习心动信箱使用",
                        description = "了解心动信箱的功能",
                        stepType = GuestGuideStepType.Tutorial,
                        targetPath = "UI/MailboxPanel",
                        canSkip = false
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "mail_2",
                        stepName = "写出第一封信",
                        description = "给你心仪的对象写一封信",
                        stepType = GuestGuideStepType.Interaction,
                        targetPath = "UI/MailboxPanel/WriteButton",
                        canSkip = false
                    }
                },
                rewards = new List<GuideReward>
                {
                    new GuideReward { rewardType = GuideRewardType.Item, itemId = "love_stamp", amount = 1 }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建任务系统阶段
        /// </summary>
        private static GuestGuidePhaseConfig Create任务系统Phase()
        {
            GuestGuidePhaseConfig config = new GuestGuidePhaseConfig
            {
                phase = GuestGuidePhase.任务系统,
                phaseName = "任务系统",
                description = "了解每日任务，获取更多奖励",
                canSkip = true,
                steps = new List<GuestGuideStepInfo>
                {
                    new GuestGuideStepInfo
                    {
                        stepId = "task_1",
                        stepName = "查看每日任务",
                        description = "了解可完成的每日任务",
                        stepType = GuestGuideStepType.Tutorial,
                        targetPath = "UI/TaskPanel",
                        canSkip = true
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "task_2",
                        stepName = "接受任务",
                        description = "接受一个每日任务",
                        stepType = GuestGuideStepType.Interaction,
                        targetPath = "UI/TaskPanel/AcceptButton",
                        canSkip = true
                    }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建社交互动阶段
        /// </summary>
        private static GuestGuidePhaseConfig Create社交互动Phase()
        {
            GuestGuidePhaseConfig config = new GuestGuidePhaseConfig
            {
                phase = GuestGuidePhase.社交互动,
                phaseName = "社交互动",
                description = "和其他嘉宾互动，提升好感度",
                canSkip = true,
                steps = new List<GuestGuideStepInfo>
                {
                    new GuestGuideStepInfo
                    {
                        stepId = "social_1",
                        stepName = "发起对话",
                        description = "和其他嘉宾聊天",
                        stepType = GuestGuideStepType.Interaction,
                        targetPath = "UI/GuestList/TalkButton",
                        canSkip = true
                    },
                    new GuestGuideStepInfo
                    {
                        stepId = "social_2",
                        stepName = "赠送礼物",
                        description = "给心仪的对象送个小礼物",
                        stepType = GuestGuideStepType.Interaction,
                        targetPath = "UI/GiftPanel",
                        canSkip = true
                    }
                }
            };

            return config;
        }

        /// <summary>
        /// 创建入场准备对话
        /// </summary>
        private static GuestPhaseDialogue Create入场准备Dialogue()
        {
            return new GuestPhaseDialogue
            {
                phase = GuestGuidePhase.入场准备,
                dialogues = new List<GuideDialogueLine>
                {
                    new GuideDialogueLine
                    {
                        speaker = "小Nova",
                        text = "哇，新嘉宾要来啦！欢迎欢迎~我是你的专属引导助手小Nova！",
                        displayTime = 3f
                    },
                    new GuideDialogueLine
                    {
                        speaker = "小Nova",
                        text = "在入住心动小屋之前，让我先给你介绍一下节目的基本规则吧~",
                        displayTime = 3f
                    }
                }
            };
        }

        /// <summary>
        /// 创建入住心动小屋对话
        /// </summary>
        private static GuestPhaseDialogue Create入住心动小屋Dialogue()
        {
            return new GuestPhaseDialogue
            {
                phase = GuestGuidePhase.入住心动小屋,
                dialogues = new List<GuideDialogueLine>
                {
                    new GuideDialogueLine
                    {
                        speaker = "小Nova",
                        text = "恭喜你正式入住心动小屋啦！这里就是你在节目中的家~",
                        displayTime = 3f
                    },
                    new GuideDialogueLine
                    {
                        speaker = "小Nova",
                        text = "快去认识一下其他嘉宾吧！记得给大家留个好印象哦~",
                        displayTime = 3f
                    }
                }
            };
        }

        /// <summary>
        /// 创建心动信箱对话
        /// </summary>
        private static GuestPhaseDialogue Create心动信箱Dialogue()
        {
            return new GuestPhaseDialogue
            {
                phase = GuestGuidePhase.心动信箱,
                dialogues = new List<GuideDialogueLine>
                {
                    new GuideDialogueLine
                    {
                        speaker = "小Nova",
                        text = "心动信箱是我们节目的核心玩法哦！每天你可以写信给你心仪的对象~",
                        displayTime = 3f
                    },
                    new GuideDialogueLine
                    {
                        speaker = "小Nova",
                        text = "收到你的信的人会在第二天早上看到内容，怎么样，是不是很浪漫？",
                        displayTime = 3f
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

    /// <summary>
    /// 引导助手配置
    /// </summary>
    [Serializable]
    public class GuideAssistantConfig
    {
        public string assistantName;
        public Sprite assistantSprite;
        public bool showBubbleOnStart;
        public float bubbleDisplayTime;
        public bool enableVoice;
        public Color bubbleColor = new Color(1f, 0.85f, 0.9f);
        public Vector2 bubbleOffset = new Vector2(0, 100);
    }
}
