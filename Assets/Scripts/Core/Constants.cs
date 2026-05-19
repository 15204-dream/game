namespace 糟糕是心动鸭
{
    /// <summary>
    /// 游戏常量定义类
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 游戏名称
        /// </summary>
        public const string GAME_NAME = "糟糕！是心动鸭！";

        /// <summary>
        /// 游戏版本
        /// </summary>
        public const string GAME_VERSION = "1.0.0";

        /// <summary>
        /// 角色最大数量
        /// </summary>
        public const int MAX_CHARACTER_COUNT = 8;

        /// <summary>
        /// 每期节目回合数
        /// </summary>
        public const int ROUNDS_PER_SHOW = 5;

        /// <summary>
        /// 最大好感度
        /// </summary>
        public const int MAX_AFFECTION = 100;

        /// <summary>
        /// 初始好感度
        /// </summary>
        public const int INITIAL_AFFECTION = 20;

        /// <summary>
        /// 心动阈值
        /// </summary>
        public const int HEARTBEAT_THRESHOLD = 60;

        /// <summary>
        /// 告白成功阈值
        /// </summary>
        public const int CONFESSION_SUCCESS_THRESHOLD = 80;

        /// <summary>
        /// 每日选择次数
        /// </summary>
        public const int DAILY_CHOICE_COUNT = 3;

        /// <summary>
        /// 秘密任务奖励倍率
        /// </summary>
        public const float SECRET_TASK_REWARD_MULTIPLIER = 1.5f;

        /// <summary>
        /// 约会事件奖励倍率
        /// </summary>
        public const float DATE_EVENT_REWARD_MULTIPLIER = 2.0f;

        /// <summary>
        /// 数据保存路径
        /// </summary>
        public const string SAVE_DATA_PATH = "SaveData";

        /// <summary>
        /// 存档文件名格式
        /// </summary>
        public const string SAVE_FILE_FORMAT = "save_{0}.json";

        /// <summary>
        /// 最大存档数量
        /// </summary>
        public const int MAX_SAVE_SLOTS = 3;

        /// <summary>
        /// UI动画持续时间
        /// </summary>
        public const float UI_ANIMATION_DURATION = 0.3f;

        /// <summary>
        /// 对话打字速度
        /// </summary>
        public const float DIALOGUE_TYPING_SPEED = 0.05f;

        /// <summary>
        /// 场景名称定义
        /// </summary>
        public static class SceneNames
        {
            public const string MAIN_MENU = "MainMenu";
            public const string CHARACTER_SELECT = "CharacterSelect";
            public const string GAME_SCENE = "GameScene";
            public const string SHOW_RESULT = "ShowResult";
        }

        /// <summary>
        /// 资源路径定义
        /// </summary>
        public static class ResourcePaths
        {
            public const string CHARACTER_DATA = "Data/CharacterData";
            public const string EVENT_DATA = "Data/EventData";
            public const string DIALOGUE_DATA = "Data/DialogueData";
            public const string UI_PREFABS = "Prefabs/UI";
            public const string CHARACTER_PREFABS = "Prefabs/Characters";
        }

        /// <summary>
        /// 事件标签定义
        /// </summary>
        public static class EventTags
        {
            public const string HEART_EVENT = "HeartEvent";
            public const string FUNNY_EVENT = "FunnyEvent";
            public const string DRAMATIC_EVENT = "DramaticEvent";
            public const string ROMANTIC_EVENT = "RomanticEvent";
        }

        /// <summary>
        /// 角色标签定义
        /// </summary>
        public static class CharacterTags
        {
            public const string MALE = "Male";
            public const string FEMALE = "Female";
            public const string PLAYER = "Player";
            public const string NPC = "NPC";
        }
    }
}
