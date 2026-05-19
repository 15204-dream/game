using UnityEngine;

namespace 糟糕是心动鸭
{
    /// <summary>
    /// 游戏配置数据类
    /// </summary>
    [System.Serializable]
    public class GameConfig
    {
        /// <summary>
        /// 游戏模式
        /// </summary>
        [SerializeField]
        private GameMode gameMode = GameMode.GuestMode;

        /// <summary>
        /// 难度等级 (1-5)
        /// </summary>
        [SerializeField]
        private int difficultyLevel = 1;

        /// <summary>
        /// 是否启用音效
        /// </summary>
        [SerializeField]
        private bool enableSound = true;

        /// <summary>
        /// 背景音乐音量
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float bgmVolume = 0.8f;

        /// <summary>
        /// 音效音量
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float sfxVolume = 1.0f;

        /// <summary>
        /// 是否启用字幕
        /// </summary>
        [SerializeField]
        private bool enableSubtitles = true;

        /// <summary>
        /// 对话显示速度
        /// </summary>
        [SerializeField]
        [Range(0.5f, 2f)]
        private float dialogueSpeed = 1.0f;

        /// <summary>
        /// 自动保存间隔（秒）
        /// </summary>
        [SerializeField]
        private float autoSaveInterval = 60f;

        /// <summary>
        /// 是否启用心动提示
        /// </summary>
        [SerializeField]
        private bool enableHeartbeatIndicator = true;

        /// <summary>
        /// 每回合时间限制（秒）
        /// </summary>
        [SerializeField]
        private float roundTimeLimit = 30f;

        /// <summary>
        /// 好感度变化动画持续时间
        /// </summary>
        [SerializeField]
        private float affectionAnimationDuration = 1.0f;

        /// <summary>
        /// 是否启用快进模式
        /// </summary>
        [SerializeField]
        private bool enableFastForward = false;

        /// <summary>
        /// 导演模式设置
        /// </summary>
        [SerializeField]
        private DirectorModeConfig directorModeConfig = new DirectorModeConfig();

        /// <summary>
        /// 嘉宾模式设置
        /// </summary>
        [SerializeField]
        private GuestModeConfig guestModeConfig = new GuestModeConfig();

        public GameMode GameMode
        {
            get => gameMode;
            set => gameMode = value;
        }

        public int DifficultyLevel
        {
            get => difficultyLevel;
            set => difficultyLevel = Mathf.Clamp(value, 1, 5);
        }

        public bool EnableSound
        {
            get => enableSound;
            set => enableSound = value;
        }

        public float BgmVolume
        {
            get => bgmVolume;
            set => bgmVolume = Mathf.Clamp01(value);
        }

        public float SfxVolume
        {
            get => sfxVolume;
            set => sfxVolume = Mathf.Clamp01(value);
        }

        public bool EnableSubtitles
        {
            get => enableSubtitles;
            set => enableSubtitles = value;
        }

        public float DialogueSpeed
        {
            get => dialogueSpeed;
            set => dialogueSpeed = Mathf.Clamp(value, 0.5f, 2f);
        }

        public float AutoSaveInterval
        {
            get => autoSaveInterval;
            set => autoSaveInterval = Mathf.Max(0f, value);
        }

        public bool EnableHeartbeatIndicator
        {
            get => enableHeartbeatIndicator;
            set => enableHeartbeatIndicator = value;
        }

        public float RoundTimeLimit
        {
            get => roundTimeLimit;
            set => roundTimeLimit = Mathf.Max(0f, value);
        }

        public float AffectionAnimationDuration
        {
            get => affectionAnimationDuration;
            set => affectionAnimationDuration = Mathf.Max(0f, value);
        }

        public bool EnableFastForward
        {
            get => enableFastForward;
            set => enableFastForward = value;
        }

        public DirectorModeConfig DirectorModeConfig
        {
            get => directorModeConfig;
            set => directorModeConfig = value;
        }

        public GuestModeConfig GuestModeConfig
        {
            get => guestModeConfig;
            set => guestModeConfig = value;
        }

        /// <summary>
        /// 导演模式配置
        /// </summary>
        [System.Serializable]
        public class DirectorModeConfig
        {
            /// <summary>
            /// 是否启用剧情编辑
            /// </summary>
            [SerializeField]
            private bool enablePlotEditing = false;

            /// <summary>
            /// 事件生成概率
            /// </summary>
            [SerializeField]
            [Range(0f, 1f)]
            private float eventGenerationRate = 0.7f;

            /// <summary>
            /// 心动CP组合概率
            /// </summary>
            [SerializeField]
            [Range(0f, 1f)]
            private float heartCoupleRate = 0.5f;

            public bool EnablePlotEditing
            {
                get => enablePlotEditing;
                set => enablePlotEditing = value;
            }

            public float EventGenerationRate
            {
                get => eventGenerationRate;
                set => eventGenerationRate = Mathf.Clamp01(value);
            }

            public float HeartCoupleRate
            {
                get => heartCoupleRate;
                set => heartCoupleRate = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// 嘉宾模式配置
        /// </summary>
        [System.Serializable]
        public class GuestModeConfig
        {
            /// <summary>
            /// 玩家选择的角色ID
            /// </summary>
            [SerializeField]
            private string playerCharacterId = "";

            /// <summary>
            /// AI难度 (1-3)
            /// </summary>
            [SerializeField]
            [Range(1, 3)]
            private int aiDifficulty = 2;

            /// <summary>
            /// 是否显示AI思考提示
            /// </summary>
            [SerializeField]
            private bool showAIThinkingHints = true;

            /// <summary>
            /// 心动信号灵敏度
            /// </summary>
            [SerializeField]
            [Range(0f, 1f)]
            private float heartbeatSensitivity = 0.5f;

            public string PlayerCharacterId
            {
                get => playerCharacterId;
                set => playerCharacterId = value;
            }

            public int AiDifficulty
            {
                get => aiDifficulty;
                set => aiDifficulty = Mathf.Clamp(value, 1, 3);
            }

            public bool ShowAIThinkingHints
            {
                get => showAIThinkingHints;
                set => showAIThinkingHints = value;
            }

            public float HeartbeatSensitivity
            {
                get => heartbeatSensitivity;
                set => heartbeatSensitivity = Mathf.Clamp01(value);
            }
        }
    }
}
