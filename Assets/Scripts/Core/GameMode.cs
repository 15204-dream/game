namespace 糟糕是心动鸭
{
    /// <summary>
    /// 游戏模式枚举
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// 嘉宾模式 - 玩家作为恋综嘉宾参与节目
        /// </summary>
        GuestMode,

        /// <summary>
        /// 导演模式 - 玩家作为节目导演控制节目流程
        /// </summary>
        DirectorMode
    }

    /// <summary>
    /// 游戏阶段枚举
    /// </summary>
    public enum GameStage
    {
        /// <summary>
        /// 主菜单
        /// </summary>
        MainMenu,

        /// <summary>
        /// 角色选择
        /// </summary>
        CharacterSelection,

        /// <summary>
        /// 节目进行中
        /// </summary>
        ShowInProgress,

        /// <summary>
        /// 回合结束
        /// </summary>
        RoundEnd,

        /// <summary>
        /// 节目结束
        /// </summary>
        ShowEnd
    }

    /// <summary>
    /// 事件类型枚举
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// 心动选择
        /// </summary>
        HeartChoice,

        /// <summary>
        /// 约会事件
        /// </summary>
        DateEvent,

        /// <summary>
        /// 群体活动
        /// </summary>
        GroupActivity,

        /// <summary>
        /// 秘密任务
        /// </summary>
        SecretTask,

        /// <summary>
        /// 最终告白
        /// </summary>
        FinalConfession
    }

    /// <summary>
    /// 关系状态枚举
    /// </summary>
    public enum RelationshipStatus
    {
        /// <summary>
        /// 陌生
        /// </summary>
        Stranger,

        /// <summary>
        /// 认识
        /// </summary>
        Acquaintance,

        /// <summary>
        /// 朋友
        /// </summary>
        Friend,

        /// <summary>
        /// 好感
        /// </summary>
        Good Impression,

        /// <summary>
        /// 心动的信号
        /// </summary>
        Heartbeat,

        /// <summary>
        /// 恋人
        /// </summary>
        Lover
    }

    /// <summary>
    /// 选择结果枚举
    /// </summary>
    public enum ChoiceResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,

        /// <summary>
        /// 失败
        /// </summary>
        Failure,

        /// <summary>
        /// 平局
        /// </summary>
        Draw
    }
}
